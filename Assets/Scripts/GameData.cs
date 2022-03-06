using System;
using UnityEngine;

public static class GameData
{
	private const string USER_NAME = "userName";
	private const string LANGUAGE = "language";

	public static string UserName
	{
		get
		{
			return PlayerPrefs.HasKey(USER_NAME) ? PlayerPrefs.GetString(USER_NAME) : "";
		}
		set
		{
			PlayerPrefs.SetString(USER_NAME, value);
			PlayerPrefs.Save();
		}
	}

	public static Language CurrentLanguage
	{
		get
		{
			return PlayerPrefs.HasKey(LANGUAGE) ? (Language)Enum.Parse(typeof(Language), PlayerPrefs.GetString(LANGUAGE)) : Language.French;
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