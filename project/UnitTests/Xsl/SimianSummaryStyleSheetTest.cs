using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Xsl
{
	[TestFixture]
	public class SimianSummaryStylesheetTest : StylesheetTestFixture
	{
		protected override string Stylesheet
		{
			get { return @"xsl\SimianSummary.xsl"; }
		}

		[Test]
		public void ShouldOutputSummaryReport()
		{
			string input = 
				@"<simian version=""2.2.3"">
					<check ignoreCharacterCase=""true"" threshold=""6"">
						<set lineCount=""6"">
							<block sourceFile=""c:\temp\A.cs"" startLineNumber=""18"" endLineNumber=""23""/>
						    <block sourceFile=""c:\temp\B.cs"" startLineNumber=""34"" endLineNumber=""39""/>
					    </set>
						<set lineCount=""7"">
							<block sourceFile=""c:\temp\C.cs"" startLineNumber=""112"" endLineNumber=""118""/>
						    <block sourceFile=""c:\temp\D.cs"" startLineNumber=""183"" endLineNumber=""190""/>
				            <block sourceFile=""c:\temp\E.cs"" startLineNumber=""25"" endLineNumber=""45""/>
					    </set>
						<summary duplicateFileCount=""36"" duplicateLineCount=""365"" duplicateBlockCount=""50"" totalFileCount=""104"" totalRawLineCount=""17016"" totalSignificantLineCount=""9086"" processingTime=""1922""/>
					</check>
				</simian>";

			string actualXml = LoadStylesheetAndTransformInput(input);
			Assert.AreEqual(@"<div id=""simian-summary"" xmlns=""http://schemas.microsoft.com/intellisense/ie5""><table class=""section-table"" cellSpacing=""0"" cellPadding=""2"" width=""98%"" border=""0""><tr><td class=""sectionheader"" colSpan=""4"">Simian 2.2.3 Summary</td></tr><tr><td><b>Configuration</b></td></tr><tr><td colspan=""2"" class=""section-data"">ignoreCharacterCase</td><td colspan=""2"" class=""section-data"">true</td></tr><tr><td colspan=""2"" class=""section-data"">threshold</td><td colspan=""2"" class=""section-data"">6</td></tr><tr><td><b>Results</b></td></tr><tr><td colspan=""2"" class=""section-data"">duplicateFileCount</td><td colspan=""2"" class=""section-data"">36</td></tr><tr><td colspan=""2"" class=""section-data"">duplicateLineCount</td><td colspan=""2"" class=""section-data"">365</td></tr><tr><td colspan=""2"" class=""section-data"">duplicateBlockCount</td><td colspan=""2"" class=""section-data"">50</td></tr><tr><td colspan=""2"" class=""section-data"">totalFileCount</td><td colspan=""2"" class=""section-data"">104</td></tr><tr><td colspan=""2"" class=""section-data"">totalRawLineCount</td><td colspan=""2"" class=""section-data"">17016</td></tr><tr><td colspan=""2"" class=""section-data"">totalSignificantLineCount</td><td colspan=""2"" class=""section-data"">9086</td></tr><tr><td colspan=""2"" class=""section-data"">processingTime</td><td colspan=""2"" class=""section-data"">1922</td></tr></table></div>", actualXml);
		}
	}
}