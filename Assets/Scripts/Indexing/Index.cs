using Tools;
using UnityEngine;

// See Design Pattern example: https://www.notion.so/Index-8c49dc7f08e241238ca8b933268d2661

public sealed class Index : BaseIndex
{
	// The static instance is what allows us to get the index from anywhere in the code:
	private static Index _instance;
	public static Index Instance => GetOrLoad(ref _instance);

	// Set up your references below!
	// You only need to assign references once with this pattern.

	// public Cell cell;
}