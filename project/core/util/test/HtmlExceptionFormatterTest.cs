using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Util;

namespace ThoughtWorks.CruiseControl.Core.Util.Test
{
	[TestFixture]
	public class HtmlExceptionFormatterTest : CustomAssertion
	{
		[Test]
		public void FormatShouldReplaceNewLinesWithBRTags()
		{
			HtmlExceptionFormatter formatter  = new HtmlExceptionFormatter(new Exception("foo"+Environment.NewLine+"Bar"));
		    string formattedString = formatter.ToString();
			Assert.IsTrue(formattedString.IndexOf(Environment.NewLine) == -1);
			Assert.AreEqual(1+1, CountOfStrings(formattedString, "<br/>"));
		}

	    private int CountOfStrings (string baseString, string stringToSearch)
	    {
	        int count=0;
			int curPos=0;
			while(true)
			{
			    int offset = baseString.IndexOf(stringToSearch, curPos);
				if(offset == -1) break;
				curPos = offset+ stringToSearch.Length;
				count++;
			}

			return count;
	    }
	}
}
