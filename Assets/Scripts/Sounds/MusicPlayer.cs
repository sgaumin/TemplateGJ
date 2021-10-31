using DG.Tweening;
using System.Collections;
using Tools;
using Tools.Utils;
using UnityEngine;
using static Facade;

public class MusicPlayer : MonoBehaviour
{
	public static MusicPlayer Instance { get; private set; }

	[SerializeField] private Dependency<AudioSource> _audioSource;

	private AudioSource audioSource => _audioSource.Resolve(this);
	public AudioUnit MusicOverride { get; set; }

	private Coroutine updateClip;

	protected void Awake()
	{
		if (Instance == null)
		{
			DontDestroyOnLoad(gameObject);
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
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
		if (audioSource.clip != clip)
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