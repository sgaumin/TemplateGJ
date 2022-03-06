using System.IO;
using UnityEditor;
using UnityEngine;

// Based on documentation: https://docs.unity3d.com/Manual/BuildPlayerPipeline.html
public class ScriptBatch
{
	[MenuItem("Tools/Build Main Platforms %B", false, 1)]
	public static void BuildMainPlatform()
	{
		BuildTarget startTarget = EditorUserBuildSettings.activeBuildTarget;
		BuildTargetGroup startTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
		BuildProcess(BuildTarget.WebGL);
		BuildProcess(BuildTarget.StandaloneWindows64);
		BuildProcess(BuildTarget.StandaloneWindows, true);
		EditorUserBuildSettings.SwitchActiveBuildTarget(startTargetGroup, startTarget);
	}

	public static void BuildProcess(BuildTarget target, bool showBuildLocation = false)
	{
		string generalBuildsFolder = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Builds");
		if (!Directory.Exists(generalBuildsFolder))
		{
			Directory.CreateDirectory(generalBuildsFolder);
		}

		string templateName = $"{ Application.productName.Replace(" ", "") }_{ target}_{ Application.version}";
		string buildFolder = Path.Combine(generalBuildsFolder, templateName);
		if (!Directory.Exists(buildFolder))
		{
			Directory.CreateDirectory(buildFolder);
		}

		string buildName;
		if (target == BuildTarget.WebGL)
		{
			buildName = buildFolder;
		}
		else
		{
			buildName = Path.Combine(buildFolder, $"{Application.productName}.exe");
		}
		if (EditorBuildSettings.scenes.Length > 0)
		{
			BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, buildName, target, BuildOptions.None);
		}

		// Compression
		string zipFolder = Path.Combine(generalBuildsFolder, $"{templateName}.zip");
		if (File.Exists(zipFolder))
		{
			File.Delete(zipFolder);
		}
		System.IO.Compression.ZipFile.CreateFromDirectory(buildFolder, zipFolder, System.IO.Compression.CompressionLevel.Fastest, false);

		// Deletion build folder
		if (Directory.Exists(buildFolder))
		{
			Directory.Delete(buildFolder, true);
		}

		// Open Explorer
		if (showBuildLocation)
		{
			System.Diagnostics.Process.Start(generalBuildsFolder);
		}
	}
}