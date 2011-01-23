using System;

namespace Objection
{
	public interface ImplementationResolver
	{
		Type ResolveImplementation(Type baseType);
	}
}
