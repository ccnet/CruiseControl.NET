using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
	[TestFixture]
	public class LogFileTest : CustomAssertion
	{
		[Test]
		public void ParseDateFromFilename()
		{
			DateTime date = new DateTime(2002, 3, 28, 13, 0, 0);
			Assert.AreEqual(date, new LogFile("log20020328130000.xml").Date);
		}
		
		[Test]
		public void VerifyFormattedDateString()
		{
			DateTime date = new DateTime(1971, 5, 14, 15, 0, 0);
			string actual = new LogFile("log19710514150000.xml").FormattedDateString;
			string expected = DateUtil.FormatDate(date);
			Assert.AreEqual(expected, actual);
		}
		
		[Test]
		public void ParseForFileFormattedDateString()
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
			
			using (TempDirectory tempPath = new TempDirectory())
			{
				CreateTempFiles(tempPath, testFilenames);
				string[] fileNames = LogFileUtil.GetLogFileNames(tempPath.ToString());
                Array.Sort(fileNames);
				Assert.AreEqual(3,fileNames.Length);
				Assert.AreEqual(testFilenames[0],fileNames[0]);
				Assert.AreEqual(testFilenames[1],fileNames[1]);
			}
		}
		
		[Test]
		public void GetLastLogFileName()
		{
			string[] testFilenames = {"log123.xml", "log200.xml", "logfile.txt", 
										 "log20010830164057Lbuild.6.xml", 
										 "log20011230164057Lbuild.8.xml", 
										 "log20010430164057Lbuild.7.xml", 
										 "badfile.xml" };
			using (TempDirectory tempPath = new TempDirectory())
			{
				CreateTempFiles(tempPath, testFilenames);
				string logfile = LogFileUtil.GetLatestLogFileName(tempPath.ToString());
				Assert.AreEqual("log20011230164057Lbuild.8.xml", logfile);
			}
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
			using (TempDirectory tempDir = new TempDirectory())
			{
				Assert.IsNull(LogFileUtil.GetLatestLogFileName(tempDir.ToString()));
			}
		}

		[Test]
		public void GetLastBuildDate()
		{
			string[] testFilenames = {"log123.xml", "log200.xml", "logfile.txt", 
										 "log20010830164057Lbuild.6.xml", 
										 "log20011230164057Lbuild.6.xml", 
										 "log20010430164057Lbuild.6.xml", 
										 "badfile.xml" };
			using (TempDirectory tempPath = new TempDirectory())
			{
				CreateTempFiles(tempPath, testFilenames);
				DateTime expected = new DateTime(2001,12,30,16,40,57);
				DateTime actual = LogFileUtil.GetLastBuildDate(tempPath.ToString(), new DateTime());
				Assert.AreEqual(expected, actual);
			}
		}

		private void CreateTempFiles(SystemPath path, string[] filenames)
		{
			foreach (string filename in filenames)
			{
				path.CreateEmptyFile(filename);
			}
		}

		[Test]
		public void GetLastBuildDate_NoDirectory()
		{
			Assert.AreEqual(new DateTime(), LogFileUtil.GetLastBuildDate(@"c:\non\exi\stent", new DateTime()));
		}

		[Test]
		public void GetLastBuildDate_NoFiles()
		{
			using (TempDirectory tempDir = new TempDirectory())
			{				
				Assert.AreEqual(new DateTime(), LogFileUtil.GetLastBuildDate(tempDir.ToString(), new DateTime()));
			}
		}

		[Test]
		public void GetLatestBuildNumberWithMissingPath()
		{
			Assert.AreEqual(0, LogFileUtil.GetLatestBuildNumber(@"c:\non\exi\stent"));
		}
		
		private void CheckDateString(string expected, string filename)
		{
			string actual = new LogFile(filename).FilenameFormattedDateString;
			Assert.AreEqual(expected, actual);
		}
		
		[Test]
		public void AttemptToCreateLogFileForFilenameWithWrongPrefix()
		{
            Assert.That(delegate { new LogFile("garbage.txt"); },
                        Throws.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("filename"));
		}
		
		[Test]
		public void AttemptToCreateLogFileForFilenameWithoutDate()
		{
			Assert.That(delegate { new LogFile("log3.xml"); },
                        Throws.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("filename"));
		}
		
		[Test]
		public void BuildSuccessful()
		{
			Assert.IsTrue(!new LogFile("log19750101120000.xml").Succeeded);
			Assert.IsTrue(new LogFile("log20020830164057Lbuild.6.xml").Succeeded);
		}
		
		[Test]
		public void CreateFileNameNoBuildNumber()
		{
			DateTime date = new DateTime(2002, 3, 28, 13, 0, 0);
			string expected = "log20020328130000.xml";
			IntegrationResult result = new IntegrationResult();
			result.StartTime = date;
			Assert.AreEqual(expected, new LogFile(result).Filename);
		}
		
		[Test]
		public void CreateFileNameWithBuildNumber()
		{
			DateTime date = new DateTime(2002, 3, 28, 13, 0, 0);
			string expected = "log20020328130000Lbuild.33.xml";
			IntegrationResult result = new IntegrationResult();
			result.StartTime = date;
			result.Label = "33";
			result.Status = IntegrationStatus.Success;
			Assert.AreEqual(expected, new LogFile(result).Filename);
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
