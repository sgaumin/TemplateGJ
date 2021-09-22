using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using Tools;
using Tools.Utils;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;

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
	[SerializeField] private Dependency<PostProcessVolume> _volume;
	[SerializeField] private AudioMixer mixer;
	[SerializeField] private Material transition;

	private GameState gameState;
	private Coroutine loadingLevel;
	private float gameMusicVolume;
	private Coroutine inversingColor;
	private float startOrthographicSize;
	private FloatParameter startVignetteIntensity;
	private FloatParameter startChromaticAberation;
	private Tween zooming;
	private Tween updatingVignette;
	private Tween updatingChromatic;
	private Vignette vignette;
	private ChromaticAberration chromatic;

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
	private PostProcessVolume volume => _volume.Resolve(this);

	protected override void Awake()
	{
		base.Awake();
		Instance = this;
	}

	protected void Start()
	{
		// Post Processing
		transition.SetFloat("_isInversed", 0);
		volume.profile.TryGetSettings<Vignette>(out vignette);
		if (volume != null)
		{
			startVignetteIntensity = vignette.intensity;
		}

		volume.profile.TryGetSettings<ChromaticAberration>(out chromatic);
		if (volume != null)
		{
			startChromaticAberation = chromatic.intensity;
		}

		mixer.GetFloat(GAME_MUSIC_VOLUME, out gameMusicVolume);
		startOrthographicSize = currentCamera.m_Lens.OrthographicSize;

		fader.FadIn();
		gameMusic.Play();

		GameState = GameState.Play;
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

	public void SetZoom(float value, float duration, Ease ease)
	{
		zooming?.Kill();
		currentCamera.m_Lens.OrthographicSize = startOrthographicSize;
		zooming = DOTween.To(() => currentCamera.m_Lens.OrthographicSize, x => currentCamera.m_Lens.OrthographicSize = x, value, duration).SetEase(ease).SetLoops(2, LoopType.Yoyo);
	}

	public void SetVignette(float value, float duration, Ease ease)
	{
		if (vignette == null)
		{
			Debug.LogWarning("Vignette effect has not been initialized. is PostProcessVolume component missing?");
			return;
		}

		updatingVignette?.Kill();
		vignette.intensity = startVignetteIntensity;
		updatingVignette = DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, value, duration).SetEase(ease).SetLoops(2, LoopType.Yoyo);
	}

	public void SetChromaticAberation(float value, float duration, Ease ease)
	{
		if (chromatic == null)
		{
			Debug.LogWarning("Chromatic effect has not been initialized. is PostProcessVolume component missing?");
			return;
		}

		updatingChromatic?.Kill();
		chromatic.intensity = startChromaticAberation;
		updatingChromatic = DOTween.To(() => chromatic.intensity.value, x => chromatic.intensity.value = x, value, duration).SetEase(ease).SetLoops(2, LoopType.Yoyo);
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

	public void LoadSceneTransition(LevelLoading levelLoading)
	{
		if (loadingLevel == null)
		{
			loadingLevel = StartCoroutine(LoadLevelCore(

			content: () =>
			{
				LevelLoader.OnLoadLevel(levelLoading);
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