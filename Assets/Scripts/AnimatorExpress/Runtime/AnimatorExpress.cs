using System.Collections;
using System.Collections.Generic;
using Utils.Dependency;
using UnityEngine;
using Utils;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimatorExpress : MonoBehaviour
{
	[SerializeField] private List<AnimationExpress> animations = new List<AnimationExpress>();
	[SerializeField] protected Dependency<SpriteRenderer> _spriteRenderer;

	private SpriteRenderer spriteRenderer => _spriteRenderer.Resolve(this);

	private bool hasBeenInitialized;
	private int currentIndex;
	private Coroutine animationRoutine;
	private AnimationExpress currentAnimation;
	private Dictionary<string, AnimationExpress> declaredAnimations;

	private void OnValidate()
	{
		if (animations.Count > 0)
		{
			spriteRenderer.sprite = animations[0]?.Frames[0]?.sprite;
		}
	}

	private void Start()
	{
		PlayDefault();
	}

	private void Init()
	{
		hasBeenInitialized = true;
		declaredAnimations = new Dictionary<string, AnimationExpress>(animations.Count);
		foreach (AnimationExpress animation in animations)
		{
			declaredAnimations.Add(animation.name, animation);
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
		currentIndex = 0;
		Frame[] frames = currentAnimation.Frames;
		while (true)
		{
			if (currentIndex >= frames.Length)
			{
				if (currentAnimation.IsLooping)
				{
					currentIndex = 0;
				}
				else
				{
					break;
				}
			}

			Frame currentFrame = frames[currentIndex++];
			spriteRenderer.sprite = currentFrame.sprite;
			yield return new WaitForSeconds(currentFrame.duration);
		}

		PlayDefault();
	}
}