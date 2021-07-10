using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
internal struct I18nImageData
{
	public Language language;
	public Sprite sprite;
	public Sprite buttonActiveSprite;
}

public class I18nImageSelector : MonoBehaviour
{
	[SerializeField] private List<I18nImageData> options = new List<I18nImageData>();

	private SpriteRenderer spriteRenderer;
	private Image image;
	private Button button;

	protected void Start()
	{
		image = GetComponent<Image>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		button = GetComponent<Button>();

		SetSprite();
	}

	private void SetSprite()
	{
		I18nImageData op = options.Where(x => x.language == GameData.CurrentLanguage).FirstOrDefault();
		if (image != null)
			image.sprite = op.sprite;
		else if (spriteRenderer != null)
			spriteRenderer.sprite = op.sprite;

		if (button != null && op.buttonActiveSprite != null)
		{
			SpriteState states;
			states.highlightedSprite = op.buttonActiveSprite;
			states.pressedSprite = op.buttonActiveSprite;
			states.selectedSprite = op.buttonActiveSprite;

			button.spriteState = states;
		}
	}
}