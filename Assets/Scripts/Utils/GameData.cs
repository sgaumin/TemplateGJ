using System;
using UnityEngine;

public static class GameData
{
	private const string DATA = "data";
	private const string LANGUAGE = "language";

	// Template Below
	public static string Data
	{
		get
		{
			return PlayerPrefs.HasKey(DATA) ? PlayerPrefs.GetString(DATA) : "";
		}
		set
		{
			PlayerPrefs.SetString(DATA, value);
			PlayerPrefs.Save();
		}
	}

	public static Language CurrentLanguage
	{
		get
		{
			return PlayerPrefs.HasKey(LANGUAGE) ? (Language)Enum.Parse(typeof(Language), PlayerPrefs.GetString(DATA)) : Language.French;
		}
		set
		{
			PlayerPrefs.SetString(LANGUAGE, value.ToString());
			PlayerPrefs.Save();
		}
	}

	public static void DeleteAllSave()
	{
		PlayerPrefs.DeleteAll();
		PlayerPrefs.Save();
	}
}