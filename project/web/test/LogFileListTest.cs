using System;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Web.Test
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
		
		public void TestGetLinks()
		{
			// testFilenames array must be in sorted order -- otherwise links iteration will fail
			string[] testFilenames = {
										 "log19741224120000.xml", "log19750101120000.xml", "log20020507010355.xml", 
										 "log20020507023858.xml", "log20020507042535.xml", "log20020830164057Lbuild.6.xml",
										 "logfile.txt", "badfile.xml" };
			TempFileUtil.CreateTempFiles(TestFolder, testFilenames);

			HtmlAnchor[] links = LogFileLister.GetLinks(_tempFolder);
			AssertEquals(6,links.Length);

			for (int i = 0; i < links.Length; i++)
			{
				AssertEquals(LogFileUtil.CreateUrl(testFilenames[5-i]), links[i].HRef);
				string expected = LogFileLister.GetDisplayLabel(testFilenames[5-i]);
				Assert(links[i].InnerText.StartsWith(expected));
			}
		}
		
		public void TestGetBuildStatus()
		{
			CheckBuildStatus("(Failed)", "log19750101120000.xml");
			CheckBuildStatus("(62)","log20020830164057Lbuild.62.xml");
		}
		
		private void CheckBuildStatus(string expected, string input)
		{
			AssertEquals(expected, LogFileLister.GetBuildStatus(input));
		}

		public void TestParseDate()
		{
			DateTime date = new DateTime(2002, 3, 28, 13, 0, 0);
			AssertEquals(date,LogFileUtil.ParseForDate("20020328130000"));
		}

		public void TestGetCurrentFilename()
		{
			// testFilenames array must be in sorted order -- otherwise links iteration will fail
			string[] testFilenames = {
										 "log19741224120000.xml", "log19750101120000.xml", "log20020507010355.xml", 
										 "log20020507023858.xml", "log20030507042535.xml", "logfile.txt", "badfile.xml" };
			TempFileUtil.CreateTempFiles(TestFolder, testFilenames);

			AssertEquals("log20030507042535.xml", LogFileLister.GetCurrentFilename(new DirectoryInfo(_tempFolder)));
		}
		
		public void TestTransform()
		{
			string logfile = TempFileUtil.CreateTempXmlFile(TestFolder, "samplelog.xml", TestData.LogFileContents);
			string xslfile = TempFileUtil.CreateTempXmlFile(TestFolder, "samplestylesheet.xsl", TestData.StyleSheetContents);

			string output = LogFileLister.Transform(logfile, xslfile);
			AssertNotNull(output);
			Assert("Transform returned no data", ! String.Empty.Equals(output));
		}

		[ExpectedException(typeof(CruiseControlException))]
		public void TestTransform_LogfileMissing()
		{
			string logfile = "nosuchlogfile";
			string xslfile = XslFileGood;				
			string output = LogFileLister.Transform(logfile, xslfile);
		}

		[ExpectedException(typeof(CruiseControlException))]
		public void TestTransform_logfileBadFormat()
		{
			string logfile = LogFileBadFormat;
			string xslfile = XslFileGood;
			LogFileLister.Transform(logfile, xslfile);
		}

		[ExpectedException(typeof(CruiseControlException))]
		public void TestTransform_stylesheetMissing()
		{
			string logfile = LogFileGood;
			string xslfile = "nosuchstylefile";
			LogFileLister.Transform(logfile, xslfile);			
		}

		[ExpectedException(typeof(CruiseControlException))]
		public void TestTransform_stylesheetBadFormat()
		{
			string logfile = LogFileGood;
			string xslfile = XslFileBadFormat;
			LogFileLister.Transform(logfile, xslfile);			
		}

		public void TestInitAdjacentAnchors_NoLogFiles()
		{
			HtmlAnchor previous = new HtmlAnchor();
			HtmlAnchor next = new HtmlAnchor();
			LogFileLister.InitAdjacentAnchors(previous, next, _tempFolder, null);
			AssertEquals("Previous link set", String.Empty, previous.HRef);
			AssertEquals("Next link set", String.Empty, next.HRef);
		}

		public void TestInitAdjacentAnchors_OneLogFile()
		{
			HtmlAnchor previous = new HtmlAnchor();
			HtmlAnchor next = new HtmlAnchor();
			TempFileUtil.CreateTempFile(_tempFolder, LogFileUtil.CreateSuccessfulBuildLogFileName(new DateTime(), "2"));
			LogFileLister.InitAdjacentAnchors(new HtmlAnchor(), new HtmlAnchor(), 
				_tempFolder, null);
			AssertEquals("Previous link set", String.Empty, previous.HRef);
			AssertEquals("Next link set", String.Empty, next.HRef);
		}

		private string LogFileGood
		{
			get 
			{
				return TempFileUtil.CreateTempXmlFile(
					  TestFolder, "samplelog.xml", TestData.LogFileContents); }
		}

		private string LogFileBadFormat
		{
			get 
			{
				return  TempFileUtil.CreateTempXmlFile(
					  TestFolder, "samplelog.xsl", @"<i am so bad it's almost good & so is my friend"); }
		}

		private string XslFileGood
		{
			get 
			{
				return TempFileUtil.CreateTempXmlFile(
					  TestFolder, "samplestylesheet.xsl", TestData.StyleSheetContents); }
		}

		private string XslFileBadFormat
		{
			get 
			{
				return  TempFileUtil.CreateTempXmlFile(
					  TestFolder, "samplestylesheet.xsl", @"<xsl:i am so bad it hurts"); }
		}
	}
}