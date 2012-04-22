using NUnit.Framework;

namespace ThoughtWorks.CruiseControl.UnitTests.Xsl
{
	[TestFixture]
	public class AlternativeNUnitStylesheetTest : StylesheetTestFixture
	{
		protected override string Stylesheet
		{
			get { return @"xsl\AlternativeNUnitDetails.xsl"; }
		}

		[Test]
		public void ShouldShowTestDetailsIfIncludingOutputFromNAnt()
		{
			string xml = @"<cruisecontrol><build><buildresults><target><task>
                    <test-results total=""6"" failures=""0"" not-run=""0"" date=""2005-04-29"" time=""9:02 PM"">
						<test-suite />
					</test-results>
				</task></target></buildresults></build></cruisecontrol>";

			string actualXml = LoadStylesheetAndTransformInput(xml);
			CustomAssertion.AssertContains("6\xA0tests", actualXml);
		}
	}
}