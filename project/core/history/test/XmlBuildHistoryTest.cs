using System;
using System.IO;
using NUnit.Framework;
using tw.ccnet.core.util;
using tw.ccnet.core.test;
using tw.ccnet.remote;

namespace tw.ccnet.core.history.test
{
	[TestFixture]
	public class XmlBuildHistoryTest
	{
		private const string TEMP = "buildhistory";
		private string _tempDir;
		XmlBuildHistory _history;
		IntegrationResult _result;

		[SetUp]
		protected void SetUp()
		{
			_history = new XmlBuildHistory();
			_result = IntegrationResultFixture.CreateIntegrationResult();
			_tempDir = TempFileUtil.CreateTempDir(TEMP);
			_history.LogDir = _tempDir;
		}

		[TearDown]
		protected void TearDown()
		{
			TempFileUtil.DeleteTempDir(_tempDir);
		}

		public void TestWriteIntegrationResult_VerifyFileExists()
		{
			string filename = _history.GetFilename(_result);
			Assertion.Assert("file exists, but it shouldn't", ! TempFileUtil.TempFileExists(TEMP, filename));
			_history.Save(_result);
			Assertion.Assert("file does not exists!", TempFileUtil.TempFileExists(TEMP, filename));
		}

		[ExpectedException(typeof(CruiseControlException))]
		public void TestWriteIntegrationResult_InvalidLogDir()
		{
			_history.LogDir = @"q:\blah";
			_history.Save(_result);
		}

		public void TestGetFilename_SuccessfulBuild()
		{
			_result.StartTime = new DateTime(2002, 12, 12);
			_result.Status = IntegrationStatus.Success;
			_result.Label = "12";
			Assertion.AssertEquals("log20021212000000Lbuild.12.xml", _history.GetFilename(_result));
		}

		public void TestGetFilename_FailedBuild()
		{
			_result.StartTime = new DateTime(2002, 12, 12);
			_result.Status = IntegrationStatus.Failure;
			_result.Label = "12";
			Assertion.AssertEquals("log20021212000000.xml", _history.GetFilename(_result));
		}

		[Test]
		public void GetFilePath_NullLogDir()
		{
			_history.LogDir = null;
			string path = _history.GetFilePath(_result);
			Assertion.AssertEquals(Path.Combine(Directory.GetCurrentDirectory(), _history.GetFilename(_result)), path);
			Assertion.AssertEquals(null, _history.Load());
		}

		public void TestSerializeIntegrationResult()
		{
			StringWriter writer = new StringWriter();
			_history.SerializeIntegrationResult(writer, _result);
			
			StringReader reader = new StringReader(writer.ToString());
			IntegrationResult actual = _history.DeserializeIntegrationResult(reader);
			Assertion.AssertEquals(_result, actual);
		}

		public void TestLoadIntegrationResult()
		{
			Assertion.Assert("file exists, but it shouldn't", ! TempFileUtil.TempFileExists(TEMP, _history.GetFilename(_result)));
			_history.Save(_result);
			IntegrationResult actual = _history.Load(_history.GetFilePath(_result));
			Assertion.AssertEquals(_result, actual);
		}

		public void TestLoad()
		{
			SaveIntegrationResult(new DateTime(2002,12,12));
			SaveIntegrationResult(new DateTime(2003,1,1));
			SaveIntegrationResult(new DateTime(1999,10,10));
			TempFileUtil.CreateTempFile(TEMP, "log999999000000.xma");
			TempFileUtil.CreateTempFile(TEMP, "z999999000000.xml");

			Assertion.AssertEquals(true, _history.Exists());
			IntegrationResult result = _history.Load();
			Assertion.AssertNotNull("no build result loaded", result);
			Assertion.AssertEquals(new DateTime(2003,1,1), result.StartTime);
		}

		public void TestLoad_NoBuildsExist()
		{
			Assertion.AssertEquals(false, _history.Exists());
			Assertion.AssertNull(_history.Load());
		}

		private void SaveIntegrationResult(DateTime buildDate)
		{
			IntegrationResult result = IntegrationResultFixture.CreateIntegrationResult();
			result.StartTime = buildDate;
			_history.Save(result);
		}
	}
}
