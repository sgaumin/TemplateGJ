using System.IO;
using UnityEngine;


public static class GameData
{
	private const string fileName = "playerScore.json";

	static private string PathToDataFolder()
	{
		string savepath;

#if UNITY_EDITOR
		savepath = Application.dataPath;
#else
			savepath = Application.persistentDataPath;
#endif
		savepath += "/Data";
		if (!Directory.Exists(savepath))
		{
			Directory.CreateDirectory(savepath);
		}

		return savepath;
	}

	static public void SaveData()
	{
		//PlayerData playerData = new PlayerData();
		//playerData.bestScoreLevel1 = BestScoreLevel;

		//string filePath = Path.Combine(PathToDataFolder(), fileName);
		//string json = JsonUtility.ToJson(playerData);
		//File.WriteAllText(filePath, json);
	}

	static public void LoadData()
	{
		//string filePath = Path.Combine(PathToDataFolder(), fileName);

		//if (!File.Exists(filePath)) return;

		//string json = File.ReadAllText(filePath);

		//PlayerData playerDatas = JsonUtility.FromJson<PlayerData>(json);
		//BestScoreLevel1 = playerDatas.bestScoreLevel;
	}

	static public void ResetData()
	{

	}
}
