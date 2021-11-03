using DG.Tweening;
using System.Collections;
using TMPro;
using Tools;
using Tools.Utils;
using UnityEngine;
using static Facade;

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