public class LevelController : SceneBase, ISingleton
{
	public static LevelController Instance { get; private set; }

	public int GetSingletonPriority()
	{
		return 100;
	}

	public void OnSingletonSetup()
	{
		Instance = this;
	}

	protected override void Start()
	{
		base.Start();
	}

	protected override void Update()
	{
		base.Update();
	}

	protected void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}
}