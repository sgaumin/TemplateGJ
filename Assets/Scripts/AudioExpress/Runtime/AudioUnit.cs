using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioUnit : MonoBehaviour
{
	private AudioSource audioSource;

	public event Action OnPlay;

	public bool playOnAwake { get; set; }
	public bool loop { get; set; }
	public AudioLoopType loopType { get; set; }
	public FloatRange timeBetweenLoop { get; set; }
	public AudioMixerGroup outputAudioMixerGroup { get; set; }
	public AudioClip clip { get; set; }
	public AudioClip[] clips { get; set; }
	public float pitch { get; set; }

	public float volume
	{ get => audioSource.volume; set { audioSource.volume = value; } }
	public bool isGoingToStop { get; set; }
	public float duration { get; set; }

	public void Play()
	{
		audioSource = GetComponent<AudioSource>();

		audioSource.playOnAwake = playOnAwake;
		audioSource.loop = loop;
		audioSource.outputAudioMixerGroup = outputAudioMixerGroup;
		audioSource.clip = clip;
		audioSource.pitch = pitch;
		audioSource.volume = 1f;

		OnPlay?.Invoke();

		if (loopType == AudioLoopType.Manuel)
		{
			StartCoroutine(PlayLoop());
		}
		else
		{
			audioSource.Play();
			if (!loop)
			{
				StartCoroutine(WaitBeforeReturningToPool());
			}
		}
	}

	public void StopAndReturnToPool()
	{
		audioSource.Stop();
		AudioPool.ReturnToPool(this);
	}

	public void FadeIn(float duration = 1f)
	{
		audioSource.volume = 0f;
		audioSource.DOFade(1f, duration).Play();
	}

	public void FadeOut(float duration = 1f, bool returnToPool = false)
	{
		audioSource.DOFade(0f, duration).Play();
		if (returnToPool)
		{
			StartCoroutine(ReturnToPoolAfterDuration(duration));
		}
	}

	public void SetPitch(float value, float duration = 1f, bool isIgnoringTime = false)
	{
		audioSource.DOPitch(0.5f, duration).SetUpdate(UpdateType.Normal, isIgnoringTime).Play();
	}

	public void ResetPitch(float duration = 1f, bool isIgnoringTime = false)
	{
		audioSource.DOPitch(pitch, duration).SetUpdate(UpdateType.Normal, isIgnoringTime).Play();
	}

	public void SetVolume(float value, float duration = 1f, bool isIgnoringTime = false)
	{
		DOTween.To(() => audioSource.volume, x => audioSource.volume = x, value, duration).SetUpdate(UpdateType.Normal, isIgnoringTime).Play();
	}

	public void ResetVolume(float duration = 1f, bool isIgnoringTime = false)
	{
		DOTween.To(() => audioSource.volume, x => audioSource.volume = x, 1f, duration).SetUpdate(UpdateType.Normal, isIgnoringTime).Play();
	}

	private IEnumerator PlayLoop()
	{
		while (true)
		{
			yield return new WaitForSeconds(timeBetweenLoop.RandomValue);
			audioSource.Play();

			if (clips != null)
			{
				audioSource.clip = clips.Random();
			}
		}
	}

	private IEnumerator WaitBeforeReturningToPool()
	{
		yield return new WaitForSeconds(audioSource.clip.length);
		AudioPool.ReturnToPool(this);
	}

	private IEnumerator ReturnToPoolAfterDuration(float duration)
	{
		yield return new WaitForSeconds(duration);
		AudioPool.ReturnToPool(this);
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}
}