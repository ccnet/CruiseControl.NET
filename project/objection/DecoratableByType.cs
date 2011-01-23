using System;

namespace Objection
{
	public interface DecoratableByType
	{
		DecoratableByType Decorate(Type type);
	}
}
