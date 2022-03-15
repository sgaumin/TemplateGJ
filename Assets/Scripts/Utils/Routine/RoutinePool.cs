using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils
{
	public static class RoutinePool
	{
		private const string holderName = "RoutinePool";
		private const string unitPrefix = "RoutineUnit";

		private static List<RoutineUnit> pool = new List<RoutineUnit>();
		private static GameObject holder;

		public static void Run(IEnumerator routineMethod)
		{
			var r = GetFromPool();
			r.Run(routineMethod);
		}

		public static void Reset()
		{
			pool = new List<RoutineUnit>();
			holder = null;
		}

		private static RoutineUnit GetFromPool()
		{
			if (holder == null)
			{
				holder = new GameObject(holderName);
			}

			RoutineUnit routine = pool.Where(x => !x.gameObject.activeSelf).FirstOrDefault();
			if (routine == null)
			{
				routine = new GameObject(unitPrefix, typeof(RoutineUnit)).GetComponent<RoutineUnit>();
				pool.Add(routine);
				routine.transform.SetParent(holder.transform, false);
			}

			routine.gameObject.SetActive(true);

			return routine;
		}

		public static void ReturnToPool(RoutineUnit routine)
		{
			routine.gameObject.SetActive(false);
			if (!pool.Contains(routine))
			{
				pool.Add(routine);
			}
		}
	}
}