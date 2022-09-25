using System;
using UnityEngine;

namespace AnimatorExpress
{
	[Serializable]
	public class Frame
	{
		[SerializeField] private Sprite sprite;
		[SerializeField] private float duration;

		public Sprite Sprite => sprite;
		public float Duration => duration;

		public Frame(Sprite sprite)
		{
			this.sprite = sprite;
		}
	}
}