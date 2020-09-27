using Tools;
using UnityEngine;

public class Game : GameSystem
{
	public static Game Instance { get; private set; }

	public delegate void GameEventHandler();
	public event GameEventHandler OnStart;
	public event GameEventHandler OnGameOver;
	public event GameEventHandler OnPause;

	[Header("References")]
	[SerializeField] private FadScreen fader;

	private GameStates gameState;

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
		Instance = this;
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
}