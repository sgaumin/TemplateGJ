using System;
using System.Collections;
using Tools;
using Tools.Utils;
using UnityEngine;
using UnityEngine.Audio;

public class Game : GameSystem
{
	public const string GAME_MUSIC_VOLUME = "musicVolume";

	public delegate void GameEventHandler();
	public event GameEventHandler OnStart;
	public event GameEventHandler OnGameOver;
	public event GameEventHandler OnPause;

	[Header("Audio")]
	[SerializeField] private AudioMixer mixer;
	[Space]
	[SerializeField] private AudioExpress gameMusic;
	[SerializeField, FloatRangeSlider(-80f, 10f)] private FloatRange gameMusicVolumeLimits = new FloatRange(-60f, 0f);

	[Header("References")]
	[SerializeField] private FadScreen fader;

	private GameStates gameState;
	private Coroutine loadingLevel;
	private float gameMusicVolume;

	public GameStates GameState
	{
		get => gameState;
		set
		{
			gameState = value;

			switch (value)
			{
				case GameStates.Play:
					OnStart?.Invoke();
					break;

				case GameStates.GameOver:
					OnGameOver?.Invoke();
					break;

				case GameStates.Pause:
					OnPause?.Invoke();
					break;
			}
		}
	}

	#region Unity Callbacks
	protected override void Awake()
	{
		base.Awake();
	}

	protected void Start()
	{
		GameState = GameStates.Play;
		fader.FadIn();

		mixer.GetFloat(GAME_MUSIC_VOLUME, out gameMusicVolume);

		gameMusic.Play();
	}

	protected override void Update()
	{
		base.Update();
	}
	#endregion

	#region Audio
	public void UpdateGameMusicVolume(float percentage)
	{
		gameMusicVolume = Mathf.Lerp(gameMusicVolumeLimits.Min, gameMusicVolumeLimits.Max, percentage);
		mixer.SetFloat(GAME_MUSIC_VOLUME, gameMusicVolume);
	}
	#endregion

	#region Level Loading Methods
	public void ReloadLevel()
	{
		if (loadingLevel == null)
		{
			loadingLevel = StartCoroutine(LoadLevelCore(

			content: () =>
			{
				LevelLoader.ReloadLevel();
			}));
		}
	}

	public void LoadNextLevel()
	{
		if (loadingLevel == null)
		{
			loadingLevel = StartCoroutine(LoadLevelCore(

			content: () =>
			{
				LevelLoader.LoadNextLevel();
			}));
		}
	}

	public void LoadMenu()
	{
		if (loadingLevel == null)
		{
			loadingLevel = StartCoroutine(LoadLevelCore(

			content: () =>
			{
				LevelLoader.LoadLevelByName(Constants.MENU_SCENE);
			}));
		}
	}

	public void LoadSceneByName(string sceneName)
	{
		if (loadingLevel == null)
		{
			loadingLevel = StartCoroutine(LoadLevelCore(

			content: () =>
			{
				LevelLoader.LoadLevelByName(sceneName);
			}));
		}
	}

	public void QuitGame()
	{
		if (loadingLevel == null)
		{
			loadingLevel = StartCoroutine(LoadLevelCore(

			content: () =>
			{
				LevelLoader.QuitGame();
			}));
		}
	}

	private IEnumerator LoadLevelCore(Action content = null)
	{
		Time.timeScale = 1f;
		yield return fader.FadOutCore();
		content?.Invoke();
	}
	#endregion
}