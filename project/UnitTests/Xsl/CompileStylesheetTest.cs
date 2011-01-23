using NUnit.Framework;

namespace ThoughtWorks.CruiseControl.UnitTests.Xsl
{
	[TestFixture]
	public class CompileStylesheetTest : StylesheetTestFixture
	{
		protected override string Stylesheet
		{
			get { return @"xsl\compile.xsl"; }
		}

		[Test]
		public void ShouldRenderErrorMessageAtTheStartOfLine()
		{
			string input = @"error CS1504: Source file 'C:\Beachball\Checkout\shared\Model\beachball-schema1.cs' could not be opened ('The system cannot find the file specified. ')";

			string actualXml = LoadStylesheetAndTransformInput(CreateInfoMessage(input));
			CustomAssertion.AssertContains("error CS1504: Source file", actualXml);
		}

		[Test]
		public void ShouldNotRenderBuildCompleteMessage()
		{
			string input = @"<![CDATA[ Build complete -- 1 errors, 0 warnings
  ]]>";

			string actualXml = LoadStylesheetAndTransformInput(CreateInfoMessage(input));
			CustomAssertion.AssertNotContains("Build complete", actualXml);
		}

		[Test]
		public void ShouldNotRenderRulesErrorMessage()
		{
			string input = @"* Rules gave the following errors:";

			string actualXml = LoadStylesheetAndTransformInput(CreateInfoMessage(input));
			CustomAssertion.AssertNotContains("Rules", actualXml);
		}

		[Test]
		public void ShouldRenderNAntBuildErrorElement()
		{
			string xml = string.Format(@"<cruisecontrol><build><buildresults><failure><builderror>
	<message>External Program Failed: D:\tools\cvsexe\cvswithplinkrsh.bat (return code was 1)</message>
</builderror></failure></buildresults></build></cruisecontrol>");

			string actualXml = LoadStylesheetAndTransformInput(CreateInfoMessage(xml));
			CustomAssertion.AssertContains("External Program Failed:", actualXml);
		}

		[Test]
		public void ShouldRenderNAntInternalErrorElement()
		{
			string xml = string.Format(@"<cruisecontrol><build><buildresults><failure><internalerror>
      <type>System.IO.FileNotFoundException</type>
      <message><![CDATA[Could not load file or assembly 'nunit.core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=96d09a1eb7f44a77' or one of its dependencies. The system cannot find the file specified.]]></message>
      <stacktrace><![CDATA[   at NAnt.NUnit2.Tasks.NUnit2Task.ExecuteTask()
   at NAnt.Core.Task.Execute()
   at NAnt.Core.Target.Execute()
   at NAnt.Core.Project.Execute(String targetName, Boolean forceDependencies)
   at NAnt.Core.Project.Execute()
   at NAnt.Core.Project.Run()]]></stacktrace>
    </internalerror></failure></buildresults></build></cruisecontrol>");

			string actualXml = LoadStylesheetAndTransformInput(CreateInfoMessage(xml));
			CustomAssertion.AssertContains("Could not load file or assembly 'nunit.core", actualXml);			
		}

		private static string CreateInfoMessage(string input)
		{
			return string.Format(@"<cruisecontrol><buildresults>
	<message level=""Info"">
{0}
</message></buildresults></cruisecontrol>", input);
		}
	}
}