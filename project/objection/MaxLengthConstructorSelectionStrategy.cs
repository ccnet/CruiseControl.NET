using System;
using System.Reflection;

namespace Objection
{
	public class MaxLengthConstructorSelectionStrategy : ConstructorSelectionStrategy
	{
		public ConstructorInfo GetConstructor(Type type)
		{
			ConstructorInfo candidate = null;
			foreach (ConstructorInfo ctor in type.GetConstructors())
			{
				if (candidate == null)
				{
					candidate = ctor;
				}
				else
				{
					if (ctor.GetParameters().Length > candidate.GetParameters().Length)
					{
						candidate = ctor;
					}
				}
			}			
			return candidate;
		}
	}
}
