using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport
{
    public class ProjectXmlReport : ICruiseAction
    {
        public const string ActionName = "ProjectXml";
        private readonly IFarmService farmService;

        public ProjectXmlReport(IFarmService farmService)
        {
            this.farmService = farmService;
        }

        public IResponse Execute(ICruiseRequest cruiseRequest)
        {
            ProjectStatusListAndExceptions projectStatuses = farmService.GetProjectStatusListAndCaptureExceptions(cruiseRequest.ServerSpecifier);
            ProjectStatus projectStatus = projectStatuses.GetStatusForProject(cruiseRequest.ProjectName);
            string xml = new CruiseXmlWriter().Write(projectStatus);
            return new XmlFragmentResponse(xml);
        }
    }
}
