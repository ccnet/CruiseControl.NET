using System;
using System.Text;
using System.IO;
using System.Xml;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    public class HtmlDetailsMessageBuilder : IMessageBuilder
    {
//		private IBuildLogTransformer _buildLogTransformer;
        public HtmlDetailsMessageBuilder()
        {
//			_buildLogTransformer=new BuildLogTransformer();
        }
//		public HtmlDetailsMessageBuilder(IBuildLogTransformer logTransformer)
//		{
//			_buildLogTransformer=logTransformer;
//		}

        public string BuildMessage(IntegrationResult result, string projectURL)
        {
            StringBuilder message = new StringBuilder(10000);
            message.Append(CreateHtmlHeader());
            message.Append(CreateLinkToWebPage(result, projectURL));
            message.Append(CreateHorizontalRule());
            message.Append(CreateHtmlMessageDetails(result));
            message.Append(CreateHtmlFooter());
            return message.ToString();
        }

		private string CreateHtmlHeader()
		{
			return string.Format("<html><head>{0}</head><body>", HtmlEmailCss);
		}

		private string CreateLinkToWebPage(IntegrationResult result, string projectURL)
		{
			return new HtmlLinkMessageBuilder().BuildMessage(result, projectURL);
		}

		private string CreateHorizontalRule()
		{
			return @"<p></p><hr size=""1"" width=""98%"" align=""left"" color=""#888888""/>";
		}

        private string CreateHtmlMessageDetails(IntegrationResult result)
        {
            StringWriter buffer = new StringWriter();

            using (XmlIntegrationResultWriter integrationWriter=new XmlIntegrationResultWriter(new XmlTextWriter(buffer)))
			{
				integrationWriter.Write(result);
			}

			XmlDocument xml = new XmlDocument();
            xml.LoadXml(buffer.ToString());
            return new BuildLogTransformer().TransformResultsWithAllStyleSheets(xml);
        }

		private string CreateHtmlFooter()
		{
		    return "</body></html>";
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