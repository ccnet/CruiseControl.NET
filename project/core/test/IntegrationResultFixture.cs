using System;
using tw.ccnet.core.util;
using tw.ccnet.remote;

namespace tw.ccnet.core.test
{
	public class IntegrationResultFixture
	{
		public static IntegrationResult CreateIntegrationResult()
		{
			IntegrationResult result = new IntegrationResult("testProject");
			result.MarkStartTime();
			result.Status = IntegrationStatus.Success;
			result.Label = "hello";
			result.Modifications = new Modification[] { new Modification() };
			result.Output = "<somexml>output</somexml>";
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
</buildResult>", result.Status, result.Label, result.StartTime, result.EndTime, result.Output, result.ProjectName));
		}

	}
}
