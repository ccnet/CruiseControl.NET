using System;
using System.Reflection;

namespace Objection
{
	public interface ConstructorSelectionStrategy
	{
		ConstructorInfo GetConstructor(Type type);
	}
}
