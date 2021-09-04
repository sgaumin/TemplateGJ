public static class Facade
{
	// See design pattern example: https://www.notion.so/Facade-b48dc31153384c428795ad7ad97dd7eb

	// Singletons
	public static Game GameController => Game.Instance;
	public static MusicPlayer GameMusic => MusicPlayer.Instance;
	public static TextFontConfig FontConfig => TextFontConfig.Instance;

	// Index
	public static Index Prefabs => Index.instance;
}