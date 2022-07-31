using UnityEngine;

public static class Logger
{
	public static void Report(string message)
	{
#if UNITY_EDITOR || LOG
		Debug.Log(message);
#endif
	}

	public static void Report(string message, Object context)
	{
#if UNITY_EDITOR || LOG
		Debug.Log(message, context);
#endif
	}

	public static void Warning(string message)
	{
#if UNITY_EDITOR || LOG
		Debug.LogWarning(message);
#endif
	}

	public static void Warning(string message, Object context)
	{
#if UNITY_EDITOR || LOG
		Debug.LogWarning(message, context);
#endif
	}

	public static void Error(string message)
	{
#if UNITY_EDITOR || LOG
		Debug.LogError(message);
#endif
	}

	public static void Error(string message, Object context)
	{
#if UNITY_EDITOR || LOG
		Debug.LogError(message, context);
#endif
	}

	public static void Break()
	{
#if UNITY_EDITOR || LOG
		Debug.Break();
#endif
	}
}