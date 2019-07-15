using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
	public const string menuScene = "Menu";
	public const string creditsScene = "Credits";
	public const string gameScene = "Game";

	public static LevelLoader Instance { get; private set; }

	protected void Awake() => Instance = this;

	public void ReloadLevel()
	{
		LevelClear();
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void LoadNextLevel()
	{
		LevelClear();
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void LoadMenu()
	{
		LevelClear();
		SceneManager.LoadScene(menuScene);
	}

	public void LoadCredits()
	{
		LevelClear();
		SceneManager.LoadScene(creditsScene);
	}

	public void LoadGame()
	{
		LevelClear();
		SceneManager.LoadScene(gameScene);
	}

	public void QuitGame()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
	}

	private void LevelClear() => DOTween.Clear(true);
}