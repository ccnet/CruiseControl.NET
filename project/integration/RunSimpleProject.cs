using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using NUnit.Framework;
using tw.ccnet.core;
using tw.ccnet.core.configuration;
using tw.ccnet.core.util;
using tw.ccnet.remote;
using tw.ccnet.core.schedule;

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
			CruiseControl cc = IntegrationFixture.CreateCruiseControl(PROJECT_NAME, projectXml);
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
			CruiseControl cc = IntegrationFixture.CreateCruiseControl(PROJECT_NAME, projectXml);
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
			CruiseControl cc = IntegrationFixture.CreateCruiseControl(PROJECT_NAME, projectXml);
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
			CruiseControl cc = IntegrationFixture.CreateCruiseControl(PROJECT_NAME, projectXml);
			IntegrationEventCounter counter = IntegrationFixture.AddIntegrationEventHandler(cc, PROJECT_NAME);
			RunLoopTest(cc, counter);
		}

		private void RunLoopTest(CruiseControl cc, IntegrationEventCounter counter)
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

		[Test]
		public void RunIntegrationLoop_WithNoSpecifiedSchedule()
		{
			string projectXml = ConfigurationFileFixture.GenerateSimpleProjectXml(PROJECT_NAME, null);
			CruiseControl cc = IntegrationFixture.CreateCruiseControl(PROJECT_NAME, projectXml);
			IntegrationEventCounter counter = IntegrationFixture.AddIntegrationEventHandler(cc, PROJECT_NAME);
			RunLoopTest(cc, counter);
		}

		[Test]
		public void RunIntegrationLoop_WithNoStateDirectory()
		{
			string curDir = Directory.GetCurrentDirectory();
			string tempdir = TempFileUtil.CreateTempDir(PROJECT_NAME);
			Directory.SetCurrentDirectory(tempdir);
			string projectXml = ConfigurationFileFixture.GenerateSimpleProjectXml(PROJECT_NAME, null, null);
			CruiseControl cc = IntegrationFixture.CreateCruiseControl(PROJECT_NAME, projectXml);
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
