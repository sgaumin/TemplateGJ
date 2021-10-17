public static class Facade
{
	// See design pattern example: https://www.notion.so/Facade-b48dc31153384c428795ad7ad97dd7eb
	public static LevelController Level => LevelController.Instance;
	public static MusicPlayer Music => MusicPlayer.Instance;
	public static TextFontConfig FontConfig => TextFontConfig.Instance;
	public static PrefabsData Prefabs => PrefabsData.Instance;
	public static SettingsData Settings => SettingsData.Instance;
}