using System;
using Exortech.NetReflector.Util;

namespace Objection.NetReflectorPlugin
{
	public class ObjectionNetReflectorInstantiator : IInstantiator
	{
		private readonly ObjectSource objectSource;

		public ObjectionNetReflectorInstantiator(ObjectSource objectSource)
		{
			this.objectSource = objectSource;
		}

		public object Instantiate(Type type)
		{
			return objectSource.GetByType(type);
		}
	}
}
