using System;
using UnityEngine;

namespace Utils.Dependency
{
	public interface IDependency
	{
		Type type { get; }

		DependencySource source { get; }

		Component localReference { get; }

		bool CanResolve(Component self);

		Component Resolve(Component self);

		bool TryResolve(Component self, out Component component);
	}
}