using System;

using NUnit.Framework;

using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Test
{
	[TestFixture]
	public class LogFileTest : CustomAssertion
	{
		private static readonly string TestFolder = "logfilelist";
		private string _tempFolder;

		[SetUp]
		public void Setup()
		{
			_tempFolder = TempFileUtil.CreateTempDir(TestFolder);
		}

		[TearDown]
		public void Teardown()
		{
			TempFileUtil.DeleteTempDir(TestFolder);
		}

		public void TestParseForDate()
		{
			DateTime date = new DateTime(2002, 3, 28, 13, 0, 0);
			AssertEquals(date,LogFileUtil.ParseForDate("20020328130000"));
		}
		
		public void TestGetFormattedDateString()
		{
			DateTime date = new DateTime(1971, 5, 14, 15, 0, 0);
			string actual = LogFileUtil.GetFormattedDateString("log19710514150000.xml");
			string expected = DateUtil.FormatDate(date);
			AssertEquals(expected, actual);
		}
		
		public void TestParseForDateString()
		{
			CheckDateString("19741224120000", "log19741224120000.xml");
			CheckDateString("19750101120000","log19750101120000.xml");
			CheckDateString("20020830164057","log20020830164057Lbuild.6.xml");
		}
		
		public void TestGetLatestBuildNumber()
		{
			string[] filenames = new string[]{
												 "log19710514150000.xml",
												 "log19710514150001.xml",
												 "log20020830164057Lbuild.9.xml",
												 "log19710514150002.xml",
												 "log20020830164057Lbuild.7.xml",
												 "log19710514150003.xml",
												 "log20020830164057Lbuild.6.xml",
												 "log20020830164057Lbuild.8.xml",
												 "log19710514150004.xml"
											 };
			int actual = LogFileUtil.GetLatestBuildNumber(filenames);
			AssertEquals(9, actual);
		}

		public void TestGetLatestBuildNumberHandlesString()
		{
			int actual = LogFileUtil.GetLatestBuildNumber(new string[] {"log20020830164057Lbuild.v1.1.8.xml"});
			AssertEquals(118, actual);
		}

		public void TestListFiles() 
		{
			// testFilenames array must be in sorted order -- otherwise links iteration will fail
			string[] testFilenames = {"log123.xml", "log200.xml", "logfile.txt", 
										 "log20020830164057Lbuild.6.xml", "badfile.xml" };
			TempFileUtil.CreateTempFiles(TestFolder, testFilenames);

			string[] fileNames = LogFileUtil.GetLogFileNames(_tempFolder);
			AssertEquals(3,fileNames.Length);
			AssertEquals(testFilenames[0],fileNames[0]);
			AssertEquals(testFilenames[1],fileNames[1]);
		}
		
		public void TestGetLastLogFileName()
		{
			string[] testFilenames = {"log123.xml", "log200.xml", "logfile.txt", 
										 "log20010830164057Lbuild.6.xml", 
										 "log20011230164057Lbuild.8.xml", 
										 "log20010430164057Lbuild.7.xml", 
										 "badfile.xml" };
			TempFileUtil.CreateTempFiles(TestFolder, testFilenames);
			string path = TempFileUtil.GetTempPath(TestFolder);

			string logfile = LogFileUtil.GetLatestLogFileName(path);
			AssertEquals("log20011230164057Lbuild.8.xml", logfile);
		}

		public void TestGetLastLogFileName_UnknownPath()
		{
			string logfile = LogFileUtil.GetLatestLogFileName(@"c:\non\exi\stent");
			AssertNull(logfile);
		}
		
		public void TestGetLastLogFileName_EmptyFolder()
		{
			string folder = TempFileUtil.CreateTempDir(TestFolder);
			AssertNull(LogFileUtil.GetLatestLogFileName(folder));
		}

		public void TestGetLastBuildDate()
		{
			string[] testFilenames = {"log123.xml", "log200.xml", "logfile.txt", 
										 "log20010830164057Lbuild.6.xml", 
										 "log20011230164057Lbuild.6.xml", 
										 "log20010430164057Lbuild.6.xml", 
										 "badfile.xml" };
			TempFileUtil.CreateTempFiles(TestFolder, testFilenames);
			DateTime expected = new DateTime(2001,12,30,16,40,57);
			string path = TempFileUtil.GetTempPath(TestFolder);
			DateTime actual = LogFileUtil.GetLastBuildDate(path, new DateTime());
			AssertEquals("path: "+path,expected, actual);
		}
		
		public void TestGetLastBuildDate_NoDirectory()
		{
			AssertEquals(new DateTime(), LogFileUtil.GetLastBuildDate(@"c:\non\exi\stent", new DateTime()));
		}

		public void TestGetLastBuildDate_NoFiles()
		{
			string path = TempFileUtil.CreateTempDir("lastbuilddate_nofiles");
			AssertEquals(new DateTime(), LogFileUtil.GetLastBuildDate(path, new DateTime()));
		}

		public void TestGetLatestBuildNumberWithMissingPath()
		{
			AssertEquals(0, LogFileUtil.GetLatestBuildNumber(@"c:\non\exi\stent"));
		}
		
		private void CheckDateString(string expected, string filename)
		{
			string actual = LogFileUtil.ParseForDateString(filename);
			AssertEquals(expected, actual);
		}
		
		[ExpectedException(typeof(ArgumentException))]
		public void TestParseForDateStringWrongPrefix()
		{
			LogFileUtil.ParseForDateString("garbage.txt");
		}
		
		[ExpectedException(typeof(ArgumentException))]
		public void TestParseForDateStringShortFilename()
		{
			LogFileUtil.ParseForDateString("log3.xml");
		}
		
		public void TestBuildSuccessful()
		{
			Assert("expected false",!LogFileUtil.IsSuccessful("log19750101120000.xml"));
			Assert("expected true",LogFileUtil.IsSuccessful("log20020830164057Lbuild.6.xml"));
		}
		
		public void TestCreateFileNameNoBuildNumber()
		{
			DateTime date = new DateTime(2002, 3, 28, 13, 0, 0);
			string expected = "log20020328130000.xml";
			AssertEquals(expected, LogFileUtil.CreateFailedBuildLogFileName(date));
		}
		
		public void TestCreateFileNameWithBuildNumber()
		{
			DateTime date = new DateTime(2002, 3, 28, 13, 0, 0);
			string expected = "log20020328130000Lbuild.33.xml";
			AssertEquals(expected, LogFileUtil.CreateSuccessfulBuildLogFileName(date, "33"));
		}

		public void TestCreateUrl()
		{
			string expected = "?log=log19800101000000Lbuild.0.xml";
			string actual = LogFileUtil.CreateUrl(CreateIntegrationResult(IntegrationStatus.Success, new DateTime(2002, 02, 22, 12, 00, 00)));
			AssertEquals(expected, actual);
		}

		private IntegrationResult CreateIntegrationResult(IntegrationStatus status, DateTime lastModifiedDate)
		{
			IntegrationResult result = new IntegrationResult();
			result.Status = status;
			result.Label = "0";
			result.Modifications = new Modification[1];
			result.Modifications[0] = new Modification();
			result.Modifications[0].ModifiedTime = lastModifiedDate;
			return result;
		}

		public void TestCreateUrl_FailedBuild()
		{
			string expected = "?log=log19800101000000.xml";
			string actual = LogFileUtil.CreateUrl(CreateIntegrationResult(IntegrationStatus.Failure, new DateTime(2002, 02, 22, 12, 00, 00)));
			AssertEquals(expected, actual);
		}

		public void TestCreateUrl_givenFilename()
		{
			string filename = "log20020222120000Lbuild.0.xml";
			string expected = "?log=log20020222120000Lbuild.0.xml";
			string actual = LogFileUtil.CreateUrl(filename);				
			AssertEquals(expected, actual);
		}

		[Test]
		public void TestCreateUrlWithGivenFilenameAndProjectName()
		{
			string filename = "log20020222120000Lbuild.0.xml";
			string expected = "?log=log20020222120000Lbuild.0.xml&project=myproject";
			string actual = LogFileUtil.CreateUrl(filename, "myproject");				
			AssertEquals(expected, actual);
		}
	}
}
