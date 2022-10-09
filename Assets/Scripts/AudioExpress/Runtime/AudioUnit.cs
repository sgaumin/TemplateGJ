using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace AudioExpress
{
	public class AudioUnit : MonoBehaviour
	{
		private AudioSource audioSource;

		public event Action OnPlay;

		public bool playOnAwake { get; set; }
		public bool loop { get; set; }
		public AudioLoopType loopType { get; set; }
		public float timeBetweenLoop { get; set; }
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

		private IEnumerator PlayLoop()
		{
			while (true)
			{
				yield return new WaitForSeconds(timeBetweenLoop);
				audioSource.Play();

				if (clips != null)
				{
					audioSource.clip = clips[Random.Range(0, clips.Length - 1)];
				}
			}
		}

		private IEnumerator WaitBeforeReturningToPool()
		{
			yield return new WaitForSeconds(audioSource.clip.length);
			AudioPool.ReturnToPool(this);
		}

		private void OnDisable()
		{
			StopAllCoroutines();
		}
	}
}