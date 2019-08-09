public class Game : GameSystem
{
	public static Game Instance { get; private set; }

	public GameStates GameState { get; private set; } = GameStates.Play;

	protected override void Awake()
	{
		base.Awake();
		Instance = this;
	}
}
