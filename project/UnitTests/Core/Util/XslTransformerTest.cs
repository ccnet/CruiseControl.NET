using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Web.Test;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Util
{
	[TestFixture]
	public class XslTransformerTest : CustomAssertion
	{
		private static readonly string TestFolder = "logTransformerTest";

		[SetUp]
		public void Setup()
		{
			TempFileUtil.CreateTempDir(TestFolder);
		}

		[TearDown]
		public void Teardown()
		{
			TempFileUtil.DeleteTempDir(TestFolder);
		}

		// Todo - more thorough testing here
		[Test]
		public void ShouldTransformData()
		{
			string input = TestData.LogFileContents;
			string xslfile = TempFileUtil.CreateTempXmlFile(TestFolder, "samplestylesheet.xsl", TestData.StyleSheetContents);

			string output = new XslTransformer().Transform(input, xslfile);
			Assert.IsNotNull(output);
			Assert.IsTrue(! String.Empty.Equals(output), "Transform returned no data");
		}

		[ExpectedException(typeof(CruiseControlException))]
		[Test]
		public void ShouldFailWhenInputInvalid()
		{
			string input = @"<This is some invalid xml";
			string xslfile = TempFileUtil.CreateTempXmlFile(TestFolder, "samplestylesheet.xsl", TestData.StyleSheetContents);

			new XslTransformer().Transform(input, xslfile);
		}

		[ExpectedException(typeof(CruiseControlException))]
		[Test]
		public void ShouldFailWhenXslFileMissing()
		{
			string logfile = TestData.LogFileContents;
			string xslfile = "nosuchstylefile";

			new XslTransformer().Transform(logfile, xslfile);			
		}

		[ExpectedException(typeof(CruiseControlException))]
		[Test]
		public void ShouldFailWhenXslInvalid()
		{
			string logfile = TestData.LogFileContents;;
			string xslfile = XslFileBadFormat;
			new XslTransformer().Transform(logfile, xslfile);			
		}	

		private string XslFileBadFormat
		{
			get 
			{
				return  TempFileUtil.CreateTempXmlFile(
					TestFolder, "samplestylesheet.xsl", @"<xsl:this is some bad xsl"); 
			}
		}
	}
}
