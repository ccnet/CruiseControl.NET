using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Configuration;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Schedules;

namespace integration
{
	[TestFixture]
	public class RunMultipleProjects
	{
		const string PROJECT1_NAME = "IntegrationTestProject1";
		const string PROJECT2_NAME = "IntegrationTestProject2";

		[TearDown]
		protected void TearDown()
		{
			TempFileUtil.DeleteTempDir(PROJECT1_NAME);
		}

		[Test]
		public void RunIntegration()
		{
			string project1Xml = ConfigurationFileFixture.GenerateSimpleProjectXml(PROJECT1_NAME, IntegrationFixture.CreateSchedule(2));
			string project2Xml = ConfigurationFileFixture.GenerateSimpleProjectXml(PROJECT2_NAME, IntegrationFixture.CreateSchedule(4));

			CruiseServer cc = IntegrationFixture.CreateCruiseControl(PROJECT1_NAME, project1Xml, project2Xml);			
			IntegrationEventCounter counter1 = IntegrationFixture.AddIntegrationEventHandler(cc, PROJECT1_NAME);
			IntegrationEventCounter counter2 = IntegrationFixture.AddIntegrationEventHandler(cc, PROJECT2_NAME);

			cc.Start();
			cc.WaitForExit();

			Assertion.AssertEquals(2, counter1.EventCount);
			Assertion.AssertEquals(4, counter2.EventCount);

			// validate integrationresults
		}

		private void ValidateIntegrationResults(string projectName, string expectedLabel)
		{
			IntegrationResult result = IntegrationFixture.LoadIntegrationResult(projectName);
			Assertion.AssertNotNull(result);
			Assertion.AssertEquals(true, result.Succeeded);
			Assertion.AssertEquals(expectedLabel, result.Label);
		}
	}
}
