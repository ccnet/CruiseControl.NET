using System;

namespace Objection
{
	public interface TypeToTypeMap
	{
		Type this[Type baseType] {get; set;}
	}
}