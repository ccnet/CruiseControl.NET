using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	public class HtmlDetailsMessageBuilder : IMessageBuilder
	{
		private const string HtmlCSSFile =@"xsl\Mailstyle.css";
		private string _htmlCss;

		public string BuildMessage(IIntegrationResult result, string projectURL)
		{
			StringBuilder message = new StringBuilder(10000);
			AppendHtmlHeader(message);
			AppendLinkToWebPage(message, result, projectURL);
			AppendHorizontalRule(message);
			AppendHtmlMessageDetails(message, result);
			AppendHtmlFooter(message);
			return message.ToString();
		}

		private void AppendHtmlHeader(StringBuilder message)
		{
			message.Append("<html><head>");
			message.Append("<style>");
			message.Append(HtmlEmailCss);
			message.Append("</style>");
			message.Append("</head><body>");
		}

		private void AppendLinkToWebPage(StringBuilder message, IIntegrationResult result, string projectURL)
		{
			message.Append(new HtmlLinkMessageBuilder(true).BuildMessage(result, projectURL));
		}

		private void AppendHorizontalRule(StringBuilder message)
		{
			message.Append(@"<p></p><hr size=""1"" width=""98%"" align=""left"" color=""#888888""/>");
		}

		private void AppendHtmlMessageDetails(StringBuilder message, IIntegrationResult result)
		{
			StringWriter buffer = new StringWriter();
			using (XmlIntegrationResultWriter integrationWriter = new XmlIntegrationResultWriter(new XmlTextWriter(buffer)))
			{
				integrationWriter.Write(result);
			}

			XPathDocument xml = new XPathDocument(new StringReader(buffer.ToString()));
			message.Append(new BuildLogTransformer().TransformResultsWithAllStyleSheets(xml));
		}

		private void AppendHtmlFooter(StringBuilder message)
		{
			message.Append("</body></html>");
		}

		private string HtmlEmailCss
		{
			get
			{
				if(_htmlCss == null)
				{
				    using (StreamReader textReader = File.OpenText(HtmlCSSFile))
				    {
				        _htmlCss = textReader.ReadToEnd();
				    }
				}
				return  _htmlCss;
			}
		}
	}
}