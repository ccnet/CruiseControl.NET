using System;
using System.Web.UI;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.NCover
{
	public class NCoverReportBuildPlugin : ICruiseAction, IPluginLinkRenderer, IBuildPlugin
	{
		private readonly IRequestTransformer requestTransformer;

		public NCoverReportBuildPlugin(IRequestTransformer requestTransformer)
		{
			this.requestTransformer = requestTransformer;
		}

		public Control Execute (ICruiseRequest cruiseRequest)
		{
			return requestTransformer.Transform(cruiseRequest, @"xsl\NCover.xsl");
		}

		public string Description
		{
			get { return "View NCover Report"; }
		}

		public string ActionName
		{
			get { return "ViewNCoverBuildReport"; }
		}

		public Type ActionType
		{
			get { return this.GetType(); }
		}
	}
}
