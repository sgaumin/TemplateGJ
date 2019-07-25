using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class AudioExpress
{
	private enum AutoDestroyTypes
	{
		No,
		AutoDestroyAfterDuration,
		AutoDestroyAfterPlays
	}

	[SerializeField] private bool isUsingClips;
	[SerializeField] private AudioClip clip;
	[SerializeField] private AudioClip[] clips;

	[SerializeField] private bool attached;

	[SerializeField] private bool loop;
	[SerializeField] private bool isPitchModified;
	[SerializeField, Range(0f, 1f)] private float pitchMaxVariation = 0.3f;
	[SerializeField] private bool isStayedWhenLoaddingScene;

	[SerializeField] private AutoDestroyTypes autoDestroy = AutoDestroyTypes.No;
	[SerializeField, Range(0f, 10f)] private float multiplier = 5f;

	private AudioSource audioSource;

	public void Play(GameObject gameObject)
	{
		// Initialization
		if (audioSource is null)
		{
			audioSource = attached ?
				gameObject.AddComponent<AudioSource>() :
				new GameObject("Audio", typeof(AudioSource)).GetComponent<AudioSource>();

			if (isStayedWhenLoaddingScene)
			{
				audioSource.gameObject.AddComponent<DontDestroyOnLoad>();
			}

			// Setup Paramaters
			audioSource.clip = isUsingClips ? clips[Random.Range(0, clips.Length)] : clip;
			audioSource.playOnAwake = false;
			audioSource.loop = loop;
		}

		if (isPitchModified)
		{
			audioSource.pitch = 1f - Random.Range(0f, pitchMaxVariation);
		}

		// Auto Destroy
		if (!attached)
		{
			switch (autoDestroy)
			{
				case AutoDestroyTypes.AutoDestroyAfterDuration:
					audioSource.gameObject.AddComponent<DestroyAfterLoad>().Initialize(multiplier);
					break;
				case AutoDestroyTypes.AutoDestroyAfterPlays:
					audioSource.gameObject.AddComponent<DestroyAfterLoad>().Initialize(audioSource.clip.length * (multiplier - 1));
					break;
			}
		}

		// Play Sound
		audioSource?.Play();
	}
}
