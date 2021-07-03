using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextFontSetter : MonoBehaviour
{
	private TextMeshProUGUI tmpText;
	private Text plainText;

	protected void Start()
	{
		tmpText = GetComponent<TextMeshProUGUI>();
		plainText = GetComponent<Text>();
		SetFont();
	}

	public void SetFont()
	{
		if (tmpText != null)
			tmpText.font = TextFontConfig.Instance.TMPFont;
		if (plainText != null)
			plainText.font = TextFontConfig.Instance.PlainFont;
	}
}
