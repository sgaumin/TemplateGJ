using AudioExpress;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using static Facade;

public class MusicPlayer : MonoBehaviour
{
	public const string MASTER_VOLUME = "masterVolume";
	public const string MUSIC_VOLUME = "musicVolume";
	public const string MUSIC_LOWPASS = "musicLowPass";

	[Header("References")]
	[SerializeField] protected AudioMixer mixer;
	[SerializeField] private AudioSource audioSource;

	private AudioUnit _musicOverride;
	private Coroutine _updateClip;
	private float _masterVolume;
	private float _musicVolume;
	private float _musicLowPass;

	public AudioUnit MusicOverride
	{
		get => _musicOverride;
		set
		{
			_musicOverride = value;
			if (_musicOverride != null)
			{
				FadOut();
			}
		}
	}
	public bool IsMuted => _masterVolume == -80f;

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
			if (_updateClip != null)
			{
				StopCoroutine(_updateClip);
			}
			_updateClip = StartCoroutine(TryUpdateClipCore(clip));
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

	public void UpdateSceneMasterVolume(float percentage)
	{
		_masterVolume = Mathf.Lerp(-80f, 0f, percentage);
		mixer.SetFloat(MASTER_VOLUME, _masterVolume);
	}

	public void UpdateSceneMusicVolume(float percentage)
	{
		_musicVolume = Mathf.Lerp(-80f, 0f, percentage);
		mixer.SetFloat(MUSIC_VOLUME, _musicVolume);
	}

	public void UpdateSceneMusicLowPass(float percentage)
	{
		_musicLowPass = Mathf.Lerp(800f, 22000f, percentage);
		mixer.SetFloat(MUSIC_LOWPASS, _musicLowPass);
	}

	public void Mute()
	{
		if (IsMuted)
		{
			UpdateSceneMasterVolume(1f);
		}
		else
		{
			UpdateSceneMasterVolume(0f);
		}
	}
}