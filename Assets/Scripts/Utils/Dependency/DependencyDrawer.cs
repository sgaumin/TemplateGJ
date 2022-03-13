#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Utils.Dependency
{
	[CustomPropertyDrawer(typeof(IDependency), true)]
	public sealed class DependencyDrawer : PropertyDrawer
	{
		private SerializedProperty sourceProperty;

		private SerializedProperty localReferenceProperty;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			EditorGUI.BeginChangeCheck();

			var controlPosition = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			sourceProperty = property.FindPropertyRelativeOrFail("_" + nameof(IDependency.source));
			localReferenceProperty = property.FindPropertyRelativeOrFail("_" + nameof(Dependency<Component>.localReference));

			EditorGUI.showMixedValue = sourceProperty.hasMultipleDifferentValues;

			var sourcePosition = new Rect
			(
			  controlPosition.x,
			  controlPosition.y,
			  Styles.sourceWidth,
			  EditorGUIUtility.singleLineHeight
			);

			var fieldPosition = new Rect
			(
			  sourcePosition.xMax + Styles.spaceBetweenSourceAndField,
			  controlPosition.y,
			  controlPosition.width - Styles.sourceWidth - Styles.spaceBetweenSourceAndField,
			  EditorGUIUtility.singleLineHeight
			);

			// Draw source field

			EditorGUI.PropertyField(sourcePosition, sourceProperty, GUIContent.none);

			// Draw reference field

			if (sourceProperty.hasMultipleDifferentValues)
			{
				EditorGUI.BeginDisabledGroup(true);
				EditorGUI.LabelField(fieldPosition, "\u2014", EditorStyles.textField);
				EditorGUI.EndDisabledGroup();
			}
			else
			{
				var source = (DependencySource)sourceProperty.enumValueIndex;

				if (source == DependencySource.Local)
				{
					EditorGUI.PropertyField(fieldPosition, localReferenceProperty, GUIContent.none);
				}
				else
				{
					EditorGUI.BeginDisabledGroup(true);

					if (property.serializedObject.isEditingMultipleObjects)
					{
						EditorGUI.LabelField(fieldPosition, "\u2014", EditorStyles.textField);
					}
					else
					{
						var dependency = (IDependency)property.GetUnderlyingValue();
						var self = (Component)property.serializedObject.targetObject;
						var component = dependency.Resolve(self);

						EditorGUI.ObjectField(fieldPosition, component, dependency.type, true);
					}

					EditorGUI.EndDisabledGroup();
				}
			}

			if (EditorGUI.EndChangeCheck())
			{
				DependencyInvalidator.InvalidateAllDependencies();
			}

			EditorGUI.EndProperty();
		}

		private static class Styles
		{
			public static readonly float sourceWidth = 76;

			public static readonly float spaceBetweenSourceAndField = 2;
		}
	}
}

#endif