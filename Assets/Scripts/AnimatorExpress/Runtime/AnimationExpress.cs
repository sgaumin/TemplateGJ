using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationExpress", menuName = "AnimationExpress", order = 1)]
public class AnimationExpress : ScriptableObject
{
	[SerializeField] private bool isLooping;
	[SerializeField] private Frame[] frames;

	public bool IsLooping => isLooping;
	public Frame[] Frames => frames;
}