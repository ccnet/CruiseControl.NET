using System.IO;
using System.Text;

namespace ThoughtWorks.CruiseControl.WebDashboard
{
	public class HtmlLogFormatter
	{
		public const string HTML_NEWLINE = "<br />";

		public string Format(string log)
		{
			StringBuilder builder = new StringBuilder(log.Length);

			StringReader reader = new StringReader(log);
			while (reader.Peek() >= 0)
			{
				builder.Append(FormatLine(reader.ReadLine()));
				builder.Append(HTML_NEWLINE);
			}
			return builder.ToString();
		}

		private string FormatLine(string line)
		{
			if (line.IndexOf(":Warning]") >= 0)
			{
				return string.Format(@"<span class=""warning"">{0}</span>", line);
			}
			else if (line.IndexOf(":Error]") >= 0)
			{
				return string.Format(@"<span class=""error"">{0}</span>", line);
			}
			else
			{
				return line;
			}
		}
	}
}
