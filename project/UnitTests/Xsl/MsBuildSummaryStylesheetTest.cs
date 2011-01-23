using System;
using NUnit.Framework;

namespace ThoughtWorks.CruiseControl.UnitTests.Xsl
{
	[TestFixture]
	public class MsBuildSummaryStylesheetTest : StylesheetTestFixture
	{
		protected override string Stylesheet
		{
			get { return @"xsl\compile-msbuild.xsl"; }
		}

		[Test]
		public void ShouldNotRenderAnyOutputIfRootNodeIsMissing()
		{
			string xml = WrapInBuildResultsElement("<foo>bar</foo>");
			string actualXml = LoadStylesheetAndTransformInput(xml);
			Assert.AreEqual("", actualXml);
		}

		[Test]
		public void ShouldRenderWarnings()
		{
			string xml = WrapInBuildResultsElement(@"<msbuild><warning file=""c:\temp\foo.txt"" line=""10"" column=""23"">Invalid behaviour.</warning></msbuild>");
			string actualXml = LoadStylesheetAndTransformInput(xml);
			CustomAssertion.AssertContains(@"c:\temp\foo.txt", actualXml);
			CustomAssertion.AssertContains("(10,23)", actualXml);
			CustomAssertion.AssertContains("Invalid behaviour.", actualXml);
		}

		[Test]
		public void ShouldRenderErrors()
		{
			string xml = WrapInBuildResultsElement(@"<msbuild><error file=""c:\temp\foo.txt"" line=""10"" column=""23"">Invalid behaviour.</error></msbuild>");
			string actualXml = LoadStylesheetAndTransformInput(xml);
			CustomAssertion.AssertContains(@"c:\temp\foo.txt", actualXml);
			CustomAssertion.AssertContains("(10,23)", actualXml);
			CustomAssertion.AssertContains("Invalid behaviour.", actualXml);
		}

	}
}