using UnityEngine;

public class SettingsData : BaseIndex
{
	private static SettingsData _instance;

	public static SettingsData Instance => GetOrLoad(ref _instance);

	// Set up your references below!
	public bool autoAssignScene = true;

	[Header("Animations")]
	public float sceneFadeDuration = 0.5f;

	[Header("Audio")]
	public float audioFadeDuration = 0.2f;
}