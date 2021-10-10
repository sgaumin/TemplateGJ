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
	public abstract class SceneBase : MonoBehaviour
	{
		public const string MASTER_VOLUME = "masterVolume";
		public const string MUSIC_VOLUME = "musicVolume";
		public const string MUSIC_LOWPASS = "musicLowPass";

		public delegate void LevelEventHandler();

		public event LevelEventHandler OnStart;
		public event LevelEventHandler OnGameOver;
		public event LevelEventHandler OnPause;

		[Header("Audio")]
		[SerializeField] protected AudioClip music;

		[Header("References")]
		[SerializeField] protected Dependency<FadeScreen> _fader;
		[SerializeField] protected Dependency<CinemachineImpulseSource> _impulse;
		[SerializeField] protected Dependency<CinemachineVirtualCamera> _camera;
		[SerializeField] protected Dependency<PostProcessVolume> _volume;
		[SerializeField] protected AudioMixer mixer;
		[SerializeField] protected Material transition;

		protected LevelState state;
		protected float masterVolume;
		protected float musicVolume;
		protected float musicLowPass;
		protected float startOrthographicSize;
		protected bool isMusicMuted;
		protected Coroutine loading;
		protected Coroutine inversingColor;
		protected FloatParameter startVignetteIntensity;
		protected FloatParameter startChromaticAberation;
		protected Tween zooming;
		protected Tween updatingVignette;
		protected Tween updatingChromatic;
		protected Vignette vignette;
		protected ChromaticAberration chromatic;

		public LevelState State
		{
			get => state;
			set
			{
				state = value;

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
		protected FadeScreen fader => _fader.Resolve(this);
		protected CinemachineImpulseSource impulse => _impulse.Resolve(this);
		protected CinemachineVirtualCamera currentCamera => _camera.Resolve(this);
		protected PostProcessVolume volume => _volume.Resolve(this);

		protected virtual void Awake()
		{
			Application.targetFrameRate = 60;

			DOTween.Init();
			DOTween.defaultAutoPlay = AutoPlay.All;
		}

		protected virtual void Start()
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
			mixer.GetFloat(MASTER_VOLUME, out masterVolume);
			mixer.GetFloat(MUSIC_VOLUME, out musicVolume);
			mixer.GetFloat(MUSIC_LOWPASS, out musicLowPass);

			Music.TryUpdateClip(music);

			State = LevelState.Play;
			fader.FadeIn();
		}

		protected virtual void Update()
		{
#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_EDITOR
			if (Input.GetButtonDown("Quit"))
			{
				SceneLoader.QuitGame();
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

		#region Level Loading

		public void ReloadLevel()
		{
			if (loading == null)
			{
				loading = StartCoroutine(LoadLevelCore(

				content: () =>
				{
					SceneLoader.Reload();
				}));
			}
		}

		public void LoadNextLevel()
		{
			if (loading == null)
			{
				loading = StartCoroutine(LoadLevelCore(

				content: () =>
				{
					SceneLoader.LoadNext();
				}));
			}
		}

		public void LoadMenu()
		{
			if (loading == null)
			{
				loading = StartCoroutine(LoadLevelCore(

				content: () =>
				{
					SceneLoader.LoadScene(Constants.MENU_SCENE);
				}));
			}
		}

		public void LoadSceneByName(string sceneName)
		{
			if (loading == null)
			{
				loading = StartCoroutine(LoadLevelCore(

				content: () =>
				{
					SceneLoader.LoadScene(sceneName);
				}));
			}
		}

		public void LoadSceneTransition(SceneLoading levelLoading)
		{
			if (loading == null)
			{
				loading = StartCoroutine(LoadLevelCore(

				content: () =>
				{
					SceneLoader.LoadScene(levelLoading);
				}));
			}
		}

		public void QuitGame()
		{
			if (loading == null)
			{
				loading = StartCoroutine(LoadLevelCore(

				content: () =>
				{
					SceneLoader.QuitGame();
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

			yield return fader.FadeOutCore();
			content?.Invoke();
		}

		#endregion Level Loading
	}
}