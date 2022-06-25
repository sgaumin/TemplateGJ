using System;
using System.Collections;
using UnityEngine.Networking;

namespace Utils
{
	public static class GetExternalIPAddress
	{
		public static string IP;

		public static IEnumerator Get()
		{
			if (!string.IsNullOrEmpty(IP)) yield break;

			UnityWebRequest www = UnityWebRequest.Get("http://checkip.dyndns.org");
			www.timeout = 10;
			yield return www.SendWebRequest();

			if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
			{
				throw new Exception(www.error);
			}
			else
			{
				string result = www.downloadHandler.text;
				string[] a = result.Split(':'); // Split into two substrings -> one before : and one after.
				string a2 = a[1].Substring(1);  // Get the substring after the :
				string[] a3 = a2.Split('<');    // Now split to the first HTML tag after the IP address.
				string a4 = a3[0];              // Get the substring before the tag.

				IP = a4;
			}
		}
	}
}