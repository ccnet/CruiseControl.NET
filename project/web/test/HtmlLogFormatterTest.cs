using NUnit.Framework;
using System;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Web.Test
{
	[TestFixture]
	public class HtmlLogFormatterTest : CustomAssertion
	{
		private const string DEBUG = "07/04/2004 17:13:08: [foo:Debug]: VSSPublisher: History of $/foo ...";
		private const string INFO = "07/04/2004 17:13:08: [foo:Info]: No modifications detected.";
		private const string WARNING = @"07/04/2004 17:13:08: [foo:Warning]: Source control wrote output to stderr: ";
		private const string ERROR = @"07/04/2004 17:13:08: [foo:Error]: Unexpected exception occurred.";

		private HtmlLogFormatter formatter;

		[SetUp]
		protected void CreateHtmlLogFormatter()
		{
			formatter = new HtmlLogFormatter();
		}

		[Test]
		public void ShouldReturnSameStringForDebugLogMessage()
		{
			AssertEquals(DEBUG + HtmlLogFormatter.HTML_NEWLINE, formatter.Format(DEBUG));
		}

		[Test]
		public void ShouldReturnSameStringForInfoLogMessage()
		{
			AssertEquals(INFO + HtmlLogFormatter.HTML_NEWLINE, formatter.Format(INFO));
		}

		[Test]
		public void ShouldEncloseWarningInSpan()
		{
			string actual = formatter.Format(WARNING);
			AssertEquals(string.Format(@"<span class=""warning"">{0}</span>{1}", WARNING, HtmlLogFormatter.HTML_NEWLINE), actual);
		}

		[Test]
		public void ShouldEncloseErrorInSpan()
		{
			string actual = formatter.Format(ERROR);
			AssertEquals(string.Format(@"<span class=""error"">{0}</span>{1}", ERROR, HtmlLogFormatter.HTML_NEWLINE), actual);
		}

		[Test]
		public void ShouldEncloseOnlyWarningsAndErrorsInSpanIfMultipleLinesSupplied()
		{
			string input = string.Format("{0}{1}{2}{1}{3}{1}{4}{1}{2}", DEBUG, Environment.NewLine, WARNING, INFO, ERROR);
			string expected = string.Format(@"{0}{4}<span class=""warning"">{1}</span>{4}{2}<br /><span class=""error"">{3}</span>{4}<span class=""warning"">{1}</span>{4}", 
				DEBUG, WARNING, INFO, ERROR, HtmlLogFormatter.HTML_NEWLINE);
			AssertEquals(expected, formatter.Format(input));
		}

	}
}