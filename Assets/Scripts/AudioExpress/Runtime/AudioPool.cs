using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AudioExpress
{
	public static class AudioPool
	{
		private const string audioPoolParent = "AudioParent";
		private const string audioUnitPrefix = "AudioUnit: ";

		private static List<AudioUnit> audioPool = new List<AudioUnit>();
		private static GameObject audioParent;

		public static void Reset()
		{
			audioPool = new List<AudioUnit>();
			audioParent = null;
		}

		public static AudioUnit GetFromPool()
		{
			if (audioParent == null)
			{
				audioParent = new GameObject(audioPoolParent);
			}

			AudioUnit audio = audioPool.Where(x => x != null && !x.gameObject.activeSelf).FirstOrDefault();
			if (audio == null)
			{
				audio = new GameObject(audioUnitPrefix, typeof(AudioSource), typeof(AudioUnit)).GetComponent<AudioUnit>();
				audioPool.Add(audio);
				audio.transform.SetParent(audioParent.transform, false);
			}

			audio.name = audioUnitPrefix;

			audio.OnPlay += delegate ()
			{
				audio.gameObject.SetActive(true);
			};

			return audio;
		}

		public static void ReturnToPool(AudioUnit audio)
		{
			audio.gameObject.SetActive(false);
			if (!audioPool.Contains(audio))
			{
				audioPool.Add(audio);
			}
		}
	}
}