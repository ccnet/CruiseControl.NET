using NUnit.Framework;
using System.Xml.Linq;

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

		[Test]
		public void ShouldShowEachTestSuiteOnlyOnce()
		{
			string xml = @"<cruisecontrol><build><buildresults><target><task>
					<test-results total=""4"" failures=""1"" not-run=""3"" date=""2012-03-03"" time=""13:11:39"">
						<test-suite type=""TestFixture"" name=""Failed"" executed=""True"" result=""Failure"" success=""False""><results>
							<test-case name=""Failed.Fail"" executed=""True"" result=""Failure"" success=""False"" />
							<test-case name=""Failed.Ignore"" executed=""False"" result=""Ignored"" />
							<test-case name=""Failed.Pass"" executed=""True"" result=""Success"" success=""True"" />
						</results></test-suite>
							<test-suite type=""TestFixture"" name=""Ignored"" executed=""True"" result=""Success"" success=""True""><results>
							<test-case name=""Ignored.Ignore"" executed=""False"" result=""Ignored"" />
							<test-case name=""Ignored.Pass"" executed=""True"" result=""Success"" success=""True"" />
						</results></test-suite>
							<test-suite type=""TestFixture"" name=""IgnoredOnly"" executed=""True"" result=""Inconclusive"" success=""False""><results>
							<test-case name=""IgnoredOnly.Ignore"" executed=""False"" result=""Ignored"" />
						</results></test-suite>
							<test-suite type=""TestFixture"" name=""Passed"" executed=""True"" result=""Success"" success=""True""><results>
							<test-case name=""Passed.Pass"" executed=""True"" result=""Success"" success=""True"" />
						</results></test-suite>
					</test-results>
				</task></target></buildresults></build></cruisecontrol>";

			string actualXml = LoadStylesheetAndTransformInput(xml);

			CustomAssertion.AssertMatchCount(4, @"<a name=""[^""]+"">", actualXml);
		}
	}
}