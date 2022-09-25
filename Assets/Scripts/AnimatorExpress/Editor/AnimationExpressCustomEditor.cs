using UnityEditor;
using UnityEngine;

namespace AnimExpress
{
	[CustomEditor(typeof(AnimationExpress))]
	public class AnimationExpressCustomEditor : Editor
	{
		private AnimationExpress context;

		public void OnEnable()
		{
			context = (AnimationExpress)target;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			GUIStyle GuistyleBoxDND = new GUIStyle(GUI.skin.box);
			GuistyleBoxDND.alignment = TextAnchor.MiddleCenter;
			GuistyleBoxDND.fontStyle = FontStyle.Italic;
			GuistyleBoxDND.fontSize = 12;
			GUI.skin.box = GuistyleBoxDND;

			EditorGUILayout.Space(32f);
			Rect myRect = GUILayoutUtility.GetRect(0, 140, GUILayout.ExpandWidth(true));
			GUI.Box(myRect, "Drag and Drop Sprites to this Box!", GuistyleBoxDND);
			if (myRect.Contains(Event.current.mousePosition))
			{
				if (Event.current.type == EventType.DragUpdated)
				{
					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
					Event.current.Use();
				}
				else if (Event.current.type == EventType.DragPerform)
				{
					for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
					{
						if (DragAndDrop.objectReferences[i] is Sprite sprite)
						{
							context.AddFrame(sprite);
						}
					}
					Event.current.Use();
				}
			}
		}
	}
}