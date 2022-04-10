using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;

public static class LinksMenu
{
	[MenuItem("Links/GitHub Repo %#X", false, 0)]
	public static void OpenGitHub()
	{
		Application.OpenURL(Constants.GITHUB_PROJECT);
	}

	[MenuItem("Links/Project Folder  %#E", false, 1)]
	public static void OpenProjectFolder()
	{
		Process.Start(Path.GetDirectoryName(Application.dataPath));
	}

	[MenuItem("Links/Downloads Folder  %#D", false, 2)]
	public static void OpenDownloadsFolder()
	{
		Process.Start("shell:Downloads");
	}

	[MenuItem("Links/Itch Profile", false, 100)]
	public static void OpenItchProfile()
	{
		Application.OpenURL(Constants.ITCH_PROFILE);
	}

	[MenuItem("Links/GitHub Profile", false, 101)]
	public static void OpenGitHubProfile()
	{
		Application.OpenURL(Constants.GITHUB_PROFILE);
	}

	[MenuItem("Links/Twitter Profile", false, 102)]
	public static void OpenTwitterProfile()
	{
		Application.OpenURL(Constants.TWITTER_PROFILE);
	}
}