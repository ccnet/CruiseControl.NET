using Exortech.NetReflector;
using Exortech.NetReflector.Util;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class TimeoutSerializerFactory : ISerialiserFactory
	{
		public IXmlSerialiser Create(ReflectorMember memberInfo, ReflectorPropertyAttribute attribute)
		{
			return new TimeoutSerializer(memberInfo, attribute);
		}
	}
}