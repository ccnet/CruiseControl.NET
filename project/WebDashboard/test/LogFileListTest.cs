using System;
using System.IO;
using System.Web.UI.HtmlControls;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.WebDashboard.Test
{
	[TestFixture]
	public class LogFileListTest : CustomAssertion
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
		public void GetLinks()
		{
			// testFilenames array must be in sorted order -- otherwise links iteration will fail
			string[] testFilenames = {
				"log19741224120000.xml",
				"log19750101120000.xml",
				"log20020507010355.xml",
				"log20020507023858.xml",
				"log20020507042535.xml",
				"log20020830164057Lbuild.6.xml",
				"logfile.txt",
				"badfile.xml"
			};
			TempFileUtil.CreateTempFiles(TestFolder, testFilenames);

			HtmlAnchor[] actualLinks = LogFileLister.GetLinks(_tempFolder);
			AssertEquals(6, actualLinks.Length);

			// expected Date format: dd MMM yyyy HH:mm
			AssertEquals("?log=log20020830164057Lbuild.6.xml", actualLinks[0].HRef);
			AssertEquals("?log=log20020507042535.xml", actualLinks[1].HRef);
			AssertEquals("?log=log20020507023858.xml", actualLinks[2].HRef);
			AssertEquals("?log=log20020507010355.xml", actualLinks[3].HRef);
			AssertEquals("?log=log19750101120000.xml", actualLinks[4].HRef);
			AssertEquals("?log=log19741224120000.xml", actualLinks[5].HRef);

			AssertEquals("<nobr>30 Aug 2002 16:40 (6)</nobr>", actualLinks[0].InnerText);
			AssertEquals("<nobr>07 May 2002 04:25 (Failed)</nobr>", actualLinks[1].InnerHtml);
			AssertEquals("<nobr>07 May 2002 02:38 (Failed)</nobr>", actualLinks[2].InnerHtml);
			AssertEquals("<nobr>07 May 2002 01:03 (Failed)</nobr>", actualLinks[3].InnerHtml);
			AssertEquals("<nobr>01 Jan 1975 12:00 (Failed)</nobr>", actualLinks[4].InnerHtml);
			AssertEquals("<nobr>24 Dec 1974 12:00 (Failed)</nobr>", actualLinks[5].InnerHtml);
		}

		[Test]
		public void GetBuildStatus()
		{
			CheckBuildStatus("(Failed)", "log19750101120000.xml");
			CheckBuildStatus("(62)", "log20020830164057Lbuild.62.xml");
		}

		private void CheckBuildStatus(string expected, string input)
		{
			AssertEquals(expected, LogFileLister.GetBuildStatus(input));
		}

		[Test]
		public void ParseDate()
		{
			DateTime date = new DateTime(2002, 3, 28, 13, 0, 0);
			AssertEquals(date, LogFileUtil.ParseForDate("20020328130000"));
		}

		[Test]
		public void GetCurrentFilename()
		{
			// testFilenames array must be in sorted order -- otherwise links iteration will fail
			string[] testFilenames = {
				"log19741224120000.xml",
				"log19750101120000.xml",
				"log20020507010355.xml",
				"log20020507023858.xml",
				"log20030507042535.xml",
				"logfile.txt",
				"badfile.xml"
			};
			TempFileUtil.CreateTempFiles(TestFolder, testFilenames);

			AssertEquals("log20030507042535.xml", LogFileLister.GetCurrentFilename(new DirectoryInfo(_tempFolder)));
		}

		[Test]
		public void InitAdjacentAnchors_NoLogFiles()
		{
			HtmlAnchor previous = new HtmlAnchor();
			HtmlAnchor next = new HtmlAnchor();
			LogFileLister.InitAdjacentAnchors(previous, next, _tempFolder, null);
			AssertEquals("Previous link set", String.Empty, previous.HRef);
			AssertEquals("Next link set", String.Empty, next.HRef);
		}

		[Test]
		public void InitAdjacentAnchors_OneLogFile()
		{
			HtmlAnchor previous = new HtmlAnchor();
			HtmlAnchor next = new HtmlAnchor();
			TempFileUtil.CreateTempFile(_tempFolder, LogFileUtil.CreateSuccessfulBuildLogFileName(new DateTime(), "2"));
			LogFileLister.InitAdjacentAnchors(new HtmlAnchor(), new HtmlAnchor(), _tempFolder, null);
			AssertEquals("Previous link set", String.Empty, previous.HRef);
			AssertEquals("Next link set", String.Empty, next.HRef);
		}

	}
}