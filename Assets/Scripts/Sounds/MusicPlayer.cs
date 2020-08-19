using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
	public static MusicPlayer Instance { get; private set; }

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
