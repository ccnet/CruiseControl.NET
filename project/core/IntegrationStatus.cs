using System;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	public class IntegrationStatusParser
	{
		public static IntegrationStatus Parse(string value)
		{
			return (IntegrationStatus)Enum.Parse(typeof(IntegrationStatus), value);
		}
	}
}
