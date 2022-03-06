using TMPro;
using UnityEngine;

public class TextFontConfig : MonoBehaviour, ISingleton
{
	public static TextFontConfig Instance { get; private set; }

	public TMP_FontAsset TMPFont;
	public Font PlainFont;

	public int GetSingletonPriority()
	{
		return 0;
	}

	public void OnSingletonSetup()
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