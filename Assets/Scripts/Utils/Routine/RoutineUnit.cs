using System.Collections;
using UnityEngine;

namespace Utils
{
	public class RoutineUnit : MonoBehaviour
	{
		private Coroutine routine;

		public void Run(IEnumerator routineMethod)
		{
			this.StartCoroutine(routineMethod, ref routine);
		}
	}
}