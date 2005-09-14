using System;
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
			string input = string.Format(@"<cruisecontrol>{0}</cruisecontrol>", ModificationString("2002-10-02 09:55"));
			string actualXml = LoadStylesheetAndTransformInput(input);
			CustomAssertion.AssertContains("2002-10-02 09:55", actualXml);
		}

		[Test]
		public void ShouldSortModificationsCorrectlyByDate()
		{
			string mod1 = ModificationString(DateUtil.FormatDate(new DateTime(2005, 8, 30)));
			string mod2 = ModificationString(DateUtil.FormatDate(new DateTime(2005, 9, 1)));
			string input = string.Format(@"<cruisecontrol>{0}{1}</cruisecontrol>", mod1, mod2);
			string actualXml = LoadStylesheetAndTransformInput(input);
			Assert.IsTrue(actualXml.IndexOf("2005-08-30") > actualXml.IndexOf("2005-09-01"));			
		}

		private string ModificationString(string date)
		{
			return string.Format(@"<modifications>
<modification type=""modified"">
	<filename>compile.xsl</filename>
	<project>project/web/xsl</project>
	<date>{0}</date>
	<user>exortech</user>
	<comment>modified stylesheet to view error messages</comment>
</modification></modifications>", date);
		}
	}
}