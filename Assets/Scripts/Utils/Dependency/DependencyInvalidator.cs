#if UNITY_EDITOR
using UnityEditor;

namespace Lazlo
{
	public static class DependencyInvalidator
	{
		public static uint version { get; private set; }

		static DependencyInvalidator()
		{
			EditorApplication.hierarchyChanged += InvalidateAllDependencies;
		}

		public static void InvalidateAllDependencies()
		{
			version++;
		}
	}
}

#endif