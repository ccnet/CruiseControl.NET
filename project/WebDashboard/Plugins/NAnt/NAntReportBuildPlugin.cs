using System;
using System.Web.UI;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.NAnt
{
	public class NAntReportBuildPlugin : ICruiseAction, IPluginLinkRenderer, IBuildPlugin
	{
		private readonly IRequestTransformer requestTransformer;

		public NAntReportBuildPlugin(IRequestTransformer requestTransformer)
		{
			this.requestTransformer = requestTransformer;
		}

		public Control Execute (ICruiseRequest cruiseRequest)
		{
			return requestTransformer.Transform(cruiseRequest, @"xsl\NAnt.xsl");
		}

		public string Description
		{
			get { return "View NAnt Report"; }
		}

		public string ActionName
		{
			get { return "ViewNAntBuildReport"; }
		}

		public Type ActionType
		{
			get { return this.GetType(); }
		}
	}
}
