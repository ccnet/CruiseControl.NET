using System.Collections;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.FarmReport;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ServerReport;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class TopControlsViewBuilder
	{
		private readonly ICruiseRequest request;
		private readonly ILinkFactory linkFactory;
		private readonly IVelocityViewGenerator velocityViewGenerator;

		public TopControlsViewBuilder(ICruiseRequest request, ILinkFactory linkFactory, IVelocityViewGenerator velocityViewGenerator)
		{
			this.request = request;
			this.linkFactory = linkFactory;
			this.velocityViewGenerator = velocityViewGenerator;
		}

		public IView Execute()
		{
			Hashtable velocityContext = new Hashtable();

			string serverName = request.ServerName;
			string projectName = request.ProjectName;
			string buildName = request.BuildName;

			velocityContext["serverName"] = serverName;
			velocityContext["projectName"] = projectName;
			velocityContext["buildName"] = buildName;

			velocityContext["farmLink"] = linkFactory.CreateFarmLink("Dashboard", new ActionSpecifierWithName(FarmReportFarmPlugin.ACTION_NAME));

			if (serverName != "")
			{
				velocityContext["serverLink"] = linkFactory.CreateServerLink(request.ServerSpecifier, new ActionSpecifierWithName(ServerReportServerPlugin.ACTION_NAME));
			}

			if (projectName != "")
			{
				velocityContext["projectLink"] = linkFactory.CreateProjectLink(request.ProjectSpecifier,  new ActionSpecifierWithName(ProjectReportProjectPlugin.ACTION_NAME));
			}

			if (buildName != "")
			{
				velocityContext["buildLink"] = linkFactory.CreateBuildLink(request.BuildSpecifier,  new ActionSpecifierWithName(BuildReportBuildPlugin.ACTION_NAME));
			}

			return velocityViewGenerator.GenerateView("TopMenu.vm", velocityContext);
		}
	}
}
