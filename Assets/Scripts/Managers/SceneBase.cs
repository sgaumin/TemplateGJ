using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;
using Utils;
using Utils.Dependency;
using static Facade;

public abstract class SceneBase : MonoBehaviour
{
	public const string MASTER_VOLUME = "masterVolume";
	public const string MUSIC_VOLUME = "musicVolume";
	public const string MUSIC_LOWPASS = "musicLowPass";

	public delegate void SceneEventHandler();

	public event SceneEventHandler OnStart;

	public event SceneEventHandler OnEnd;

	public event SceneEventHandler OnPause;

	[Header("Audio")]
	[SerializeField] protected AudioClip music;

	[Header("Local References")]
	[SerializeField] protected Dependency<FadeScreen> _fader;
	[SerializeField] protected Dependency<CinemachineImpulseSource> _impulse;
	[SerializeField] protected Dependency<CinemachineVirtualCamera> _camera;
	[SerializeField] protected Dependency<PostProcessVolume> _volume;

	[Header("Project References")]
	[SerializeField] protected AudioMixer mixer;
	[SerializeField] protected Material transition;

	protected SceneState state;
	protected float masterVolume;
	protected float musicVolume;
	protected float musicLowPass;
	protected float startOrthographicSize;
	protected bool isMusicMuted;
	protected Coroutine loading;
	protected Coroutine inversingColor;
	protected Coroutine whiteScreen;
	protected FloatParameter startVignetteIntensity;
	protected FloatParameter startChromaticAberation;
	protected Tween zooming;
	protected Tween updatingVignette;
	protected Tween updatingChromatic;
	protected Vignette vignette;
	protected ChromaticAberration chromatic;

	public SceneState State
	{
		get => state;
		set
		{
			state = value;

			switch (value)
			{
				case SceneState.Start:
					OnStart?.Invoke();
					break;

				case SceneState.End:
					OnEnd?.Invoke();
					break;

				case SceneState.Pause:
					OnPause?.Invoke();
					break;
			}
		}
	}

	protected FadeScreen fader => _fader.Resolve(this);
	protected CinemachineImpulseSource impulse => _impulse.Resolve(this);
	protected CinemachineVirtualCamera currentCamera => _camera.Resolve(this);
	protected PostProcessVolume volume => _volume.Resolve(this);

	protected virtual void Awake()
	{
		Application.targetFrameRate = 60;

		DOTween.Init();
		DOTween.defaultAutoPlay = AutoPlay.All;

		// Disable screen dimming
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		SetupAllSingleton();
	}

	private void SetupAllSingleton()
	{
		FindObjectsOfType<MonoBehaviour>().OfType<ISingleton>().OrderBy(x => (int)x.GetSingletonPriority()).ForEach(x => x.OnSingletonSetup());
	}

	protected virtual void Start()
	{
		// Post-Processing
		ResetShaders();
		if (volume != null && volume.profile.TryGetSettings<Vignette>(out vignette))
		{
			startVignetteIntensity = vignette.intensity;
		}
		if (volume != null && volume.profile.TryGetSettings<ChromaticAberration>(out chromatic))
		{
			startChromaticAberation = chromatic.intensity;
		}

		// Audio
		if (mixer != null)
		{
			mixer.GetFloat(MASTER_VOLUME, out masterVolume);
			mixer.GetFloat(MUSIC_VOLUME, out musicVolume);
			mixer.GetFloat(MUSIC_LOWPASS, out musicLowPass);
		}
		if (Music != null)
		{
			Music.TryUpdateClip(music);
			Music.SwitchBackToMain();
		}

		// Effects
		if (currentCamera != null)
		{
			startOrthographicSize = currentCamera.m_Lens.OrthographicSize;
		}
		if (fader != null)
		{
			fader.FadeIn();
		}

		State = SceneState.Start;
	}

	protected virtual void Update()
	{
#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_EDITOR
		if (Input.GetButtonDown("Quit"))
		{
			ResetShaders();
			SceneLoader.Quit();
		}
		if (Input.GetButtonDown("Mute"))
		{
			if (isMusicMuted)
			{
				UpdateSceneMasterVolume(1f);
			}
			else
			{
				UpdateSceneMasterVolume(0f);
			}
			isMusicMuted = !isMusicMuted;
		}
		if (Input.GetButtonDown("Fullscreen"))
		{
			Screen.fullScreen = !Screen.fullScreen;
		}
#endif
	}

	#region Post-Processing and Effects

	public void GenerateImpulse()
	{
		impulse.GenerateImpulse();
	}

	private void ResetShaders()
	{
		transition.SetFloat("_isInversed", 0);
		transition.SetFloat("_forceColor", 0);
	}

	public void InverseColor(float duration = 0.05f)
	{
		if (inversingColor != null)
			StopCoroutine(inversingColor);

		inversingColor = StartCoroutine(ApplingShader("_isInversed", duration));
	}

	public void ForceColorScreen(Color color, float duration = 0.05f)
	{
		if (whiteScreen != null)
			StopCoroutine(whiteScreen);

		transition.SetColor("_Color", color);
		whiteScreen = StartCoroutine(ApplingShader("_forceColor", duration));
	}

	public void FreezeTime(float duration = 0.1f)
	{
		Time.timeScale = 0f;
		DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, duration).SetEase(Ease.OutSine).SetUpdate(true);
	}

	public void Zoom(float value, float duration, Ease ease)
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

	private IEnumerator ApplingShader(string parameter, float duration)
	{
		ResetShaders();
		transition.SetFloat(parameter, 1);
		yield return new WaitForSeconds(duration);
		transition.SetFloat(parameter, 0);
	}

	#endregion Post-Processing and Effects

	#region Audio

	public void UpdateSceneMasterVolume(float percentage)
	{
		masterVolume = Mathf.Lerp(-80f, 0f, percentage);
		mixer.SetFloat(MASTER_VOLUME, masterVolume);
	}

	public void UpdateSceneMusicVolume(float percentage)
	{
		musicVolume = Mathf.Lerp(-80f, 0f, percentage);
		mixer.SetFloat(MUSIC_VOLUME, musicVolume);
	}

	public void UpdateSceneMusicLowPass(float percentage)
	{
		musicLowPass = Mathf.Lerp(800f, 22000f, percentage);
		mixer.SetFloat(MUSIC_LOWPASS, musicLowPass);
	}

	#endregion Audio

	#region Scene Loading

	public void ReloadScene()
	{
		if (loading == null)
		{
			loading = StartCoroutine(LoadSceneCore(

			content: () =>
			{
				SceneLoader.Reload();
			}));
		}
	}

	public void LoadNextScene()
	{
		if (loading == null)
		{
			loading = StartCoroutine(LoadSceneCore(

			content: () =>
			{
				SceneLoader.Next();
			}));
		}
	}

	public void LoadMenu()
	{
		if (loading == null)
		{
			loading = StartCoroutine(LoadSceneCore(

			content: () =>
			{
				SceneLoader.Load(Constants.MENU_SCENE);
			}));
		}
	}

	public void LoadSceneByName(string sceneName)
	{
		if (loading == null)
		{
			loading = StartCoroutine(LoadSceneCore(

			content: () =>
			{
				SceneLoader.Load(sceneName);
			}));
		}
	}

	public void LoadSceneTransition(SceneLoading loading)
	{
		if (this.loading == null)
		{
			this.loading = StartCoroutine(LoadSceneCore(

			content: () =>
			{
				SceneLoader.Load(loading);
			}));
		}
	}

	public void Quit()
	{
		if (loading == null)
		{
			loading = StartCoroutine(LoadSceneCore(

			content: () =>
			{
				SceneLoader.Quit();
			}));
		}
	}

	private IEnumerator LoadSceneCore(Action content = null)
	{
		if (inversingColor != null)
		{
			StopCoroutine(inversingColor);
		}

		Time.timeScale = 1f;

		if (fader != null)
		{
			yield return fader.FadeOutCore();
		}

		content?.Invoke();
	}

	#endregion Scene Loading
}