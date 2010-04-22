namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport
{
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
    using ThoughtWorks.CruiseControl.WebDashboard.IO;
    using ThoughtWorks.CruiseControl.WebDashboard.MVC;
    using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
    using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

    public class ProjectXmlReport : ICruiseAction
    {
        public const string ActionName = "ProjectXml";
        private readonly IFarmService farmService;
        private readonly ISessionRetriever sessionRetriever;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectXmlReport"/> class.
        /// </summary>
        /// <param name="farmService">The farm service.</param>
        /// <param name="sessionRetriever">The session retriever.</param>
        public ProjectXmlReport(IFarmService farmService, ISessionRetriever sessionRetriever)
        {
            this.sessionRetriever = sessionRetriever;
            this.farmService = farmService;
        }

        /// <summary>
        /// Executes the specified cruise request.
        /// </summary>
        /// <param name="cruiseRequest">The cruise request.</param>
        /// <returns></returns>
        public IResponse Execute(ICruiseRequest cruiseRequest)
        {
            ProjectStatusListAndExceptions projectStatuses = farmService.GetProjectStatusListAndCaptureExceptions(cruiseRequest.ServerSpecifier,
                cruiseRequest.RetrieveSessionToken(sessionRetriever));
            ProjectStatus projectStatus = projectStatuses.GetStatusForProject(cruiseRequest.ProjectName);
            string xml = new CruiseXmlWriter().Write(projectStatus);
            return new XmlFragmentResponse(xml);
        }
    }
}
