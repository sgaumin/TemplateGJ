using TMPro;
using UnityEngine;

public class TextFontConfig : MonoBehaviour
{
	public static TextFontConfig Instance { get; private set; }

	public TMP_FontAsset TMPFont;
	public Font PlainFont;

	protected void Awake()
	{
		Instance = this;
	}

	protected void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}
}