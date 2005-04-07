using NUnit.Framework;

namespace ThoughtWorks.CruiseControl.UnitTests.Xsl
{
	[TestFixture]
	public class HeaderStylesheetTest : StylesheetTestFixture
	{
		protected override string Stylesheet
		{
			get { return @"xsl\header.xsl"; }
		}

		[Test]
		public void ShouldOutputBuildConditionForForcedBuilds()
		{
			string input = @"<cruisecontrol><build buildcondition=""ForceBuild""/></cruisecontrol>";

			string actualXml = LoadStylesheetAndTransformInput(input);
			CustomAssertion.AssertContains("Forced Build", actualXml);
		}

		[Test]
		public void ShouldOutputBuildConditionForModificationsDetected()
		{
			string input = @"<cruisecontrol><build buildcondition=""IfModificationExists""/></cruisecontrol>";

			string actualXml = LoadStylesheetAndTransformInput(input);
			CustomAssertion.AssertContains("Modifications Detected", actualXml);
		}
	}
}