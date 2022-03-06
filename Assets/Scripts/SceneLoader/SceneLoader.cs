using DG.Tweening;
#if !UNITY_EDITOR
using UnityEngine;
#endif
using UnityEngine.SceneManagement;

public static class SceneLoader
{
	/// <summary>
	/// Load the corresponding scene.
	/// </summary>
	/// <param name="sceneLoading">scene to load.</param>
	public static void Load(SceneLoading sceneLoading)
	{
		switch (sceneLoading)
		{
			case SceneLoading.Previous:
				LoadPrevious();
				break;

			case SceneLoading.Reload:
				Reload();
				break;

			case SceneLoading.Next:
				Next();
				break;

			case SceneLoading.First:
				LoadScene(0);
				break;
		}
	}

	/// <summary>
	/// Reload the current scene.
	/// </summary>
	public static void Reload()
	{
		SceneClear();
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	/// <summary>
	/// Load next scene present in the build settings window.
	/// </summary>
	public static void Next()
	{
		SceneClear();
		int currentBuildIndex = SceneManager.GetActiveScene().buildIndex;
		int maxBuildIndex = SceneManager.sceneCountInBuildSettings;

		// We check if the current scene is not the last one.
		SceneManager.LoadScene(currentBuildIndex + (currentBuildIndex == maxBuildIndex ? 0 : 1));
	}

	/// <summary>
	/// Load next scene present in the build settings window.
	/// </summary>
	public static void LoadPrevious()
	{
		SceneClear();
		int currentBuildIndex = SceneManager.GetActiveScene().buildIndex;

		// We check if the current scene is not the first one.
		SceneManager.LoadScene(currentBuildIndex + (currentBuildIndex != 0 ? -1 : 0));
	}

	/// <summary>
	/// Load a scene by its name.
	/// </summary>
	/// <param name="name"></param>
	public static void Load(string name)
	{
		SceneClear();
		SceneManager.LoadScene(name);
	}

	/// <summary>
	/// Load a scene by its build index.
	/// </summary>
	/// <param name="index"></param>
	public static void LoadScene(int index)
	{
		SceneClear();
		SceneManager.LoadScene(index);
	}

	/// <summary>
	/// Quit the game or the editor mode.
	/// </summary>
	public static void Quit()
	{
#if !UNITY_EDITOR
		Application.Quit();
#else
		UnityEditor.EditorApplication.isPlaying = false;
#endif
	}

	private static void SceneClear()
	{
		AudioPool.ResetAudioPool();
		DOTween.Clear(false);
	}
}