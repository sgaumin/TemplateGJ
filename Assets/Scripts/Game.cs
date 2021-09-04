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

	public static Game Instance { get; private set; }

	[Header("Audio")]
	[SerializeField] private AudioMixer mixer;
	[SerializeField] private AudioExpress gameMusic;
	[SerializeField, FloatRangeSlider(-80f, 10f)] private FloatRange gameMusicVolumeLimits = new FloatRange(-60f, 0f);

	[Header("References")]
	[SerializeField] private Dependency<FadScreen> _fader;

	private GameState gameState;
	private Coroutine loadingLevel;
	private float gameMusicVolume;

	public GameState GameState
	{
		get => gameState;
		set
		{
			gameState = value;

			switch (value)
			{
				case GameState.Play:
					OnStart?.Invoke();
					break;

				case GameState.GameOver:
					OnGameOver?.Invoke();
					break;

				case GameState.Pause:
					OnPause?.Invoke();
					break;
			}
		}
	}
	private FadScreen fader => _fader.Resolve(this);

	#region Unity Callbacks

	protected override void Awake()
	{
		base.Awake();
		Instance = this;
	}

	protected void Start()
	{
		GameState = GameState.Play;
		fader.FadIn();

		mixer.GetFloat(GAME_MUSIC_VOLUME, out gameMusicVolume);

		gameMusic.Play();
	}

	protected override void Update()
	{
		base.Update();
	}

	#endregion Unity Callbacks

	#region Audio

	public void UpdateGameMusicVolume(float percentage)
	{
		gameMusicVolume = Mathf.Lerp(gameMusicVolumeLimits.Min, gameMusicVolumeLimits.Max, percentage);
		mixer.SetFloat(GAME_MUSIC_VOLUME, gameMusicVolume);
	}

	#endregion Audio

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

	#endregion Level Loading Methods
}