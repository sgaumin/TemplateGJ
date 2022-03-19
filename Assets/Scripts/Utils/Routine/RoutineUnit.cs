using System.Collections;
using UnityEngine;

namespace Utils
{
	public class RoutineUnit : MonoBehaviour
	{
		private Coroutine routine;

		public void Run(IEnumerator routineMethod)
		{
			this.TryStartCoroutine(RunCore(routineMethod), ref routine);
		}

		private IEnumerator RunCore(IEnumerator routineMethod)
		{
			yield return routineMethod;
			RoutinePool.ReturnToPool(this);
		}
	}
}