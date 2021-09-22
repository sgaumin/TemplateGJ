using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using Tools;
using Tools.Utils;
using UnityEngine;
using UnityEngine.Audio;

public class Game : GameSystem
{
	public const string GAME_MUSIC_VOLUME = "musicVolume";
	public const float MIN_MUSIC_VOLUME = -60f;

	public delegate void GameEventHandler();

	public event GameEventHandler OnStart;
	public event GameEventHandler OnGameOver;
	public event GameEventHandler OnPause;

	public static Game Instance { get; private set; }

	[Header("Audio")]
	[SerializeField] private AudioExpress gameMusic;

	[Header("References")]
	[SerializeField] private Dependency<FadScreen> _fader;
	[SerializeField] private Dependency<CinemachineImpulseSource> _impulse;
	[SerializeField] private Dependency<CinemachineVirtualCamera> _camera;
	[SerializeField] private AudioMixer mixer;
	[SerializeField] private Material transition;

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
	private CinemachineVirtualCamera currentCamera => _camera.Resolve(this);

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

	public void Zoom(float value, float duration, Ease ease)
	{
		DOTween.To(() => currentCamera.m_Lens.OrthographicSize, x => currentCamera.m_Lens.OrthographicSize = x, value, duration).SetEase(ease).SetLoops(2, LoopType.Yoyo);
	}

	private IEnumerator InversingColor(float duration)
	{
		transition.SetFloat("_isInversed", 1);
		yield return new WaitForSeconds(duration);
		transition.SetFloat("_isInversed", 0);
	}

	public void UpdateGameMusicVolume(float percentage)
	{
		gameMusicVolume = Mathf.Lerp(MIN_MUSIC_VOLUME, 0f, percentage);
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