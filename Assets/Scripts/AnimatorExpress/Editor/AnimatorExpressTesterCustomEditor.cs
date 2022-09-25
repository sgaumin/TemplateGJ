using UnityEditor;
using UnityEngine;

namespace AnimExpress
{
	[CustomEditor(typeof(AnimatorExpressTester))]
	public class AnimatorExpressTesterCustomEditor : Editor
	{
		private AnimatorExpressTester context;

		public void OnEnable()
		{
			context = (AnimatorExpressTester)target;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			EditorGUI.BeginDisabledGroup(!Application.isPlaying);
			if (context is not null && context.Animator is not null && context.Animator.Animations is not null)
			{
				foreach (AnimationExpress item in context.Animator.Animations)
				{
					if (item is null) continue;

					if (GUILayout.Button(item.name))
					{
						context.IsTakingControls = true;
						context.Animator.PlayTesting(item.name);
					}
				}

				GUILayout.Space(16f);
				EditorGUI.BeginDisabledGroup(!context.IsTakingControls);
				if (GUILayout.Button("Release Control"))
				{
					context.IsTakingControls = false;
				}
				EditorGUI.EndDisabledGroup();
			}
			EditorGUI.EndDisabledGroup();
		}
	}
}