using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Xml.XPath;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewBuildReport
{
	// ToDo - this is pretty much copied staright from the old web app, and is untested.
	public class ViewBuildReportAction : ICruiseAction
	{
		private readonly IPathMapper pathMapper;
		private readonly IBuildRetriever buildRetriever;

		public ViewBuildReportAction(IBuildRetriever buildRetriever, IPathMapper pathMapper)
		{
			this.buildRetriever = buildRetriever;
			this.pathMapper = pathMapper;
		}

		public Control Execute (ICruiseRequest cruiseRequest)
		{
			Build build = buildRetriever.GetBuild(cruiseRequest.ServerName, cruiseRequest.ProjectName, cruiseRequest.BuildName);
			StringBuilder builder = new StringBuilder();
			try
			{
				using(StringReader logReader = new StringReader(build.Log))
				{
					XPathDocument document = new XPathDocument(logReader);
					IList list = (IList) ConfigurationSettings.GetConfig("CCNet/xslFiles");
					foreach (string xslFile in list) 
					{
						builder.Append(Transform(xslFile, document));
						builder.Append("<br/>");
					}
				}
			}
			catch(XmlException ex)
			{
				throw new CruiseControlException(String.Format("Bad XML in logfile: " + ex.Message));
			}

			HtmlGenericControl control = new HtmlGenericControl("div");
			control.InnerHtml = builder.ToString();
			return control;
		}

		private string Transform(string xslfile, XPathDocument logFileDocument)
		{
			// Is there a better way?
			string directory = Path.GetDirectoryName(xslfile);
			string file = Path.GetFileName(xslfile);
			string transformFile = Path.Combine(pathMapper.GetLocalPathFromURLPath(directory), file);
			return new BuildLogTransformer().Transform(logFileDocument, transformFile);
		}
	}
}
