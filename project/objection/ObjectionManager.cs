using System;

namespace Objection
{
	public interface ObjectionManager
	{
		void AddInstance(params object[] instances);
		void AddInstanceForType(Type type, object instance);
		DecoratableByType AddInstanceForName(string identifier, object instance);

		DecoratableByType AddTypeForName(string identifier, Type type);
		void SetDependencyImplementationForName(string identifier, Type dependencyType, Type implementationType);

		void SetImplementationType(Type parentType, Type implementationType);
		void SetDependencyImplementationForType(Type typeWithDependency, Type dependencyParentType, Type dependencyImplementationType);
	}
}
