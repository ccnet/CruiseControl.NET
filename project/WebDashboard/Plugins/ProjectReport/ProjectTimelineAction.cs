using System;
using System.Collections;
using System.Text;
using System.Xml;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport
{
    /// <summary>
    /// Displays a timeline of the project builds.
    /// </summary>
    public class ProjectTimelineAction
        : ICruiseAction
    {
        #region Public constants
        public const string TimelineActionName = "ProjectTimeline";
        public const string DataActionName = "ProjectTimelineData";
        #endregion

        #region Private fields
        private readonly IVelocityViewGenerator viewGenerator;
        private readonly IFarmService farmService;
        private readonly ICruiseUrlBuilder urlBuilder;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectTimelineAction"/> class.
        /// </summary>
        /// <param name="viewGenerator">The view generator.</param>
        /// <param name="farmService">The farm service.</param>
        /// <param name="urlBuilder"></param>
        public ProjectTimelineAction(IVelocityViewGenerator viewGenerator, IFarmService farmService, ICruiseUrlBuilder urlBuilder)
		{
            this.viewGenerator = viewGenerator;
            this.farmService = farmService;
            this.urlBuilder = urlBuilder;
        }
        #endregion
        
        #region Public methods
        #region Execute()
        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="cruiseRequest"></param>
        /// <returns></returns>
        public IResponse Execute(ICruiseRequest cruiseRequest)
        {
            IResponse response;

            if (string.Equals(cruiseRequest.Request.FileNameWithoutExtension, TimelineActionName, StringComparison.InvariantCultureIgnoreCase))
            {
                response = this.GenerateTimelinePage(cruiseRequest);
            }
            else if (string.Equals(cruiseRequest.Request.FileNameWithoutExtension, DataActionName, StringComparison.InvariantCultureIgnoreCase))
            {
                response = this.GenerateData(cruiseRequest);
            }
            else
            {
                throw new CruiseControlException("Unknown action: " + cruiseRequest.Request.FileNameWithoutExtension);
            }

            return response;
        }
        #endregion
        #endregion

        #region Private methods
        #region GenerateTimelinePage()
        /// <summary>
        /// Generates the timeline page.
        /// </summary>
        /// <param name="cruiseRequest">The cruise request.</param>
        /// <returns>The HTML to return to the client.</returns>
        private IResponse GenerateTimelinePage(ICruiseRequest cruiseRequest)
        {
            var velocityContext = new Hashtable();
            velocityContext.Add("projectName", cruiseRequest.ProjectName);
            if (cruiseRequest.Request.ApplicationPath == "/")
            {
                velocityContext["applicationPath"] = string.Empty;
            }
            else
            {
                velocityContext["applicationPath"] = cruiseRequest.Request.ApplicationPath;
            }

            velocityContext["dataUrl"] = this.urlBuilder.BuildProjectUrl(DataActionName, cruiseRequest.ProjectSpecifier);
            return this.viewGenerator.GenerateView("ProjectTimeline.vm", velocityContext);
        }
        #endregion

        #region GenerateData()
        /// <summary>
        /// Generates the data.
        /// </summary>
        /// <param name="cruiseRequest">The cruise request.</param>
        /// <returns>The XML data for the project.</returns>
        private IResponse GenerateData(ICruiseRequest cruiseRequest)
        {
            var builder = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                CheckCharacters = true,
                ConformanceLevel = ConformanceLevel.Document,
                Indent = false,
                NewLineHandling = NewLineHandling.None,
                NewLineOnAttributes = false,
                OmitXmlDeclaration = true
            };
            var basePath = (cruiseRequest.Request.ApplicationPath == "/" ? string.Empty : cruiseRequest.Request.ApplicationPath) +
                "/javascript/Timeline/images/";
            using (var xmlWriter = XmlWriter.Create(builder, settings))
            {
                xmlWriter.WriteStartElement("data");
                var builds = this.farmService.GetBuildSpecifiers(cruiseRequest.ProjectSpecifier, cruiseRequest.RetrieveSessionToken());
                foreach (var build in builds)
                {
                    this.AppendBuild(build, xmlWriter, basePath);
                }
                xmlWriter.WriteEndElement();
            }
            return new XmlFragmentResponse(builder.ToString());
        }
        #endregion

        #region AppendBuild()
        /// <summary>
        /// Appends a build to the document.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="xmlWriter">The XML writer.</param>
        /// <param name="basePath">The base path.</param>
        private void AppendBuild(IBuildSpecifier build, XmlWriter xmlWriter, string basePath)
        {
            var logFile = new LogFile(build.BuildName);
            xmlWriter.WriteStartElement("event");
            xmlWriter.WriteAttributeString("start", logFile.Date.ToString("r"));
            xmlWriter.WriteAttributeString("title", logFile.Succeeded ? "Success (" + logFile.Label + ")" : "Failure");
            xmlWriter.WriteAttributeString("color", logFile.Succeeded ? "green" : "red");
            xmlWriter.WriteAttributeString("icon", basePath + "dark-" + (logFile.Succeeded ? "green" : "red") + "-circle.png");

            var buildUrl = this.urlBuilder.BuildBuildUrl(BuildReportBuildPlugin.ACTION_NAME, build);
            xmlWriter.WriteString("<a href=\"" + buildUrl + "\">View Build</a>");
            xmlWriter.WriteEndElement();
        }
        #endregion
        #endregion
    }
}
