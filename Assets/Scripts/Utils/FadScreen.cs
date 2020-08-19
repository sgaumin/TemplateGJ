using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadScreen : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private Image image;

	private Color defaultFadInColorTarget = Color.black;
	private Color defaultFadOutColorTarget = Color.black.WithAlpha(0);
	private Ease defaultEase = Ease.Linear;

	public void FadOut(Color? colorTarget = null, float fadDuration = 1f, Ease? ease = null)
	{
		image.color = colorTarget != null ? new Color((float)colorTarget?.r, (float)colorTarget?.g, (float)colorTarget?.b, 0f) : defaultFadOutColorTarget;
		image.DOFade(1f, fadDuration).SetEase(ease ?? defaultEase).Play();
	}

	public IEnumerator FadOutCore(Color? colorTarget = null, float fadDuration = 1f, Ease? ease = null)
	{
		image.color = colorTarget != null ? new Color((float)colorTarget?.r, (float)colorTarget?.g, (float)colorTarget?.b, 0f) : defaultFadOutColorTarget;
		Tweener fad = image.DOFade(1f, fadDuration).SetEase(ease ?? defaultEase).Play();
		yield return fad.WaitForCompletion();
	}

	public void FadIn(Color? colorTarget = null, float fadDuration = 1f, Ease? ease = null)
	{
		image.color = colorTarget ?? defaultFadInColorTarget;
		image.DOFade(0f, fadDuration).SetEase(ease ?? defaultEase).Play();
	}

	public IEnumerator FadInCore(Color? colorTarget = null, float fadDuration = 1f, Ease? ease = null)
	{
		image.color = colorTarget ?? defaultFadInColorTarget;
		Tweener fad = image.DOFade(0f, fadDuration).SetEase(ease ?? defaultEase).Play();
		yield return fad.WaitForCompletion();
	}
}