using System.IO;
using System.Text;
using System.Xml.XPath;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    /// <summary>
    /// 	
    /// </summary>
	public class HtmlDetailsMessageBuilder : IMessageBuilder
	{
		private static readonly SystemPath HtmlCSSFile = new SystemPath(@"xsl\cruisecontrol.css");
		private string htmlCss;

        private System.Collections.IList myXslFiles;



        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlDetailsMessageBuilder" /> class.	
        /// </summary>
        /// <remarks></remarks>
        public HtmlDetailsMessageBuilder()
        {
        }


        /// <summary>
        /// Gets or sets the XSL files.	
        /// </summary>
        /// <value>The XSL files.</value>
        /// <remarks></remarks>
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


        /// <summary>
        /// Builds the message.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public string BuildMessage(IIntegrationResult result)
		{
			StringBuilder message = new StringBuilder(10000);
			AppendHtmlHeader(message);
			AppendLinkToWebPage(message, result);
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

		private void AppendLinkToWebPage(StringBuilder message, IIntegrationResult result)
		{
			message.Append(new HtmlLinkMessageBuilder(true).BuildMessage(result));
		}

		private void AppendHorizontalRule(StringBuilder message)
		{
			message.Append(@"<p></p><hr size=""1"" width=""98%"" align=""left"" color=""#888888""/>");
		}

		private void AppendHtmlMessageDetails(StringBuilder message, IIntegrationResult result)
		{
			StringWriter buffer = new StringWriter();
			using (XmlIntegrationResultWriter integrationWriter = new XmlIntegrationResultWriter(buffer))
			{
				integrationWriter.Write(result);
			}

			XPathDocument xml = new XPathDocument(new StringReader(buffer.ToString()));

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