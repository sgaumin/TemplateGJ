using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.UI;
using static Facade;

public class TextFontSetter : MonoBehaviour
{
	[SerializeField] private Dependency<TextMeshProUGUI> _tmpText;
	[SerializeField] private Dependency<Text> _plainText;

	private TextMeshProUGUI tmpText => _tmpText.Resolve(this);
	private Text plainText => _plainText.Resolve(this);

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