using System;
using System.Diagnostics;
using System.IO;
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
	public class RunSimpleProject
	{
		public const string PROJECT_NAME = "RunSimpleProject";
		private int _integrationExceptionCalls;

		[SetUp]
		protected void SetUp()
		{
			TempFileUtil.DeleteTempDir(PROJECT_NAME);
			_integrationExceptionCalls = 0;
		}

		[TearDown]
		protected void TearDown()
		{
			Thread.Sleep(100);
			TempFileUtil.DeleteTempDir(PROJECT_NAME);
			Thread.Sleep(100);
		}

		[Test]
		public void RunIntegration()
		{
			string projectXml = ConfigurationFileFixture.GenerateSimpleProjectXml(PROJECT_NAME, IntegrationFixture.CreateSchedule(1));
			CruiseServer cc = IntegrationFixture.CreateCruiseControl(PROJECT_NAME, projectXml);
			IntegrationEventCounter counter = IntegrationFixture.AddIntegrationEventHandler(cc, PROJECT_NAME);

			cc.Start();
			cc.WaitForExit();

			Assertion.AssertEquals(1, counter.EventCount);
			Assertion.AssertEquals(0, _integrationExceptionCalls);

			// verify state file is created and that build ran successfully
			IntegrationResult result = IntegrationFixture.LoadIntegrationResult(PROJECT_NAME);
			Assertion.AssertNotNull(result);
			Assertion.AssertEquals(true, result.Succeeded);
			Assertion.AssertNotNull(result.StartTime);
			Assertion.AssertNotNull(result.EndTime);
			Assertion.AssertEquals("1", result.Label);
			Assertion.AssertNotNull(result.Output);
			Assertion.AssertEquals(PROJECT_NAME, result.ProjectName);
			Assertion.AssertEquals(1, result.Modifications.Length);
			Assertion.AssertEquals(IntegrationStatus.Success, result.Status);
			Assertion.AssertEquals(IntegrationStatus.Unknown, result.LastIntegrationStatus);
			Assertion.Assert(result.LastModificationDate.CompareTo(result.StartTime) >= 0);
		}

		[Test]
		public void RunIntegrationTwice()
		{
			string projectXml = ConfigurationFileFixture.GenerateSimpleProjectXml(PROJECT_NAME, IntegrationFixture.CreateSchedule(2));
			CruiseServer cc = IntegrationFixture.CreateCruiseControl(PROJECT_NAME, projectXml);
			IntegrationEventCounter counter = IntegrationFixture.AddIntegrationEventHandler(cc, PROJECT_NAME);

			cc.Start();
			cc.WaitForExit();

			Assertion.AssertEquals(2, counter.EventCount);
			Assertion.AssertEquals(0, _integrationExceptionCalls);

			// verify state file is created and that build ran successfully
			IntegrationResult result = IntegrationFixture.LoadIntegrationResult(PROJECT_NAME);
			Assertion.AssertNotNull(result);
			Assertion.AssertEquals(true, result.Succeeded);
			Assertion.AssertEquals("2", result.Label);
			Assertion.AssertEquals(IntegrationStatus.Success, result.Status);
			Assertion.AssertEquals(IntegrationStatus.Success, result.LastIntegrationStatus);
		}

		[Test]
		public void RunIntegrationThrice()
		{
			string projectXml = ConfigurationFileFixture.GenerateSimpleProjectXml(PROJECT_NAME, IntegrationFixture.CreateSchedule(3));
			CruiseServer cc = IntegrationFixture.CreateCruiseControl(PROJECT_NAME, projectXml);
			IntegrationEventCounter counter = IntegrationFixture.AddIntegrationEventHandler(cc, PROJECT_NAME);
			cc.Start();
			cc.WaitForExit();

			Assertion.AssertEquals(3, counter.EventCount);
			Assertion.AssertEquals(0, _integrationExceptionCalls);

			// verify state file is created and that build ran successfully
			IntegrationResult result = IntegrationFixture.LoadIntegrationResult(PROJECT_NAME);
			Assertion.AssertNotNull(result);
			Assertion.AssertEquals(true, result.Succeeded);
			Assertion.AssertEquals("3", result.Label);
		}

		[Test]
		public void RunIntegrationLoop()
		{
			string projectXml = ConfigurationFileFixture.GenerateSimpleProjectXml(PROJECT_NAME, IntegrationFixture.CreateSchedule(Schedule.Infinite));
			CruiseServer cc = IntegrationFixture.CreateCruiseControl(PROJECT_NAME, projectXml);
			IntegrationEventCounter counter = IntegrationFixture.AddIntegrationEventHandler(cc, PROJECT_NAME);
			RunLoopTest(cc, counter);
		}

		private void RunLoopTest(CruiseServer cc, IntegrationEventCounter counter)
		{
			cc.Start();
			Thread.Sleep(100);
			cc.Stop();
			cc.WaitForExit();

			Assertion.Assert(counter.EventCount >= 1);
//			Console.WriteLine("runs: " + _integrationCompleteCalls);
			Assertion.AssertEquals(0, _integrationExceptionCalls);

			// verify state file is created and that build ran successfully
			IntegrationResult result = IntegrationFixture.LoadIntegrationResult(PROJECT_NAME);
			Assertion.AssertNotNull(result);
			Assertion.AssertEquals(true, result.Succeeded);
			Assertion.Assert(Int32.Parse(result.Label) >= 1);
		}

		[Test][Ignore("will now take too long")]
		public void RunIntegrationLoop_WithNoSpecifiedSchedule()
		{
			string projectXml = ConfigurationFileFixture.GenerateSimpleProjectXml(PROJECT_NAME, null);
			CruiseServer cc = IntegrationFixture.CreateCruiseControl(PROJECT_NAME, projectXml);
			IntegrationEventCounter counter = IntegrationFixture.AddIntegrationEventHandler(cc, PROJECT_NAME);
			RunLoopTest(cc, counter);
		}

		[Test][Ignore("will now take too long")]
		public void RunIntegrationLoop_WithNoStateDirectory()
		{
			string curDir = Directory.GetCurrentDirectory();
			string tempdir = TempFileUtil.CreateTempDir(PROJECT_NAME);
			Directory.SetCurrentDirectory(tempdir);
			string projectXml = ConfigurationFileFixture.GenerateSimpleProjectXml(PROJECT_NAME, null, null);
			CruiseServer cc = IntegrationFixture.CreateCruiseControl(PROJECT_NAME, projectXml);
			IntegrationEventCounter counter = IntegrationFixture.AddIntegrationEventHandler(cc, PROJECT_NAME);
			RunLoopTest(cc, counter);
			Directory.SetCurrentDirectory(curDir);
		}

		//[Test]
		public void RunIntegrationWithNoModifications()
		{
		}

		//[Test]
		public void RunIntegrationWithExceptionInSourceControl()
		{
		}

		private void HandleIntegrationExceptionEvent(object sender, CruiseControlException ex)
		{
			_integrationExceptionCalls++;
		}
	}
}
