using TMPro;
using UnityEngine;

public class TextFontConfig : MonoBehaviour
{
	public static TextFontConfig Instance { get; private set; }

	protected void Awake() => Instance = this;

	public TMP_FontAsset TMPFont;
	public Font PlainFont;
}