using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Tools;
using Tools.Utils;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using static Facade;

namespace Tools
{
	public abstract class LevelBase : MonoBehaviour
	{
		public const string LEVEL_MUSIC_VOLUME = "musicVolume";
		public const string LEVEL_MUSIC_LOWPASS = "musicLowPass";

		public delegate void LevelEventHandler();

		public event LevelEventHandler OnStart;
		public event LevelEventHandler OnGameOver;
		public event LevelEventHandler OnPause;

		[Header("Audio")]
		[SerializeField] protected AudioExpress levelMusic;

		[Header("Animations")]
		[SerializeField] protected float levelTransitionDuration = 0.2f;

		[Header("References")]
		[SerializeField] protected Dependency<FadScreen> _fader;
		[SerializeField] protected Dependency<CinemachineImpulseSource> _impulse;
		[SerializeField] protected Dependency<CinemachineVirtualCamera> _camera;
		[SerializeField] protected Dependency<PostProcessVolume> _volume;
		[SerializeField] protected AudioMixer mixer;
		[SerializeField] protected Material transition;

		protected LevelState levelState;
		protected float levelMusicVolume;
		protected float levelMusicLowPass;
		protected float startOrthographicSize;
		protected bool isLevelMusicMuted;
		protected Coroutine loadingLevel;
		protected Coroutine inversingColor;
		protected FloatParameter startVignetteIntensity;
		protected FloatParameter startChromaticAberation;
		protected Tween zooming;
		protected Tween updatingVignette;
		protected Tween updatingChromatic;
		protected Vignette vignette;
		protected ChromaticAberration chromatic;
		protected AudioUnit levelMusicUnit;

		public LevelState LevelState
		{
			get => levelState;
			set
			{
				levelState = value;

				switch (value)
				{
					case LevelState.Play:
						OnStart?.Invoke();
						break;

					case LevelState.GameOver:
						OnGameOver?.Invoke();
						break;

					case LevelState.Pause:
						OnPause?.Invoke();
						break;
				}
			}
		}
		protected FadScreen fader => _fader.Resolve(this);
		protected CinemachineImpulseSource impulse => _impulse.Resolve(this);
		protected CinemachineVirtualCamera currentCamera => _camera.Resolve(this);
		protected PostProcessVolume volume => _volume.Resolve(this);

		protected virtual void Awake()
		{
			Application.targetFrameRate = 60;

			DOTween.Init();
			DOTween.defaultAutoPlay = AutoPlay.All;
		}

		protected void Start()
		{
			// Post-Processing
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

			startOrthographicSize = currentCamera.m_Lens.OrthographicSize;

			// Audio
			mixer.GetFloat(LEVEL_MUSIC_VOLUME, out levelMusicVolume);
			mixer.GetFloat(LEVEL_MUSIC_LOWPASS, out levelMusicLowPass);
			levelMusicUnit = levelMusic.Play();
			levelMusicUnit?.FadIn();

			LevelState = LevelState.Play;
			fader.FadIn();
		}

		protected virtual void Update()
		{
#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_EDITOR
			if (Input.GetButtonDown("Quit"))
			{
				LevelLoader.QuitGame();
			}
			if (Input.GetButtonDown("Mute"))
			{
				if (isLevelMusicMuted)
				{
					UpdateLevelMusicVolume(1f);
				}
				else
				{
					UpdateLevelMusicVolume(0f);
				}
				isLevelMusicMuted = !isLevelMusicMuted;
			}
			if (Input.GetButtonDown("Fullscreen"))
			{
				Screen.fullScreen = !Screen.fullScreen;
			}
#endif
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

		private IEnumerator InversingColor(float duration)
		{
			transition.SetFloat("_isInversed", 1);
			yield return new WaitForSeconds(duration);
			transition.SetFloat("_isInversed", 0);
		}

		public void UpdateLevelMusicVolume(float percentage)
		{
			levelMusicVolume = Mathf.Lerp(-80f, 0f, percentage);
			mixer.SetFloat(LEVEL_MUSIC_VOLUME, levelMusicVolume);
		}

		public void UpdateLevelMusicLowPass(float percentage)
		{
			levelMusicLowPass = Mathf.Lerp(800f, 22000f, percentage);
			mixer.SetFloat(LEVEL_MUSIC_LOWPASS, levelMusicLowPass);
		}

		#region Level Loading

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

			levelMusicUnit?.FadOut(levelTransitionDuration);

			yield return fader.FadOutCore(fadDuration: levelTransitionDuration);
			content?.Invoke();
		}

		#endregion Level Loading

	}
}