using TMPro;
using UnityEngine;

public class TextFontConfig : MonoBehaviour, ISingleton
{
	public static TextFontConfig Instance { get; private set; }

	public TMP_FontAsset TMPFont;
	public Font PlainFont;

	public SingletonPriority GetSingletonPriority()
	{
		return SingletonPriority.VeryLow;
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