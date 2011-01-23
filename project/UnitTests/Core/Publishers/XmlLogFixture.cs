using System;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
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
				return string.Format(@"<build date=""{0}"" buildtime=""00:00:00""{1} buildcondition=""{2}"" />", DateUtil.FormatDate(result.StartTime), error, result.BuildCondition);
			}
			else
			{
			    expectedBuildOutput = expectedBuildOutput.Replace("\r", string.Empty);
				return string.Format(@"<build date=""{0}"" buildtime=""00:00:00""{1} buildcondition=""{3}"">{2}</build>", DateUtil.FormatDate(result.StartTime), error, expectedBuildOutput, result.BuildCondition);
			}
		}
	}
}
