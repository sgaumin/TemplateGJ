using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using static Facade;

public class AssignScenesToBuildProcessor : AssetModificationProcessor
{
	public static string[] OnWillSaveAssets(string[] paths)
	{
		if (Settings.autoAssignScene)
		{
			AssignScenesToBuild();
		}
		return paths;
	}

	[MenuItem("Tools/Assign Scenes To Build", false, 2)]
	static void AssignScenesToBuild()
	{
		EditorBuildSettings.scenes = new EditorBuildSettingsScene[] { };
		List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();
		List<string> scenes = new List<string>();
		string MainFolder = "Assets/_Scenes";

		DirectoryInfo d = new DirectoryInfo(@MainFolder);
		FileInfo[] Files = d.GetFiles("*.unity");
		foreach (FileInfo file in Files)
		{
			scenes.Add(file.Name);
		}

		for (int i = 0; i < scenes.Count; i++)
		{
			string scenePath = MainFolder + "/" + scenes[i];
			editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(scenePath, true));
		}
		EditorBuildSettings.scenes = editorBuildSettingsScenes.OrderByDescending(x => x).ToArray();
	}
}