using System;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
	public class XmlLogFixture : CustomAssertion
	{
		public string CreateExpectedBuildXml(IntegrationResult result)
		{
			return CreateExpectedBuildXml(result, null);
		}

		public static string CreateExpectedBuildXml(IntegrationResult result, string expectedBuildOutput)
		{
			string error = (result.Status == IntegrationStatus.Failure) ? " error=\"true\"" : String.Empty;
			if (expectedBuildOutput == null)
			{
				return string.Format(@"<build date=""{0}"" buildtime=""00:00:00""{1} buildcondition=""{2}"" />", result.StartTime, error, result.BuildCondition);
			}
			else
			{
				return string.Format(@"<build date=""{0}"" buildtime=""00:00:00""{1} buildcondition=""{3}"">{2}</build>", result.StartTime, error, expectedBuildOutput, result.BuildCondition);
			}
		}
	}
}
