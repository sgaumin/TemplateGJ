using DG.Tweening;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
	public static GameSystem Instance { get; private set; }

	public GameStates GameState { get; private set; } = GameStates.Play;

	protected void Awake()
	{
		Instance = this;
		DOTween.Init();
		DOTween.defaultAutoPlay = AutoPlay.None;
		DOTween.defaultAutoKill = false;
	}

	protected void Update()
	{
#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX

		if (Input.GetButtonDown("Quit"))
		{
			LevelLoader.Instance.QuitGame();
		}
#endif
	}
}