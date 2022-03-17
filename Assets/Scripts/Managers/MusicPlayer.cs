using DG.Tweening;
using System.Collections;
using UnityEngine;
using Utils;
using static Facade;

public class MusicPlayer : Singleton<MusicPlayer>
{
	[SerializeField] private AudioSource audioSource;

	private AudioUnit musicOverride;
	private Coroutine updateClip;

	public AudioUnit MusicOverride
	{
		get => musicOverride;
		set
		{
			musicOverride = value;
			if (musicOverride != null)
			{
				FadOut();
			}
		}
	}

	public void SwitchBackToMain()
	{
		if (MusicOverride != null)
		{
			MusicOverride.FadeOut(returnToPool: true);
			MusicOverride = null;
			FadIn();
		}
	}

	public void FadIn()
	{
		audioSource.DOKill();
		audioSource.DOFade(1f, 0.2f);
	}

	public void FadOut()
	{
		audioSource.DOKill();
		audioSource.DOFade(0f, 2f);
	}

	public void TryUpdateClip(AudioClip clip)
	{
		if (clip != null && audioSource.clip != clip)
		{
			if (updateClip != null)
			{
				StopCoroutine(updateClip);
			}
			updateClip = StartCoroutine(TryUpdateClipCore(clip));
		}
	}

	private IEnumerator TryUpdateClipCore(AudioClip clip)
	{
		Tween fadOut = audioSource.DOFade(0f, Settings.audioFadeDuration);
		yield return fadOut.WaitForCompletion();

		audioSource.Stop();
		audioSource.clip = clip;
		audioSource.Play();

		Tween fadIn = audioSource.DOFade(1f, Settings.audioFadeDuration);
		yield return fadIn.WaitForCompletion();
	}
}