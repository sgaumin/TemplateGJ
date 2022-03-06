using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Facade;

public class TextFontSetter : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI tmpText;
	[SerializeField] private Text plainText;

	protected void Start()
	{
		SetFont();
	}

	public void SetFont()
	{
		if (tmpText != null)
			tmpText.font = FontConfig.TMPFont;
		if (plainText != null)
			plainText.font = FontConfig.PlainFont;
	}
}