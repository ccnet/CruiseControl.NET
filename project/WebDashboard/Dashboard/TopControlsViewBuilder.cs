using System.Collections;
using System.Web;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.FarmReport;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ServerReport;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class TopControlsViewBuilder : IConditionalGetFingerprintProvider
	{
		private readonly ICruiseRequest request;
		private readonly ILinkFactory linkFactory;
		private readonly IVelocityViewGenerator velocityViewGenerator;
		private readonly IFarmService farmService;
	    private readonly IFingerprintFactory fingerprintFactory;

	    public TopControlsViewBuilder(ICruiseRequest request, ILinkFactory linkFactory, IVelocityViewGenerator velocityViewGenerator, IFarmService farmService, IFingerprintFactory fingerprintFactory)
		{
			this.request = request;
			this.linkFactory = linkFactory;
			this.velocityViewGenerator = velocityViewGenerator;
			this.farmService = farmService;
		    this.fingerprintFactory = fingerprintFactory;
		}

		private string GetCategory()
		{
			// get category from request...
			string category = request.Request.GetText("Category");

			// ... or from the project status itself!
			if (string.IsNullOrEmpty(category) &&
				!string.IsNullOrEmpty(request.ServerName) &&
				!string.IsNullOrEmpty(request.ProjectName))
				category = farmService
					.GetProjectStatusListAndCaptureExceptions(request.ServerSpecifier)
					.GetStatusForProject(request.ProjectName)
					.Category;

			return category;
		}



		public HtmlFragmentResponse Execute()
		{
			Hashtable velocityContext = new Hashtable();

			string serverName = request.ServerName;
			string categoryName = GetCategory();
            string projectName = request.ProjectName;
			string buildName = request.BuildName;

			velocityContext["serverName"] = serverName;
			velocityContext["categoryName"] = categoryName;
			velocityContext["projectName"] = projectName;
			velocityContext["buildName"] = buildName;

			velocityContext["farmLink"] = linkFactory.CreateFarmLink("Dashboard", FarmReportFarmPlugin.ACTION_NAME);

			if (serverName != "")
			{
				velocityContext["serverLink"] = linkFactory.CreateServerLink(request.ServerSpecifier, ServerReportServerPlugin.ACTION_NAME);
			}

            if (categoryName != "")
            {
                IServerSpecifier serverSpecifier;
                try
                {
                    serverSpecifier = request.ServerSpecifier;
                }
                catch (ThoughtWorks.CruiseControl.Core.CruiseControlException)
                {
                    serverSpecifier = null;
                }

                if (serverSpecifier != null)
                {
                    velocityContext["categoryLink"] = new GeneralAbsoluteLink(categoryName, linkFactory
                        .CreateServerLink(serverSpecifier, "ViewServerReport")
                        .Url + "?Category=" + HttpUtility.UrlEncode(categoryName));
                }
                else
                {
                    velocityContext["categoryLink"] = new GeneralAbsoluteLink(categoryName, linkFactory
                        .CreateFarmLink( "Dashboard", FarmReportFarmPlugin.ACTION_NAME )
                        .Url + "?Category=" + HttpUtility.UrlEncode(categoryName));
                }
            }


			if (projectName != "")
			{
				velocityContext["projectLink"] = linkFactory.CreateProjectLink(request.ProjectSpecifier,  ProjectReportProjectPlugin.ACTION_NAME);
			}

			if (buildName != "")
			{
				velocityContext["buildLink"] = linkFactory.CreateBuildLink(request.BuildSpecifier,  BuildReportBuildPlugin.ACTION_NAME);
			}

			return velocityViewGenerator.GenerateView("TopMenu.vm", velocityContext);
		}

	    public ConditionalGetFingerprint GetFingerprint(IRequest request)
	    {
	        return fingerprintFactory.BuildFromFileNames();
	    }
	}
}
