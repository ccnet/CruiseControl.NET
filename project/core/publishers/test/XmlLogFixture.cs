using System;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Test
{
	public class XmlLogFixture : CustomAssertion
	{
		public string CreateExpectedBuildXml(IntegrationResult result)
		{
			string error = (result.Status == IntegrationStatus.Success) ? String.Empty : " error=\"true\"";
			if (result.Output == null)
			{
				return string.Format(@"<build date=""{0}"" buildtime=""00:00:00""{1} />", result.StartTime, error);
			}
			else
			{
				return string.Format(@"<build date=""{0}"" buildtime=""00:00:00""{1}>{2}</build>", result.StartTime, error, result.Output);
			}
		}
	}
}
