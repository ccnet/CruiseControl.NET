using System;
using System.IO;
using System.Text;
using System.Xml;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	public class HtmlDetailsMessageBuilder : IMessageBuilder
	{
		public string BuildMessage(IntegrationResult result, string projectURL)
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
			message.Append(HtmlEmailCss);
			message.Append("</head><body>");
		}

		private void AppendLinkToWebPage(StringBuilder message, IntegrationResult result, string projectURL)
		{
			message.Append(new HtmlLinkMessageBuilder(true).BuildMessage(result, projectURL));
		}

		private void AppendHorizontalRule(StringBuilder message)
		{
			message.Append(@"<p></p><hr size=""1"" width=""98%"" align=""left"" color=""#888888""/>");
		}

		private void AppendHtmlMessageDetails(StringBuilder message, IntegrationResult result)
		{
			StringWriter buffer = new StringWriter();

			using (XmlIntegrationResultWriter integrationWriter = new XmlIntegrationResultWriter(new XmlTextWriter(buffer)))
			{
				integrationWriter.Write(result);
			}

			XmlDocument xml = new XmlDocument();
			xml.LoadXml(buffer.ToString());			
			message.Append(new BuildLogTransformer().TransformResultsWithAllStyleSheets(xml));
		}

		private void AppendHtmlFooter(StringBuilder message)
		{
			message.Append("</body></html>");
		}

		// for now, this is simply a copy of the contents of web/cruisecontrol.css
		// TODO read this from the actual file (need a way to access the file from this publisher)
		private const string HtmlEmailCss = @"<style>
body, table, form, input, td, th, p, textarea, select
{
	font-family: verdana, helvetica, arial;
	font-size: 11px;
}

a:hover { color:#FC0; }

.main-panel { color:#FC0; }

.link { color:#FFF; text-decoration:none; }
.link-failed { color:#F30; text-decoration:none; }
.buildresults-header { color: #FFF; font-weight: bold; }
.buildresults-data { color: #9F3; }
.buildresults-data-failed { color: #F30; }

.stylesection { margin-left: 4px; }
.header-title { font-size:12px; color:#000; font-weight:bold; padding-bottom:10pt; }
.header-label { font-weight:bold; }
.header-data { color:#000; }
.header-data-error { font-family:courier, monospaced; color:#000; white-space:pre; }

.modifications-data { font-size:9px; color:#000; }
.modifications-sectionheader { background-color:#006; color:#FFF; }
.modifications-oddrow { background-color:#F0F7FF; }
.modifications-evenrow { background-color:#FFF; }

.changelists-oddrow { background-color:#F0F7FF; }
.changelists-evenrow { background-color:#FFF; }
.changelists-file-spacer { background-color:#FFF; }
.changelists-file-evenrow { background-color:#FFF; }
.changelists-file-oddrow { background-color:#F0F7FF; }
.changelists-file-header { font-size:9px; background-color:#666; color:#FFF; }

.compile-data { font-size:9px; color:#000; }
.compile-error-data { font-size:9px; color:#F30; white-space:pre; }
.compile-warn-data { font-size:9px; color:#C90; white-space:pre; }
.compile-sectionheader { background-color:#006; color:#FFF; }

.distributables-data { font-size:9px; color:#000; }
.distributables-sectionheader { background-color:#006; color:#FFF; }
.distributables-oddrow { background-color:#F0F7FF; }

.unittests-sectionheader { background-color:#006; color:#FFF; }
.unittests-oddrow { background-color:#F0F7FF; }
.unittests-data { font-size:9px; color:#000; }
.unittests-error { font-size:9px; color:#F30; white-space:pre; }

.javadoc-sectionheader { background-color:#006; color:#FFF; }
.javadoc-oddrow { background-color:#CCC; }
.javadoc-data { font-size:9px; color:#000; }
.javadoc-error { font-size:9px; color:#F30; white-space:pre; }
.javadoc-warning { font-size:9px; color:#000; white-space:pre; }

.section-table { margin-top:10px; }
</style>";

	}
}