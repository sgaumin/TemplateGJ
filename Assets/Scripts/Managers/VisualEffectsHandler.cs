using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;
using Utils;
using Utils.Dependency;
using static Facade;

public class VisualEffectsHandler : MonoBehaviour
{
	[SerializeField] private FadeScreen fader;
	[SerializeField] private CinemachineImpulseSource impulse;
	[SerializeField] private CinemachineVirtualCamera currentCamera;
	[SerializeField] private PostProcessVolume volume;
	[SerializeField] private Material transition;

	private float _startOrthographicSize;
	private Coroutine _inversingColor;
	private Coroutine _coloringScreen;
	private FloatParameter _startVignetteIntensity;
	private FloatParameter _startChromaticAberation;
	private Tween _zooming;
	private Tween _updatingVignette;
	private Tween _updatingChromatic;
	private Vignette _vignette;
	private ChromaticAberration _chromatic;
	private Transform _defaultCameraTarget;
	private Tweener _timerScaler;

	private void Start()
	{
		// Post-Processing
		ResetShaders();
		if (volume != null && volume.profile.TryGetSettings<Vignette>(out _vignette))
		{
			_startVignetteIntensity = _vignette.intensity;
		}
		if (volume != null && volume.profile.TryGetSettings<ChromaticAberration>(out _chromatic))
		{
			_startChromaticAberation = _chromatic.intensity;
		}

		// Effects
		if (currentCamera != null)
		{
			_startOrthographicSize = currentCamera.m_Lens.OrthographicSize;
		}
		if (fader != null)
		{
			fader.FadeIn();
		}
	}

	public void GenerateImpulse()
	{
		impulse.GenerateImpulse();
	}

	public void ResetShaders()
	{
		transition.SetFloat("_isInversed", 0);
		transition.SetFloat("_forceColor", 0);

		this.TryStopCoroutine(ref _inversingColor);
		this.TryStopCoroutine(ref _coloringScreen);
	}

	public void InverseColor(float duration = 0.05f)
	{
		ResetShaders();
		this.TryStartCoroutine(ApplingShader("_isInversed", duration), ref _inversingColor);
	}

	public void ForceColorScreen(Color color, float duration = 0.05f)
	{
		ResetShaders();
		transition.SetColor("_Color", color);
		this.TryStartCoroutine(ApplingShader("_forceColor", duration), ref _coloringScreen);
	}

	public void FreezeTime(float duration = 0.1f)
	{
		_timerScaler.Kill();
		Time.timeScale = 0f;
		_timerScaler = DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, duration).SetEase(Ease.OutSine).SetUpdate(true);
	}

	public void BoostTime(float startValue, float duration = 0.1f)
	{
		_timerScaler.Kill();
		Time.timeScale = startValue;
		_timerScaler = DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, duration).SetEase(Ease.OutSine).SetUpdate(true);
	}

	public void SetCameraTarget(Transform target)
	{
		if (_defaultCameraTarget == null)
			_defaultCameraTarget = currentCamera.m_Follow;

		currentCamera.m_Follow = target;
	}

	public void ResetCameraTarget()
	{
		currentCamera.m_Follow = _defaultCameraTarget;
	}

	public void Zoom(float value, float duration = 1f, Ease ease = Ease.OutSine)
	{
		_zooming?.Kill();
		currentCamera.m_Lens.OrthographicSize = _startOrthographicSize;
		_zooming = DOTween.To(() => currentCamera.m_Lens.OrthographicSize, x => currentCamera.m_Lens.OrthographicSize = x, value, duration).SetEase(ease);
	}

	public void ResetZoom(float duration = 1f, Ease ease = Ease.OutSine)
	{
		_zooming?.Kill();
		_zooming = DOTween.To(() => currentCamera.m_Lens.OrthographicSize, x => currentCamera.m_Lens.OrthographicSize = x, _startOrthographicSize, duration).SetEase(ease);
	}

	public void SetVignette(float value, float duration, Ease ease)
	{
		if (_vignette == null)
		{
			Debug.LogWarning("Vignette effect has not been initialized. is PostProcessVolume component missing?");
			return;
		}

		_updatingVignette?.Kill();
		_vignette.intensity = _startVignetteIntensity;
		_updatingVignette = DOTween.To(() => _vignette.intensity.value, x => _vignette.intensity.value = x, value, duration).SetEase(ease).SetLoops(2, LoopType.Yoyo);
	}

	public void SetChromaticAberation(float value, float duration, Ease ease)
	{
		if (_chromatic == null)
		{
			Debug.LogWarning("Chromatic effect has not been initialized. is PostProcessVolume component missing?");
			return;
		}

		_updatingChromatic?.Kill();
		_chromatic.intensity = _startChromaticAberation;
		_updatingChromatic = DOTween.To(() => _chromatic.intensity.value, x => _chromatic.intensity.value = x, value, duration).SetEase(ease).SetLoops(2, LoopType.Yoyo);
	}

	private IEnumerator ApplingShader(string parameter, float duration)
	{
		transition.SetFloat(parameter, 1);
		yield return new WaitForSeconds(duration);
		transition.SetFloat(parameter, 0);
	}
}