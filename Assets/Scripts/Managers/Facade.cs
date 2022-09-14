public static class Facade
{
	public static SceneBase Level => SceneBase.Instance;
	public static PrefabsData Prefabs => PrefabsData.Instance;
	public static SettingsData Settings => SettingsData.Instance;
}