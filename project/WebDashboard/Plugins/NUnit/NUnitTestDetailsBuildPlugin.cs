using System;
using System.Web.UI;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.NUnit
{
	public class NUnitTestDetailsBuildPlugin : ICruiseAction, IPluginLinkRenderer, IBuildPlugin
	{
		private readonly IRequestTransformer requestTransformer;

		public NUnitTestDetailsBuildPlugin(IRequestTransformer requestTransformer)
		{
			this.requestTransformer = requestTransformer;
		}

		public Control Execute (ICruiseRequest cruiseRequest)
		{
			return requestTransformer.Transform(cruiseRequest, @"xsl\tests.xsl");
		}

		public string Description
		{
			get { return "View Test Details"; }
		}

		public string ActionName
		{
			get { return "ViewTestDetailsBuildReport"; }
		}

		public Type ActionType
		{
			get { return this.GetType(); }
		}
	}
}
