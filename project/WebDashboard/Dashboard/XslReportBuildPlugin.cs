using System;
using System.Web.UI;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class XslReportBuildPlugin : ICruiseAction, IPluginLinkRenderer, IBuildPlugin
	{
		private readonly string actionName;
		private readonly string description;
		private readonly string xslFileName;
		private readonly IBuildLogTransformer buildLogTransformer;

		public XslReportBuildPlugin(IBuildLogTransformer buildLogTransformer, string xslFileName, string description, string actionName)
		{
			this.buildLogTransformer = buildLogTransformer;
			this.xslFileName = xslFileName;
			this.description = description;
			this.actionName = actionName;
		}

		public Control Execute (ICruiseRequest cruiseRequest)
		{
			return buildLogTransformer.Transform(cruiseRequest.BuildSpecifier, xslFileName);
		}

		public string Description
		{
			get { return description; }
		}

		public string ActionName
		{
			get { return actionName; }
		}

		public Type ActionType
		{
			get { return this.GetType(); }
		}
	}
}
