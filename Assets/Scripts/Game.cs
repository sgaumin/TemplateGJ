using Cinemachine;
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

	[Header("Effect")]
	[SerializeField] private Material transition;

	[Header("References")]
	[SerializeField] private Dependency<FadScreen> _fader;
	[SerializeField] private Dependency<CinemachineImpulseSource> _impulse;

	private GameState gameState;
	private Coroutine loadingLevel;
	private float gameMusicVolume;
	private Coroutine inversingColor;

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
	private CinemachineImpulseSource impulse => _impulse.Resolve(this);

	protected override void Awake()
	{
		base.Awake();
		Instance = this;
	}

	protected void Start()
	{
		GameState = GameState.Play;
		transition.SetFloat("_isInversed", 0);
		fader.FadIn();

		mixer.GetFloat(GAME_MUSIC_VOLUME, out gameMusicVolume);

		gameMusic.Play();
	}

	protected override void Update()
	{
		base.Update();
	}

	public void GenerateImpulse()
	{
		impulse.GenerateImpulse();
	}

	public void InverseColor(float duration)
	{
		if (inversingColor != null)
		{
			StopCoroutine(inversingColor);
		}

		transition.SetFloat("_isInversed", 0);
		inversingColor = StartCoroutine(InversingColor(duration));
	}

	private IEnumerator InversingColor(float duration)
	{
		transition.SetFloat("_isInversed", 1);
		yield return new WaitForSeconds(duration);
		transition.SetFloat("_isInversed", 0);
	}

	public void UpdateGameMusicVolume(float percentage)
	{
		gameMusicVolume = Mathf.Lerp(gameMusicVolumeLimits.Min, gameMusicVolumeLimits.Max, percentage);
		mixer.SetFloat(GAME_MUSIC_VOLUME, gameMusicVolume);
	}

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
		if (inversingColor != null)
		{
			StopCoroutine(inversingColor);
		}
		Time.timeScale = 1f;

		yield return fader.FadOutCore();
		content?.Invoke();
	}
}