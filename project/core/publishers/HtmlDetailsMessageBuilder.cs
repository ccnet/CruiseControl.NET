using System.IO;
using System.Text;
using System.Xml.XPath;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Core.Tasks;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	public class HtmlDetailsMessageBuilder : IMessageBuilder
	{
		private static readonly SystemPath HtmlCSSFile = new SystemPath(@"xsl\cruisecontrol.css");
		private string htmlCss;

        private System.Collections.IList myXslFiles;



        public HtmlDetailsMessageBuilder()
        {
        }


        public System.Collections.IList xslFiles
        {
            get
            {
                return myXslFiles;
            }
            set
            {
                myXslFiles = value;
            }
        }

        public string BuildMessage(IIntegrationResult result, TaskContext context)
		{
			StringBuilder message = new StringBuilder(10000);
			AppendHtmlHeader(message);
			AppendLinkToWebPage(message, result, context);
			AppendHorizontalRule(message);
			AppendHtmlMessageDetails(message, context);
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

        private void AppendLinkToWebPage(StringBuilder message, IIntegrationResult result, TaskContext context)
		{
			message.Append(new HtmlLinkMessageBuilder(true).BuildMessage(result, context));
		}

		private void AppendHorizontalRule(StringBuilder message)
		{
			message.Append(@"<p></p><hr size=""1"" width=""98%"" align=""left"" color=""#888888""/>");
		}

		private void AppendHtmlMessageDetails(StringBuilder message, TaskContext context)
		{
			var buffer = new StringWriter();
            context.WriteCurrentLog(buffer);
			var xml = new XPathDocument(new StringReader(buffer.ToString()));
            if (xslFiles == null)
            {
                message.Append(new BuildLogTransformer().TransformResultsWithAllStyleSheets(xml));
            }
            else
            {
                message.Append(new BuildLogTransformer().TransformResults(xslFiles,xml ));
            }

		}

		private void AppendHtmlFooter(StringBuilder message)
		{
			message.Append("</body></html>");
		}

		private string HtmlEmailCss
		{
			get
			{
				if (htmlCss == null)
				{
					htmlCss = HtmlCSSFile.ReadTextFile();
				}
				return htmlCss;
			}
		}
	}
}