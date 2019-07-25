using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadScreen : MonoBehaviour
{
	public static FadScreen Instance { get; private set; }

	private Image image;

	private void Awake()
	{
		Instance = this;
		image = GetComponentInChildren<Image>();
	}

	public void FadOut(Color colorTarget)
	{
		image.color = new Color(colorTarget.r, colorTarget.g, colorTarget.b, 0f);
		image.DOFade(1f, 1f).Play();
	}

	public void FadIn(Color colorTarget)
	{
		image.color = colorTarget;
		image.DOFade(0f, 1f).Play();
	}
}
