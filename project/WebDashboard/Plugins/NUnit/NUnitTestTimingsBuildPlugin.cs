using System;
using System.Web.UI;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.NUnit
{
	public class NUnitTestTimingsBuildPlugin : ICruiseAction, IPluginLinkRenderer, IBuildPlugin
	{
		private readonly IRequestTransformer requestTransformer;

		public NUnitTestTimingsBuildPlugin(IRequestTransformer requestTransformer)
		{
			this.requestTransformer = requestTransformer;
		}

		public Control Execute (ICruiseRequest cruiseRequest)
		{
			return requestTransformer.Transform(cruiseRequest, @"xsl\timing.xsl");
		}

		public string Description
		{
			get { return "View Test Timings"; }
		}

		public string ActionName
		{
			get { return "ViewTestTimingsBuildReport"; }
		}

		public Type ActionType
		{
			get { return this.GetType(); }
		}
	}
}
