using System.Collections;
using System.Collections.Generic;
using Utils.Dependency;
using UnityEngine;

namespace Utils
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class AnimatorExpress : MonoBehaviour
	{
		[System.Serializable]
		private struct Frame
		{
			public Sprite frame;
			public float duration;
		}

		[SerializeField] private bool playOnStart;
		[SerializeField] private List<Frame> frames = new List<Frame>();
		[SerializeField] protected Dependency<SpriteRenderer> _spriteRenderer;

		private SpriteRenderer spriteRenderer => _spriteRenderer.Resolve(this);

		private int currentIndex;
		private Coroutine animationRoutine;

		private void Start()
		{
			if (playOnStart)
			{
				Play();
			}
		}

		public void Play()
		{
			currentIndex = 0;
			this.TryStartCoroutine(PlayCore(), ref animationRoutine);
		}

		private IEnumerator PlayCore()
		{
			while (true)
			{
				if (currentIndex >= frames.Count) currentIndex = 0;

				var frame = frames[currentIndex++];
				spriteRenderer.sprite = frame.frame;
				yield return new WaitForSeconds(frame.duration);
			}
		}

		public void Stop()
		{
			this.TryStopCoroutine(ref animationRoutine);
		}
	}
}
