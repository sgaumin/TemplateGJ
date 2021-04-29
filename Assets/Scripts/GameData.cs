using UnityEngine;


public static class GameData
{
	private const string DATA = "data";

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

	public static void DeleteAllSave()
	{
		PlayerPrefs.DeleteAll();
		PlayerPrefs.Save();
	}
}
