using System;
using System.Collections;
using Tools;
using UnityEngine;

public class Game : GameSystem
{
	private const string MENU_SCENE = "Menu";

	public delegate void GameEventHandler();
	public event GameEventHandler OnStart;
	public event GameEventHandler OnGameOver;
	public event GameEventHandler OnPause;

	[Header("References")]
	[SerializeField] private FadScreen fader;

	private GameStates gameState;
	private Coroutine loadingLevel;

	public GameStates GameState
	{
		get => gameState;
		set
		{
			gameState = value;

			switch (value)
			{
				case GameStates.Play:
					OnStart?.Invoke();
					break;

				case GameStates.GameOver:
					OnGameOver?.Invoke();
					break;

				case GameStates.Pause:
					OnPause?.Invoke();
					break;
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();
	}

	protected void Start()
	{
		GameState = GameStates.Play;
		fader.FadIn();
	}

	protected override void Update()
	{
		base.Update();
	}

	public void ReloadLevel()
	{
		if (loadingLevel == null)
		{
			loadingLevel = StartCoroutine(LoadLevelCore(

			content: () =>
			{
				LevelLoader.ReloadLevel();
			}));
		}
	}

	public void LoadNextLevel()
	{
		if (loadingLevel == null)
		{
			loadingLevel = StartCoroutine(LoadLevelCore(

			content: () =>
			{
				LevelLoader.LoadNextLevel();
			}));
		}
	}

	public void LoadMenu()
	{
		if (loadingLevel == null)
		{
			loadingLevel = StartCoroutine(LoadLevelCore(

			content: () =>
			{
				LevelLoader.LoadLevelByName(MENU_SCENE);
			}));
		}
	}

	private IEnumerator LoadLevelCore(Action content = null)
	{
		Time.timeScale = 1f;
		yield return fader.FadOutCore();
		content?.Invoke();
	}

}