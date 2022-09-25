using UnityEditor;
using UnityEngine;

namespace AnimatorExpress
{
	[CustomPropertyDrawer(typeof(AnimationExpressEvent))]
	public class AnimationExpressEventPropertyDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var name = property.FindPropertyRelative("name");
			var triggerTime = property.FindPropertyRelative("triggerTime");
			Rect p1 = position;
			p1.width = position.width * 0.4f;
			Rect p2 = position;
			p2.width = position.width * 0.5f;
			p2.x += position.width * 0.45f;
			EditorGUI.PropertyField(p1, name, GUIContent.none);
			EditorGUI.PropertyField(p2, triggerTime, GUIContent.none);
		}
	}
}