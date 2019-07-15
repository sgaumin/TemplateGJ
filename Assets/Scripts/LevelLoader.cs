using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
	public const string menuScene = "Menu";
	public const string creditsScene = "Credits";
	public const string gameScene = "Game";

	public static LevelLoader Instance { get; private set; }

	protected void Awake() => Instance = this;

	public void ReloadLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

	public void LoadNextLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

	public void LoadMenu() => SceneManager.LoadScene(menuScene);

	public void LoadCredits() => SceneManager.LoadScene(creditsScene);

	public void LoadGame() => SceneManager.LoadScene(gameScene);

	public void QuitGame()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
	}
}