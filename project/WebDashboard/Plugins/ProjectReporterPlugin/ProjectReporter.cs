using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReporterPlugin
{
	public class ProjectReporter
	{
		private readonly IPathMapper pathMapper;
		private readonly IRequestWrapper requestWrapper;
		private readonly IBuildRetrieverForRequest buildRetrieverForRequest;

		public ProjectReporter(IRequestWrapper requestWrapper, IBuildRetrieverForRequest buildRetrieverForRequest, IPathMapper pathMapper)
		{
			this.buildRetrieverForRequest = buildRetrieverForRequest;
			this.requestWrapper = requestWrapper;
			this.pathMapper = pathMapper;
		}

		public ProjectReportResults Do()
		{
			StringBuilder builder = new StringBuilder();
			string headerCellHtml = "";
			string detailsCellHtml = "";
			try
			{
				string log = buildRetrieverForRequest.GetBuild(requestWrapper).Log;
				// Todo - using?
				StringReader rdr = new StringReader(log);
				XPathDocument document = new XPathDocument(rdr);
				
				IList list = (IList) ConfigurationSettings.GetConfig("CCNet/xslFiles");
				foreach (string xslFile in list) 
				{
					if (xslFile.ToLower().IndexOf("header") > -1)
					{
						headerCellHtml = "<br/>" + Transform(xslFile, document);
					}
					else
					{
						builder.Append(Transform(xslFile, document)).Append("<br>");
					}
				}
			}
			catch(XmlException ex)
			{
				throw new CruiseControlException(String.Format("Bad XML in logfile: " + ex.Message));
			}

			detailsCellHtml = builder.ToString();
			return new ProjectReportResults(headerCellHtml, detailsCellHtml, GeneratePluginLinks());
		}

		private string Transform(string xslfile, XPathDocument logFileDocument)
		{
			// Is there a better way?
			string directory = Path.GetDirectoryName(xslfile);
			string file = Path.GetFileName(xslfile);
			string transformFile = Path.Combine(pathMapper.GetLocalPathFromURLPath(directory), file);
			return new BuildLogTransformer().Transform(logFileDocument, transformFile);
		}

		private string GeneratePluginLinks()
		{
			// Plugins disabled at the moment
			/*
			if (ConfigurationSettings.GetConfig("CCNet/buildPlugins") == null)
			{
				return;
			}

			string pluginLinksHtml = "";
			bool firstLink = true;
			foreach (PluginSpecification spec in (IEnumerable) ConfigurationSettings.GetConfig("CCNet/buildPlugins"))
			{
				if (!firstLink)
				{
					pluginLinksHtml += String.Format("|&nbsp; ");
				}
				pluginLinksHtml += String.Format(@"<a class=""link"" href=""{0}"">{1}</a> ", ResolveUrl(String.Format("{0}{1}", urlPrefix, LogFileUtil.CreateUrl(new FileInfo(logfile).Name, webUtil.GetCurrentlyViewedProjectName()))), spec.LinkText);
				firstLink = false;
			}
			PluginLinks.InnerHtml = pluginLinksHtml;
			*/
			return "";
		}
	}
}
