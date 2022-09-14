using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Utils.Dependency;
using static Facade;

public class FadeScreen : MonoBehaviour
{
	[SerializeField] private Dependency<Image> __image;

	private Image _image => __image.Resolve(this);

	private Color _defaultFadeInColorTarget = Color.black;
	private Color _defaultFadeOutColorTarget = Color.black.WithAlpha(0);

	public void FadeOut(float fadeDuration)
	{
		_image.color = _defaultFadeOutColorTarget;
		_image.DOFade(1f, fadeDuration);
	}

	public void FadeOut()
	{
		_image.color = _defaultFadeOutColorTarget;
		_image.DOFade(1f, Settings.sceneFadeDuration);
	}

	public IEnumerator FadeOutCore(float fadeDuration)
	{
		_image.color = _defaultFadeOutColorTarget;
		Tweener fade = _image.DOFade(1f, fadeDuration);
		yield return fade.WaitForCompletion();
	}

	public IEnumerator FadeOutCore()
	{
		_image.color = _defaultFadeOutColorTarget;
		Tweener fade = _image.DOFade(1f, Settings.sceneFadeDuration);
		yield return fade.WaitForCompletion();
	}

	public void FadeIn(float fadeDuration)
	{
		_image.color = _defaultFadeInColorTarget;
		_image.DOFade(0f, fadeDuration);
	}

	public void FadeIn()
	{
		_image.color = _defaultFadeInColorTarget;
		_image.DOFade(0f, Settings.sceneFadeDuration);
	}

	public IEnumerator FadeInCore(float fadeDuration)
	{
		_image.color = _defaultFadeInColorTarget;
		Tweener fade = _image.DOFade(0f, fadeDuration);
		yield return fade.WaitForCompletion();
	}

	public IEnumerator FadeInCore()
	{
		_image.color = _defaultFadeInColorTarget;
		Tweener fade = _image.DOFade(0f, Settings.sceneFadeDuration);
		yield return fade.WaitForCompletion();
	}
}