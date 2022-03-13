public class LevelController : SceneBase, ISingleton
{
	public static LevelController Instance { get; private set; }

	public SingletonPriority GetSingletonPriority()
	{
		return SingletonPriority.VeryHigh;
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