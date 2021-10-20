using DG.Tweening;
using System.Collections;
using Tools;
using UnityEngine;
using static Facade;

public class MusicPlayer : MonoBehaviour
{
	public static MusicPlayer Instance { get; private set; }

	[SerializeField] private Dependency<AudioSource> _audioSource;
	private AudioSource audioSource => _audioSource.Resolve(this);

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