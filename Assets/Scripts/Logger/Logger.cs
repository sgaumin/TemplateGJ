using UnityEngine;

public static class Logger
{
	public static void Report(string message)
	{
#if UNITY_EDITOR || LOG
		Debug.Log(message);
#endif
	}

	public static void Warning(string message)
	{
#if UNITY_EDITOR || LOG
		Debug.LogWarning(message);
#endif
	}

	public static void Error(string message)
	{
#if UNITY_EDITOR || LOG
		Debug.LogError(message);
#endif
	}
}