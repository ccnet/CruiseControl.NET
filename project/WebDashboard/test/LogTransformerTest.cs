using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.WebDashboard.Test;

namespace ThoughtWorks.CruiseControl.WebDashboard.Test
{
	[TestFixture]
	public class LogTransformerTest : CustomAssertion
	{
		private static readonly string TestFolder = "logTransformerTest";
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
		public void TestTransform()
		{
			string logfile = TempFileUtil.CreateTempXmlFile(TestFolder, "samplelog.xml", TestData.LogFileContents);
			string xslfile = TempFileUtil.CreateTempXmlFile(TestFolder, "samplestylesheet.xsl", TestData.StyleSheetContents);

			string output = new LogTransformer(logfile, xslfile).Transform();
			AssertNotNull(output);
			Assert("Transform returned no data", ! String.Empty.Equals(output));
		}

		[ExpectedException(typeof(CruiseControlException))]
		public void TestTransform_LogfileMissing()
		{
			string logfile = "nosuchlogfile";
			string xslfile = XslFileGood;				
			new LogTransformer(logfile, xslfile).Transform();
		}

		[ExpectedException(typeof(CruiseControlException))]
		public void TestTransform_logfileBadFormat()
		{
			string logfile = LogFileBadFormat;
			string xslfile = XslFileGood;
			new LogTransformer(logfile, xslfile).Transform();
		}

		[ExpectedException(typeof(CruiseControlException))]
		public void TestTransform_stylesheetMissing()
		{
			string logfile = LogFileGood;
			string xslfile = "nosuchstylefile";
			new LogTransformer(logfile, xslfile).Transform();			
		}

		[ExpectedException(typeof(CruiseControlException))]
		public void TestTransform_stylesheetBadFormat()
		{
			string logfile = LogFileGood;
			string xslfile = XslFileBadFormat;
			new LogTransformer(logfile, xslfile).Transform();			
		}	


		private string LogFileGood
		{
			get 
			{
				return TempFileUtil.CreateTempXmlFile(
					TestFolder, "samplelog.xml", TestData.LogFileContents);
			}
		}

		private string LogFileBadFormat
		{
			get 
			{
				return  TempFileUtil.CreateTempXmlFile(
					TestFolder, "samplelog.xsl", @"<i am so bad it's almost good & so is my friend"); 
			}
		}

		private string XslFileGood
		{
			get 
			{
				return TempFileUtil.CreateTempXmlFile(
					TestFolder, "samplestylesheet.xsl", TestData.StyleSheetContents); 
			}
		}

		private string XslFileBadFormat
		{
			get 
			{
				return  TempFileUtil.CreateTempXmlFile(
					TestFolder, "samplestylesheet.xsl", @"<xsl:i am so bad it hurts"); 
			}
		}

	}
}
