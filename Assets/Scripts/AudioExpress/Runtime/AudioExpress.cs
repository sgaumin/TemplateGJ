using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class AudioExpress
{
	[SerializeField] private bool isUsingClips;
	[SerializeField] private AudioClip clip;
	[SerializeField] private AudioClip[] clips;
	[SerializeField] private AudioMixerGroup mixerGroup;
	[SerializeField] private AudioLoopType loopType = AudioLoopType.No;
	[SerializeField, FloatRangeSlider(0f, 10f)] private FloatRange timeBetweenLoop = new FloatRange(1f, 3f);
	[SerializeField] private bool isPitchModified;
	[SerializeField, FloatRangeSlider(-1f, 1f)] private FloatRange pitchMaxVariation = new FloatRange(-0.1f, 0.1f);
	[SerializeField] private AudioStopType autoDestroy = AudioStopType.No;
	[SerializeField, Range(0f, 10f)] private float multiplier = 5f;

	public AudioClip Clip => isUsingClips ? clips.First() : clip;

	public AudioUnit Play(string audioUnitPrefixName = null)
	{
		// Sanity checks
		AudioClip currentClip = isUsingClips ? clips.Random() : clip;
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
		audioSource.pitch = isPitchModified ? 1f + pitchMaxVariation.RandomValue : 1f;
		audioSource.isGoingToStop = autoDestroy != AudioStopType.No;

		if (!audioUnitPrefixName.IsEmpty())
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
}