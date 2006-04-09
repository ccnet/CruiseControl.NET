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
		public void ShouldOutputIntegrationRequest()
		{
			string input = @"<cruisecontrol><request>foobar</request></cruisecontrol>";

			string actualXml = LoadStylesheetAndTransformInput(input);
			CustomAssertion.AssertContains("foobar", actualXml);
		}
	}
}