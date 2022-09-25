using System.Collections;
using System.Collections.Generic;
using Utils.Dependency;
using UnityEngine;
using Utils;
using System;

namespace AnimatorExpress
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class AnimatorExpress : MonoBehaviour
	{
		[SerializeField] private List<AnimationExpress> animations = new List<AnimationExpress>();
		[SerializeField] protected Dependency<SpriteRenderer> _spriteRenderer;

		private SpriteRenderer spriteRenderer => _spriteRenderer.Resolve(this);

		private bool hasBeenInitialized;
		private Coroutine animationRoutine;
		private AnimationExpress currentAnimation;
		private Dictionary<string, AnimationExpress> declaredAnimations;
		private Dictionary<string, Dictionary<string, AnimationExpressEvent>> declaredAnimationEvents;

		private void OnValidate()
		{
			try
			{
				spriteRenderer.sprite = animations[0].Frames[0].Sprite;
			}
			catch (System.Exception) { }
		}

		private void Awake()
		{
			Init();
		}

		private void Start()
		{
			PlayDefault();
		}

		private void Init()
		{
			hasBeenInitialized = true;
			declaredAnimations = new Dictionary<string, AnimationExpress>(animations.Count);
			declaredAnimationEvents = new Dictionary<string, Dictionary<string, AnimationExpressEvent>>();
			foreach (AnimationExpress animation in animations)
			{
				declaredAnimations.Add(animation.name, animation);

				var d = new Dictionary<string, AnimationExpressEvent>();
				foreach (AnimationExpressEvent e in animation.Events)
				{
					d.Add(e.Name, e);
				}
				declaredAnimationEvents.Add(animation.name, d);
			}
		}

		private void PlayDefault()
		{
			if (!hasBeenInitialized) Init();

			currentAnimation = animations[0]; // First animation is default
			this.TryStartCoroutine(PlayCore(), ref animationRoutine);
		}

		public void Play(string animationKey = "")
		{
			if (!hasBeenInitialized) Init();

			if (string.IsNullOrEmpty(animationKey))
			{
				PlayDefault();
			}
			else if (declaredAnimations.TryGetValue(animationKey, out AnimationExpress animation))
			{
				currentAnimation = animation;
			}
			else
			{
				Debug.LogError($"Animation {animationKey} not found for {gameObject.name}");
			}

			this.TryStartCoroutine(PlayCore(), ref animationRoutine);
		}

		private IEnumerator PlayCore()
		{
			int currentIndex = 0;
			float currentDuration = 0f;
			float currentStartTime = 0f;
			List<Frame> frames = currentAnimation.Frames;
			List<AnimationEventChecker> events = new List<AnimationEventChecker>();
			foreach (var e in currentAnimation.Events)
			{
				var a = new AnimationEventChecker(e);
				a.SetupTrigger(currentAnimation);
				events.Add(a);
			}

			while (true)
			{
				if (Time.time >= currentDuration)
				{
					if (currentIndex >= frames.Count)
					{
						if (currentAnimation.IsLooping)
						{
							currentIndex = 0;
							currentStartTime = Time.time;
						}
						else
						{
							break;
						}

						events.ForEach(x => x.hasBeenTriggered = false);
					}

					Frame currentFrame = frames[currentIndex++];
					spriteRenderer.sprite = currentFrame.Sprite;
					currentDuration = Time.time + currentFrame.Duration;
				}

				foreach (var e in events)
				{
					float triggerTime = e.animationEvent.TriggerTime + currentStartTime;
					if (triggerTime <= Time.time && !e.hasBeenTriggered)
					{
						e.hasBeenTriggered = true;
						e.animationEvent.Invoke();
					}
				}

				yield return null;
			}

			PlayDefault();
		}

		public void AddListener(string animationName, string eventName, Action action)
		{
			if (declaredAnimationEvents.TryGetValue(animationName, out var events))
			{
				if (events.TryGetValue(eventName, out var e))
				{
					e.AddListener(action);
				}
				else
				{
					Debug.LogError($"Animation event {eventName} not found for {gameObject.name}");
				}
			}
			else
			{
				Debug.LogError($"Animation {animationName} not found for {gameObject.name}");
			}
		}

		public void RemoveListener(string animationName, string eventName, Action action)
		{
			if (declaredAnimationEvents.TryGetValue(animationName, out var events))
			{
				if (events.TryGetValue(eventName, out var e))
				{
					e.RemoveListener(action);
				}
			}
		}
	}
}