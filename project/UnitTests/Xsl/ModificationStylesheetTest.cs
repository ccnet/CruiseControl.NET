using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Xsl
{
	[TestFixture]
	public class ModificationStylesheetTest : StylesheetTestFixture
	{
		protected override string Stylesheet
		{
			get { return @"xsl\modifications.xsl"; }
		}

		[Test]
		public void ShouldOutputDateOfModification()
		{
			string input = @"<cruisecontrol><modifications>
<modification type=""modified"">
	<filename>compile.xsl</filename>
	<project>project/web/xsl</project>
	<date>02 Oct 2002 09:55</date>
	<user>exortech</user>
	<comment>modified stylesheet to view error messages</comment>
</modification></modifications></cruisecontrol>";

			string actualXml = LoadStylesheetAndTransformInput(input);
			CustomAssertion.AssertContains("02 Oct 2002 09:55", actualXml);
		}
	}
}