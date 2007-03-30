using System;
using System.Collections;

namespace Objection
{
	public class CachingImplementationResolver : ImplementationResolver
	{
		private readonly ImplementationResolver decoratoredResolver;
		private readonly IDictionary cachedResolvedTypes;

		public CachingImplementationResolver(ImplementationResolver decoratoredResolver) :
			this(decoratoredResolver, new Hashtable())
		{
		}

		public CachingImplementationResolver(ImplementationResolver decoratoredResolver, IDictionary cacheDictionary)
		{
			this.decoratoredResolver = decoratoredResolver;
			this.cachedResolvedTypes = cacheDictionary;
		}

		public Type ResolveImplementation(Type baseType)
		{
			Type resolvedType = (Type) cachedResolvedTypes[baseType];
			if (resolvedType == null)
			{
				resolvedType = decoratoredResolver.ResolveImplementation(baseType);
				cachedResolvedTypes.Add(baseType, resolvedType);
			}
			return resolvedType;
		}
	}
}