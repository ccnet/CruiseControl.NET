using System;

namespace Objection
{
	public class CachingImplementationResolver : ImplementationResolver
	{
		private readonly ImplementationResolver decoratoredResolver;
		private readonly TypeToTypeMap resolvedTypeCache;

		public CachingImplementationResolver(ImplementationResolver decoratoredResolver, TypeToTypeMap resolvedTypeCache)
		{
			this.decoratoredResolver = decoratoredResolver;
			this.resolvedTypeCache = resolvedTypeCache;
		}

		public Type ResolveImplementation(Type baseType)
		{
			Type resolvedType = resolvedTypeCache[baseType];
			if (resolvedType == null)
			{
				resolvedType = AllowOneThreadPerAppDomainToDoResolution(baseType);
			}
			return resolvedType;
		}

		private Type AllowOneThreadPerAppDomainToDoResolution(Type baseType)
		{
			Type resolvedType;
			lock (baseType)
			{
				resolvedType = resolvedTypeCache[baseType];
				if (resolvedType == null)
				{
					resolvedType = decoratoredResolver.ResolveImplementation(baseType);
					resolvedTypeCache[baseType] = resolvedType;
				}
			}
			return resolvedType;
		}
	}
}