#if UNITY_EDITOR
using System;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;

namespace Utils.Dependency
{
	public static class SerializedPropertyUtility
	{
		public static SerializedProperty FindPropertyRelativeOrFail(this SerializedProperty property, string path)
		{
			var subProperty = property.FindPropertyRelative(path);

			if (subProperty == null)
			{
				throw new InvalidOperationException($"Failed to find property '{SerializedObjectLabel(property.serializedObject)}.{property.propertyPath}.{path}'.");
			}

			return subProperty;
		}

		public static SerializedProperty FindPropertyOrFail(this SerializedObject serializedObject, string path)
		{
			var property = serializedObject.FindProperty(path);

			if (property == null)
			{
				throw new InvalidOperationException($"Failed to find property '{SerializedObjectLabel(serializedObject)}.{path}'.");
			}

			return property;
		}

		#region Reflection

		public static string FixedPropertyPath(this SerializedProperty property)
		{
			// Unity structures array paths like "fieldName.Array.data[i]".
			// Fix that quirk and directly go to index, i.e. "fieldName[i]".
			return property.propertyPath.Replace(".Array.data[", "[");
		}

		public static string[] PropertyPathParts(this SerializedProperty property)
		{
			return property.FixedPropertyPath().Split('.');
		}

		public static bool IsPropertyIndexer(string propertyPart, out string fieldName, out int index)
		{
			var regex = new Regex(@"(.+)\[(\d+)\]");
			var match = regex.Match(propertyPart);

			if (match.Success) // Property refers to an array or list
			{
				fieldName = match.Groups[1].Value;
				index = int.Parse(match.Groups[2].Value);
				return true;
			}
			else
			{
				fieldName = propertyPart;
				index = -1;
				return false;
			}
		}

		private static void EnsureReflectable(SerializedProperty property, bool allowMultiple = false)
		{
			if (property == null)
			{
				throw new ArgumentNullException(nameof(property));
			}

			if (!allowMultiple && property.serializedObject.isEditingMultipleObjects)
			{
				throw new NotSupportedException($"Attempting to reflect property '{property.propertyPath}' on multiple objects.");
			}

			if (property.serializedObject.targetObject == null)
			{
				throw new NotSupportedException($"Attempting to reflect property '{property.propertyPath}' on a null object.");
			}
		}

		private static string SerializedObjectLabel(SerializedObject serializedObject)
		{
			return serializedObject.isEditingMultipleObjects ? "[Multiple]" : serializedObject.targetObject.GetType().Name;
		}

		private static void GetUnderlying(this SerializedProperty property, out object parent, out FieldInfo field, out object value)
		{
			EnsureReflectable(property);

			parent = property.serializedObject.targetObject;
			var parts = PropertyPathParts(property);
			field = null;
			value = null;

			for (var i = 0; i < parts.Length; i++)
			{
				var part = parts[i];

				if (parent == null)
				{
					throw new NullReferenceException($"Parent of '{SerializedObjectLabel(property.serializedObject)}.{string.Join(".", parts, 0, i + 1)}' is null.");
				}

				field = GetPropertyPartField(part, parent);

				if (i < parts.Length - 1)
				{
					parent = GetPropertyPartValue(part, parent);
				}
				else
				{
					value = GetPropertyPartValue(part, parent);
				}
			}
		}

		private static void SetUnderlying(this SerializedProperty property, object value)
		{
			EnsureReflectable(property);

			object parent = property.serializedObject.targetObject;
			var parts = PropertyPathParts(property);

			for (var i = 0; i < parts.Length; i++)
			{
				var part = parts[i];

				if (parent == null)
				{
					throw new NullReferenceException($"Parent of '{SerializedObjectLabel(property.serializedObject)}.{string.Join(".", parts, 0, i + 1)}' is null.");
				}

				if (i < parts.Length - 1)
				{
					parent = GetPropertyPartValue(part, parent);
				}
				else
				{
					SetPropertyPartValue(part, parent, value);
				}
			}
		}

		public static FieldInfo GetUnderlyingField(this SerializedProperty property)
		{
			GetUnderlying(property, out var parent, out var field, out var value);
			return field;
		}

		public static object GetUnderlyingValue(this SerializedProperty property)
		{
			GetUnderlying(property, out var parent, out var field, out var value);
			return value;
		}

		public static Type GetUnderlyingType(this SerializedProperty property)
		{
			return GetUnderlyingField(property).FieldType;
		}

		public static void SetUnderlyingValue(this SerializedProperty property, object value)
		{
			EnsureReflectable(property);

			// Serialize so we don't overwrite other modifications with our deserialization later
			property.serializedObject.ApplyModifiedPropertiesWithoutUndo();

			SetUnderlying(property, value);

			// Deserialize the object for continued operations after this call
			property.serializedObject.Update();
		}

		private static FieldInfo GetPropertyPartField(string propertyPathPart, object parent)
		{
			IsPropertyIndexer(propertyPathPart, out var fieldName, out var index);

			return GetSerializedFieldInfo(parent.GetType(), fieldName);
		}

		private static object GetPropertyPartValue(string propertyPathPart, object parent)
		{
			var value = GetPropertyPartField(propertyPathPart, parent).GetValue(parent);

			if (IsPropertyIndexer(propertyPathPart, out var fieldName, out var index))
			{
				return ((IList)value)[index];
			}
			else
			{
				return value;
			}
		}

		private static void SetPropertyPartValue(string propertyPathPart, object parent, object value)
		{
			var field = GetPropertyPartField(propertyPathPart, parent);

			if (IsPropertyIndexer(propertyPathPart, out var fieldName, out var index))
			{
				((IList)field.GetValue(parent))[index] = value;
			}
			else
			{
				field.SetValue(parent, value);
			}
		}

		private static FieldInfo GetSerializedFieldInfo(Type type, string name)
		{
			var currentType = type;

			while (currentType != null)
			{
				var field = currentType.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

				if (field != null)
				{
					return field;
				}

				currentType = currentType.BaseType;
			}

			throw new MissingMemberException(type.FullName, name);
		}

		#endregion Reflection

		#region Composite Read / Write

		public static T ReadEnum<T>(this SerializedProperty property) where T : Enum
		{
			// TODO: Bug report to Unity
			// When using SerializeReference instead of SerializeField on a parent object,
			// the underlying property type becomes integer instead of enum.
			if (property.propertyType == SerializedPropertyType.Enum)
			{
				return (T)Enum.ToObject(typeof(T), property.enumValueIndex);
			}
			else if (property.propertyType == SerializedPropertyType.Integer)
			{
				return (T)Enum.ToObject(typeof(T), property.intValue);
			}
			else
			{
				throw new NotSupportedException();
			}
		}

		public static void WriteEnum<T>(this SerializedProperty property, T value) where T : Enum
		{
			// TODO: Bug report to Unity
			// When using SerializeReference instead of SerializeField on a parent object,
			// the underlying property type becomes integer instead of enum.
			if (property.propertyType == SerializedPropertyType.Enum)
			{
				property.enumValueIndex = Convert.ToInt32(value);
			}
			else if (property.propertyType == SerializedPropertyType.Integer)
			{
				property.intValue = Convert.ToInt32(value);
			}
			else
			{
				throw new NotSupportedException();
			}
		}

		public static string[] ReadStringArray(this SerializedProperty property)
		{
			return property.ReadArray(p => p.stringValue);
		}

		public static void WriteStringArray(this SerializedProperty property, string[] array)
		{
			property.WriteArray(array, (p, v) => p.stringValue = v);
		}

		public static int[] ReadIntArray(this SerializedProperty property)
		{
			return property.ReadArray(p => p.intValue);
		}

		public static void WriteIntArray(this SerializedProperty property, int[] array)
		{
			property.WriteArray(array, (p, v) => p.intValue = v);
		}

		public static byte[] ReadByteArray(this SerializedProperty property)
		{
			return property.ReadArray(p => (byte)p.intValue);
		}

		public static void WriteByteArray(this SerializedProperty property, byte[] array)
		{
			property.WriteArray(array, (p, v) => p.intValue = v);
		}

		public static T[] ReadArray<T>(this SerializedProperty property, Func<SerializedProperty, T> getValue)
		{
			var array = new T[property.arraySize];

			for (var i = 0; i < array.Length; i++)
			{
				array[i] = getValue(property.GetArrayElementAtIndex(i));
			}

			return array;
		}

		public static void WriteArray<T>(this SerializedProperty property, T[] array, Action<SerializedProperty, T> setValue)
		{
			if (array == null)
			{
				property.arraySize = 0;
				return;
			}

			property.arraySize = array.Length;

			for (var i = 0; i < array.Length; i++)
			{
				setValue(property.GetArrayElementAtIndex(i), array[i]);
			}
		}

		#endregion Composite Read / Write
	}
}

#endif