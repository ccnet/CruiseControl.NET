using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
	public class IntegrationResultFixture
	{
		public static IntegrationResult CreateIntegrationResult()
		{
			IntegrationResult result = new IntegrationResult("testProject", @"c:\temp");
			result.MarkStartTime();
			result.Status = IntegrationStatus.Success;
			result.Label = "hello";
			result.Modifications = new Modification[] { new Modification() };
			result.MarkEndTime();
			return result;
		}

		public static string GenerateDefaultXml(IntegrationResult result)
		{
			return XmlUtil.GenerateOuterXml(string.Format(
@"<buildResult projectName=""{5}"">
	<status>{0}</status>
	<label>{1}</label>
	<startTime>{2}</startTime>
	<endTime>{3}</endTime>
	<modifications>
		<modification/>
	</modifications>
	<output>{4}</output>
</buildResult>", result.Status, result.Label, result.StartTime, result.EndTime, result.TaskOutput, result.ProjectName));
		}
	}
}
