using System;
using System.Web.UI;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.FxCop
{
	public class FxCopReportBuildPlugin : ICruiseAction, IPluginLinkRenderer, IBuildPlugin
	{
		private readonly IRequestTransformer requestTransformer;

		public FxCopReportBuildPlugin(IRequestTransformer requestTransformer)
		{
			this.requestTransformer = requestTransformer;
		}

		public Control Execute (ICruiseRequest cruiseRequest)
		{
			return requestTransformer.Transform(cruiseRequest, @"xsl\FxCopReport.xsl");
		}

		public string Description
		{
			get { return "View FxCop Report"; }
		}

		public string ActionName
		{
			get { return "ViewFxCopBuildReport"; }
		}

		public Type ActionType
		{
			get { return this.GetType(); }
		}
	}
}
