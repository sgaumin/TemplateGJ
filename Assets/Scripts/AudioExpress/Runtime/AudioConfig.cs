using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace AudioExpress
{
	[Serializable]
	public class AudioConfig
	{
		[SerializeField] private bool isUsingClips;
		[SerializeField] private AudioClip clip;
		[SerializeField] private AudioClip[] clips;
		[SerializeField] private AudioMixerGroup mixerGroup;
		[SerializeField] private AudioLoopType loopType = AudioLoopType.No;
		[SerializeField] private float timeBetweenLoop = 0f;
		[SerializeField] private PitchVariation pitchVariation;
		[SerializeField] private AudioStopType autoDestroy = AudioStopType.No;
		[SerializeField, Range(0f, 10f)] private float multiplier = 5f;

		public AudioClip Clip => isUsingClips ? clips.First() : clip;

		public AudioUnit Play(string audioUnitPrefixName = null)
		{
			// Sanity checks
			AudioClip currentClip = isUsingClips ? clips[Random.Range(0, clips.Length - 1)] : clip;
			if (currentClip == null)
			{
				return null;
			}

			// Initialization
			AudioUnit audioSource = AudioPool.GetFromPool();

			// Setup Parameters
			audioSource.playOnAwake = false;
			audioSource.loop = loopType == AudioLoopType.Normal;
			audioSource.loopType = loopType;
			audioSource.timeBetweenLoop = timeBetweenLoop;
			audioSource.outputAudioMixerGroup = mixerGroup;

			audioSource.clip = currentClip;
			audioSource.clips = isUsingClips ? clips : null;
			audioSource.pitch = SetPitch(pitchVariation);
			audioSource.isGoingToStop = autoDestroy != AudioStopType.No;

			if (!string.IsNullOrEmpty(audioUnitPrefixName))
			{
				audioSource.gameObject.name = audioUnitPrefixName;
			}

			audioSource.gameObject.name += audioSource.clip.name;

			// Auto Destroy
			switch (autoDestroy)
			{
				case AudioStopType.StopAfterDuration:
					audioSource.duration = multiplier;
					break;

				case AudioStopType.StopAfterPlays:
					audioSource.duration = audioSource.clip.length * (multiplier - 1);
					break;
			}

			// Play Sound
			audioSource.Play();

			return audioSource;
		}

		private float SetPitch(PitchVariation variation)
		{
			switch (variation)
			{
				case PitchVariation.VerySmall:
					return Random.Range(0.95f, 1.05f);

				case PitchVariation.Small:
					return Random.Range(0.9f, 1.1f);

				case PitchVariation.Medium:
					return Random.Range(0.75f, 1.25f);

				case PitchVariation.Large:
					return Random.Range(0.5f, 1.5f);
			}
			return 1f;
		}
	}
}