using System;
using Exortech.NetReflector.Util;

namespace ObjectWizard.NetReflector
{
	public class ObjectGiverNetReflectorInstantiator : IInstantiator
	{
		private readonly ObjectGiver objectGiver;

		public ObjectGiverNetReflectorInstantiator(ObjectGiver objectGiver)
		{
			this.objectGiver = objectGiver;
		}

		public object Instantiate(Type type)
		{
			return objectGiver.GiveObjectByType(type);
		}
	}
}
