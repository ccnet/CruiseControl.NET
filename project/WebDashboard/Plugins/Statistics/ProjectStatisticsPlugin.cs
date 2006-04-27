using System;
using System.Collections;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.Statistics
{
	[ReflectorType("projectStatisticsPlugin")]
	public class ProjectStatisticsPlugin : ICruiseAction, IPlugin
	{
		private readonly IFarmService farmService;
		private readonly IPhysicalApplicationPathProvider pathProvider;
		public static readonly string ACTION_NAME = "ViewStatisticsReport";
		private string xslFileName;
		private ITransformer transformer;

		public ProjectStatisticsPlugin(IFarmService farmService, IPhysicalApplicationPathProvider pathProvider)
		{
			this.farmService = farmService;
			this.pathProvider = pathProvider;
			transformer = new XslTransformer();
		}

		[ReflectorProperty("xslFileName")]
		public string XslFileName
		{
			get
			{
				return xslFileName;
			}
			set
			{
				xslFileName = value;
			}
		}

		public INamedAction[] NamedActions
		{
			get { return new INamedAction[]{new ImmutableNamedAction(ACTION_NAME, this)}; }
		}

		public IResponse Execute(ICruiseRequest cruiseRequest)
		{
			if (xslFileName == null)
			{
				throw new ApplicationException("XSL File Name has not been set for XSL Report Action");
			}
			Hashtable xsltArgs = new Hashtable();
			xsltArgs["applicationPath"] = cruiseRequest.Request.ApplicationPath;

			string xslFile = Path.Combine(pathProvider.PhysicalApplicationPath, XslFileName);
			string statisticsDocument = farmService.GetStatisticsDocument(cruiseRequest.ProjectSpecifier);
			return new HtmlFragmentResponse(transformer.Transform(statisticsDocument, xslFile, xsltArgs));
		}

		public string LinkDescription
		{
			get { return "View Statistics"; }
		}
	}
}
