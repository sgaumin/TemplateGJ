using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AnimatorExpress
{
	[CreateAssetMenu(fileName = "AnimationExpress", menuName = "AnimationExpress", order = 1)]
	public class AnimationExpress : ScriptableObject
	{
		[SerializeField] private bool isLooping = true;
		[SerializeField] private List<Frame> frames;
		[SerializeField] private List<AnimationExpressEvent> events;

		public bool IsLooping => isLooping;
		public List<Frame> Frames => frames;
		public List<AnimationExpressEvent> Events => events;
		public float TotalDuration => frames.Sum(x => x.Duration);

#if UNITY_EDITOR

		public void AddFrame(Sprite sprite)
		{
			var frame = new Frame(sprite);
			frames.Add(frame);
		}

#endif
	}
}