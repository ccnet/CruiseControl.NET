using System;
using tw.ccnet.remote;

namespace tw.ccnet.core
{
	public class IntegrationStatusParser
	{
		public static IntegrationStatus Parse(string value)
		{
			return (IntegrationStatus)Enum.Parse(typeof(IntegrationStatus), value);
		}
	}
}
