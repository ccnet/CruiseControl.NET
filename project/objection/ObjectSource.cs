using System;

namespace Objection
{
	public interface ObjectSource
	{
		object GetByType(Type type);
		object GetByName(string name);
	}
}
