using TMPro;
using UnityEngine;
using Utils;

public class TextFontConfig : Singleton<TextFontConfig>
{
	[SerializeField] private TMP_FontAsset _TMPFont;
	[SerializeField] private Font _PlainFont;

	public TMP_FontAsset TMPFont => _TMPFont;
	public Font PlainFont => _PlainFont;
}