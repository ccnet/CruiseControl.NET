using System;

namespace ObjectWizard
{
	public interface ObjectGiverManager
	{
		void AddTypedObject(Type type, object instance);
		void SetImplementationType(Type parentType, Type implementationType);
		void SetDependencyImplementationForType(Type typeWithDependency, Type dependencyParentType, Type dependencyImplementationType);
		void SetDependencyImplementationForIdentifer(string identifier, Type dependencyType, Type implementationType);
		TypeDecoratorable CreateImplementationMapping(string identifier, Type type);
	}
}
