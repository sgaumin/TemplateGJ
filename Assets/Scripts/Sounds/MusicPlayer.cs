using DG.Tweening;
using Tools;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
	public static MusicPlayer Instance { get; private set; }

	[SerializeField] private Dependency<AudioSource> _audioSource;
	private AudioSource audioSource => _audioSource.Resolve(this);

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
}