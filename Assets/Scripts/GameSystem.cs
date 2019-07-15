using UnityEngine;

public class GameSystem : MonoBehaviour
{
	public static GameSystem Instance { get; private set; }

	public GameStates GameState { get; private set; } = GameStates.Play;

	protected void Awake() => Instance = this;
}