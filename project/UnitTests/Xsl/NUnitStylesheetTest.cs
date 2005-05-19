using NUnit.Framework;

namespace ThoughtWorks.CruiseControl.UnitTests.Xsl
{
	[TestFixture]
	public class NUnitStylesheetTest : StylesheetTestFixture
	{
		protected override string Stylesheet
		{
			get { return @"xsl\tests.xsl"; }
		}

		[Test]
		public void ShouldShowTestDetailsIfIncludingOutputFromNAnt()
		{
			string xml = @"<cruisecontrol><build><buildresults><target><task>
                    <test-results total=""1"" failures=""0"" not-run=""0"" date=""2005-04-29"" time=""9:02 PM"">
						<test-suite><results>
							<test-case executed=""True"" />
						</results></test-suite>
					</test-results>
				</task></target></buildresults></build></cruisecontrol>";

			string actualXml = LoadStylesheetAndTransformInput(xml);
			CustomAssertion.AssertContains("Tests executed:</td><td>1", actualXml);
		}
	}
}