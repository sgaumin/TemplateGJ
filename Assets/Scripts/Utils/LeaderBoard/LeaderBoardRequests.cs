using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Utils
{
	public static class LeaderBoardRequests
	{
		private static string URL = "https://personal-unity-games.herokuapp.com/";

		public static void Get(Action<string> callback = null)
		{
			RoutinePool.Run(GetCore(callback));
		}

		public static void Post(string name, float value, Action<string> callback = null)
		{
			RoutinePool.Run(PostCore(name, value, callback));
		}

		private static IEnumerator GetCore(Action<string> callback = null)
		{
			var url = $"{URL}getLeaderBoard.php";
			using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
			{
				yield return webRequest.SendWebRequest();
				var result = DoValidation(url, webRequest);
				callback?.Invoke(result);
			}
		}

		private static IEnumerator PostCore(string name, float value, Action<string> callback = null)
		{
			var url = $"{URL}addLeaderBoardEntry.php";

			WWWForm form = new WWWForm();
			form.AddField("game", Application.productName);
			form.AddField("name", name);
			form.AddField("value", value.ToString());
			form.AddField("utx", TimeManager.UnixTimeNow().ToString());

			using (UnityWebRequest webRequest = UnityWebRequest.Post(url, form))
			{
				yield return webRequest.SendWebRequest();
				var result = DoValidation(url, webRequest);
				callback?.Invoke(result);
			}
		}

		private static string DoValidation(string url, UnityWebRequest webRequest)
		{
			string[] pages = url.Split('/');
			int page = pages.Length - 1;

			switch (webRequest.result)
			{
				case UnityWebRequest.Result.DataProcessingError:
					Debug.LogError(pages[page] + ": Error: " + webRequest.error);
					break;

				case UnityWebRequest.Result.ProtocolError:
					Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
					break;

				case UnityWebRequest.Result.Success:
					Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
					return webRequest.downloadHandler.text;
			}

			return null;
		}
	}
}