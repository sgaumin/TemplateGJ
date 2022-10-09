using System.Collections;
using System.Data;
using UnityEngine;

namespace RoutineExpress
{
	public class RoutineUnit : MonoBehaviour
	{
		private Coroutine routine;

		public void Run(IEnumerator routineMethod)
		{
			if (routine is not null)
			{
				StopCoroutine(routine);
			}
			routine = StartCoroutine(RunCore(routineMethod));
		}

		private IEnumerator RunCore(IEnumerator routineMethod)
		{
			yield return routineMethod;
			RoutinePool.ReturnToPool(this);
		}
	}
}