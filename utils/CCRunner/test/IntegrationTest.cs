using System;
using System.IO;
using NUnit.Framework;
using tw.ccnet.core;
using tw.ccnet.core.configuration.test;
using tw.ccnet.core.schedule;
using tw.ccnet.core.util;

namespace CCNet.CCRunner.test
{
	[TestFixture]
	public class IntegrationTest : Assertion
	{
		const string TEMP_DIR = "CCRunner";
		const string TEMP_FILE = "ccnet.config";
		const string PROJECT_NAME = "test";

		[SetUp]
		protected void SetUp()
		{
			TempFileUtil.CreateTempDir(TEMP_DIR);
		}

		[TearDown]
		protected void TearDown()
		{
			TempFileUtil.DeleteTempDir(TEMP_DIR);
		}

		[Test]
		public void ForceBuildUsingRunner()
		{
			AssertNumberOfFiles(0);

			CruiseManager cc = CreateCCServer();
			try
			{
				cc.StartCruiseControl();
				cc.RegisterForRemoting();
				AssertNumberOfFiles(1);

				Runner runner = new Runner();
				runner.Url = "tcp://localhost:1234/MockCruise.rem";
				runner.Run(PROJECT_NAME);
			}
			finally
			{
				cc.StopCruiseControl();
			}
			AssertNumberOfFiles(2);
		}

		private void AssertNumberOfFiles(int expectedFiles)
		{
			AssertEquals(expectedFiles, Directory.GetFiles(TempFileUtil.GetTempPath(TEMP_DIR)).Length);
		}

		protected CruiseManager CreateCCServer()
		{
			string configData = ConfigurationFixture.GenerateConfigXml(GenerateProjectXml());
			string tempFile = TempFileUtil.CreateTempFile(TEMP_DIR, TEMP_FILE, configData);
			CruiseManager cc = new CruiseManager(tempFile);
			return cc;
		}

		private string GenerateProjectXml()
		{
			string buildXml = ConfigurationFixture.GenerateMockBuildXml();
			string sourceControlXml = ConfigurationFixture.GenerateDefaultSourceControlXml();
//			string scheduleXml = ConfigurationFixture.GenerateScheduleXml(Schedule.Infinite);
			string historyXml = ConfigurationFixture.GenerateStateManagerXml(TempFileUtil.GetTempPath(TEMP_DIR));
			return ConfigurationFixture.GenerateProjectXml(PROJECT_NAME, buildXml, sourceControlXml, null, null, historyXml);
		}
	}
}
