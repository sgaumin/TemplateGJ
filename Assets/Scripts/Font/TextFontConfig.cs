using TMPro;
using UnityEngine;

public class TextFontConfig : MonoBehaviour
{
	public static TextFontConfig Instance { get; private set; }

	protected void Awake() => Instance = this;

	[SerializeField] private TMP_FontAsset tmpFont;
	[SerializeField] private Font plainFont;

	public TMP_FontAsset TMPFont => tmpFont;
	public Font PlainFont => plainFont;
}
