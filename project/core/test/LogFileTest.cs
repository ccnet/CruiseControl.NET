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

		[Test]
		public void ParseForDate()
		{
			DateTime date = new DateTime(2002, 3, 28, 13, 0, 0);
			Assert.AreEqual(date,LogFileUtil.ParseForDate("20020328130000"));
		}
		
		[Test]
		public void GetFormattedDateString()
		{
			DateTime date = new DateTime(1971, 5, 14, 15, 0, 0);
			string actual = LogFileUtil.GetFormattedDateString("log19710514150000.xml");
			string expected = DateUtil.FormatDate(date);
			Assert.AreEqual(expected, actual);
		}
		
		[Test]
		public void ParseForDateString()
		{
			CheckDateString("19741224120000", "log19741224120000.xml");
			CheckDateString("19750101120000","log19750101120000.xml");
			CheckDateString("20020830164057","log20020830164057Lbuild.6.xml");
		}
		
		[Test]
		public void GetLatestBuildNumber()
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
			Assert.AreEqual(9, actual);
		}

		[Test]
		public void GetLatestBuildNumberHandlesString()
		{
			int actual = LogFileUtil.GetLatestBuildNumber(new string[] {"log20020830164057Lbuild.v1.1.8.xml"});
			Assert.AreEqual(118, actual);
		}

		[Test]
		public void ListFiles() 
		{
			// testFilenames array must be in sorted order -- otherwise links iteration will fail
			string[] testFilenames = {"log123.xml", "log200.xml", "logfile.txt", 
										 "log20020830164057Lbuild.6.xml", "badfile.xml" };
			TempFileUtil.CreateTempFiles(TestFolder, testFilenames);

			string[] fileNames = LogFileUtil.GetLogFileNames(_tempFolder);
			Assert.AreEqual(3,fileNames.Length);
			Assert.AreEqual(testFilenames[0],fileNames[0]);
			Assert.AreEqual(testFilenames[1],fileNames[1]);
		}
		
		[Test]
		public void GetLastLogFileName()
		{
			string[] testFilenames = {"log123.xml", "log200.xml", "logfile.txt", 
										 "log20010830164057Lbuild.6.xml", 
										 "log20011230164057Lbuild.8.xml", 
										 "log20010430164057Lbuild.7.xml", 
										 "badfile.xml" };
			TempFileUtil.CreateTempFiles(TestFolder, testFilenames);
			string path = TempFileUtil.GetTempPath(TestFolder);

			string logfile = LogFileUtil.GetLatestLogFileName(path);
			Assert.AreEqual("log20011230164057Lbuild.8.xml", logfile);
		}

		[Test]
		public void GetLastLogFileName_UnknownPath()
		{
			string logfile = LogFileUtil.GetLatestLogFileName(@"c:\non\exi\stent");
			Assert.IsNull(logfile);
		}
		
		[Test]
		public void GetLastLogFileName_EmptyFolder()
		{
			string folder = TempFileUtil.CreateTempDir(TestFolder);
			Assert.IsNull(LogFileUtil.GetLatestLogFileName(folder));
		}

		[Test]
		public void GetLastBuildDate()
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
			Assert.AreEqual(expected, actual);
		}
		
		[Test]
		public void GetLastBuildDate_NoDirectory()
		{
			Assert.AreEqual(new DateTime(), LogFileUtil.GetLastBuildDate(@"c:\non\exi\stent", new DateTime()));
		}

		[Test]
		public void GetLastBuildDate_NoFiles()
		{
			string path = TempFileUtil.CreateTempDir("lastbuilddate_nofiles");
			Assert.AreEqual(new DateTime(), LogFileUtil.GetLastBuildDate(path, new DateTime()));
		}

		[Test]
		public void GetLatestBuildNumberWithMissingPath()
		{
			Assert.AreEqual(0, LogFileUtil.GetLatestBuildNumber(@"c:\non\exi\stent"));
		}
		
		private void CheckDateString(string expected, string filename)
		{
			string actual = LogFileUtil.ParseForDateString(filename);
			Assert.AreEqual(expected, actual);
		}
		
		[Test, ExpectedException(typeof(ArgumentException))]
		public void ParseForDateStringWrongPrefix()
		{
			LogFileUtil.ParseForDateString("garbage.txt");
		}
		
		[Test, ExpectedException(typeof(ArgumentException))]
		public void ParseForDateStringShortFilename()
		{
			LogFileUtil.ParseForDateString("log3.xml");
		}
		
		[Test]
		public void BuildSuccessful()
		{
			Assert.IsTrue(!LogFileUtil.IsSuccessful("log19750101120000.xml"));
			Assert.IsTrue(LogFileUtil.IsSuccessful("log20020830164057Lbuild.6.xml"));
		}
		
		[Test]
		public void CreateFileNameNoBuildNumber()
		{
			DateTime date = new DateTime(2002, 3, 28, 13, 0, 0);
			string expected = "log20020328130000.xml";
			Assert.AreEqual(expected, LogFileUtil.CreateFailedBuildLogFileName(date));
		}
		
		[Test]
		public void CreateFileNameWithBuildNumber()
		{
			DateTime date = new DateTime(2002, 3, 28, 13, 0, 0);
			string expected = "log20020328130000Lbuild.33.xml";
			Assert.AreEqual(expected, LogFileUtil.CreateSuccessfulBuildLogFileName(date, "33"));
		}

		[Test]
		public void CreateUrl()
		{
			string expected = "?log=log20020222120000Lbuild.0.xml";
			string actual = LogFileUtil.CreateUrl(CreateIntegrationResult(IntegrationStatus.Success, new DateTime(2002, 02, 22, 12, 00, 00)));
			Assert.AreEqual(expected, actual);
		}

		private IntegrationResult CreateIntegrationResult(IntegrationStatus status, DateTime lastModifiedDate)
		{
			IntegrationResult result = new IntegrationResult();
			result.StartTime = lastModifiedDate;
			result.Status = status;
			result.Label = "0";
			result.Modifications = new Modification[1];
			result.Modifications[0] = new Modification();
			result.Modifications[0].ModifiedTime = lastModifiedDate;
			return result;
		}

		[Test]
		public void CreateUrl_FailedBuild()
		{
			string expected = "?log=log20020222120000.xml";
			string actual = LogFileUtil.CreateUrl(CreateIntegrationResult(IntegrationStatus.Failure, new DateTime(2002, 02, 22, 12, 00, 00)));
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void CreateUrl_givenFilename()
		{
			string filename = "log20020222120000Lbuild.0.xml";
			string expected = "?log=log20020222120000Lbuild.0.xml";
			string actual = LogFileUtil.CreateUrl(filename);				
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void CreateUrlWithGivenFilenameAndProjectName()
		{
			string filename = "log20020222120000Lbuild.0.xml";
			string expected = "?log=log20020222120000Lbuild.0.xml&project=myproject";
			string actual = LogFileUtil.CreateUrl(filename, "myproject");				
			Assert.AreEqual(expected, actual);
		}
	}
}
