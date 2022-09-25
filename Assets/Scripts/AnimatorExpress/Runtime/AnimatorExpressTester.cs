using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AnimatorExpress
{
	public class AnimatorExpressTester : MonoBehaviour
	{
		private AnimatorExpress animator;

		private void Start()
		{
			animator = GetComponent<AnimatorExpress>();

			// TODO: Try to have unique event IDs to simplify parameters entry
			animator.AddListener("Characters-IdleAnimation", "OnIdle", IdleLog);
		}

		private void IdleLog()
		{
			Debug.Log("Hey!");
		}

		[ContextMenu("Attack")]
		public void Attack()
		{
			animator.Play("Characters-AttackAnimation");
		}

		[ContextMenu("Idle")]
		public void Idle()
		{
			animator.Play("Characters-IdleAnimation");
		}
	}
}