using System;
using System.Collections;
using System.Web;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.Statistics;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.Configuration;
using System.Collections.Generic;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport
{
    [ReflectorType("projectReportProjectPlugin")]
    public class ProjectReportProjectPlugin : ICruiseAction, IPlugin
    {
        private readonly IFarmService farmService;
        private readonly IVelocityViewGenerator viewGenerator;
        private readonly ILinkFactory linkFactory;
        public static readonly string ACTION_NAME = "ViewProjectReport";
        private IBuildPlugin[] pluginNames = null;
        private readonly IRemoteServicesConfiguration configuration;
        private ICruiseUrlBuilder urlBuilder;
        
        // retrieve at most this amount of builds                             
        public static readonly Int32 AmountOfBuildsToRetrieve = 100;

        [ReflectorArray("reportPlugins", Required = false)]
        public IBuildPlugin[] DashPlugins
        {
            get { return pluginNames; }
            set { pluginNames = value; }
        }

        public ProjectReportProjectPlugin(IFarmService farmService, IVelocityViewGenerator viewGenerator, ILinkFactory linkFactory,
            IRemoteServicesConfiguration configuration, ICruiseUrlBuilder urlBuilder)
        {
            this.farmService = farmService;
            this.viewGenerator = viewGenerator;
            this.linkFactory = linkFactory;
            this.configuration = configuration;
            this.urlBuilder = urlBuilder;
        }

        public IResponse Execute(ICruiseRequest cruiseRequest)
        {
            Hashtable velocityContext = new Hashtable();
            IProjectSpecifier projectSpecifier = cruiseRequest.ProjectSpecifier;
            IServerSpecifier serverSpecifier = FindServer(projectSpecifier);
            var sessionToken = cruiseRequest.RetrieveSessionToken();

            IBuildSpecifier[] buildSpecifiers = farmService.GetMostRecentBuildSpecifiers(projectSpecifier, 1, sessionToken);
            if (buildSpecifiers.Length == 1)
            {
                velocityContext["mostRecentBuildUrl"] = linkFactory.CreateProjectLink(projectSpecifier, LatestBuildReportProjectPlugin.ACTION_NAME).Url;
            }

            velocityContext["projectName"] = projectSpecifier.ProjectName;
            velocityContext["server"] = serverSpecifier;
            velocityContext["externalLinks"] = farmService.GetExternalLinks(projectSpecifier, sessionToken);
            velocityContext["noLogsAvailable"] = (buildSpecifiers.Length == 0);

            velocityContext["StatusMessage"] = ForceBuildIfNecessary(projectSpecifier, cruiseRequest.Request, sessionToken);
            ProjectStatus status = FindProjectStatus(projectSpecifier, cruiseRequest);
            velocityContext["status"] = status;
            velocityContext["StartStopButtonName"] = (status.Status == ProjectIntegratorState.Running) ? "StopBuild" : "StartBuild"; 
            velocityContext["StartStopButtonValue"] = (status.Status == ProjectIntegratorState.Running) ? "Stop" : "Start";
            velocityContext["ForceAbortBuildButtonName"] = (status.Activity.IsSleeping() ? "ForceBuild" : "AbortBuild");
		    velocityContext["ForceAbortBuildButtonValue"] = (status.Activity.IsSleeping() ? "Force" : "Abort");
            velocityContext["ParametersUrl"] = urlBuilder.BuildProjectUrl(ProjectParametersAction.ActionName, projectSpecifier);

            if (cruiseRequest.Request.ApplicationPath == "/")
                velocityContext["applicationPath"] = string.Empty;
            else
                velocityContext["applicationPath"] = cruiseRequest.Request.ApplicationPath;

            velocityContext["rssDataPresent"] = farmService.GetRSSFeed(projectSpecifier).Length > 0;

            // I (willemsruben) can not figure out why the line below does not work :-(
            // velocityContext["rss"] = linkFactory.CreateProjectLink(projectSpecifier, WebDashboard.Plugins.RSS.RSSFeed.ACTION_NAME).Url;
            //
            velocityContext["rss"] = RSSLinkBuilder.CreateRSSLink(linkFactory, projectSpecifier);

            velocityContext["ohloh"] = farmService.GetLinkedSiteId(projectSpecifier, sessionToken, "ohloh") ?? string.Empty;

            string subReportData = GetPluginSubReport(cruiseRequest, projectSpecifier, buildSpecifiers);
            if (subReportData != null && subReportData != String.Empty)
                velocityContext["pluginInfo"] = subReportData;



            BuildGraph GraphMaker;
            // if the amount of builds exceed this, foresee extra column(s) for the days                         
            //   adjusting the Y-axis of the graph                                                               
            Int32 MaxBuildTreshhold = 15;
            // Limits the X-axis to this amount of days                                                          
            Int32 MaxAmountOfDaysToDisplay = 15;
            // the amount of columns to foresee for 1 day in the graph                                           
            Int32 DateMultiPlier;

            GraphMaker = new BuildGraph(
                farmService.GetMostRecentBuildSpecifiers(projectSpecifier, AmountOfBuildsToRetrieve, sessionToken),
                this.linkFactory);

            velocityContext["graphDayInfo"] = GraphMaker.GetBuildHistory(MaxAmountOfDaysToDisplay);
            velocityContext["highestAmountPerDay"] = GraphMaker.HighestAmountPerDay;

            DateMultiPlier = (GraphMaker.HighestAmountPerDay / MaxBuildTreshhold) + 1;
            velocityContext["dateMultiPlier"] = DateMultiPlier;


            Int32 okpercent = 100;
            if (GraphMaker.AmountOfOKBuilds + GraphMaker.AmountOfFailedBuilds > 0)
            {
                okpercent = 100 * GraphMaker.AmountOfOKBuilds / (GraphMaker.AmountOfOKBuilds + GraphMaker.AmountOfFailedBuilds);
            }
            velocityContext["OKPercent"] = okpercent;
            velocityContext["NOKPercent"] = 100 - okpercent;
                                           
            return viewGenerator.GenerateView(@"ProjectReport.vm", velocityContext);
        }

        private IServerSpecifier FindServer(IProjectSpecifier projectSpecifier)
        {
            foreach (ServerLocation server in configuration.Servers)
            {
                if (string.Equals(projectSpecifier.ServerSpecifier.ServerName, server.Name))
                {
                    return new DefaultServerSpecifier(server.Name, server.AllowForceBuild, server.AllowStartStopBuild);
                }
            }
            throw new Exception("Unable to find specified server");
        }

        private ProjectStatus FindProjectStatus(IProjectSpecifier projectSpecifier, ICruiseRequest request)
        {
            ProjectStatusListAndExceptions list = farmService.GetProjectStatusListAndCaptureExceptions(projectSpecifier.ServerSpecifier, request.RetrieveSessionToken());
            foreach (ProjectStatusOnServer status in list.StatusAndServerList)
            {
                if (string.Equals(status.ProjectStatus.Name, projectSpecifier.ProjectName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return status.ProjectStatus;
                }
            }
            throw new Exception("Unable to retrieve project status");
        }

        private string ForceBuildIfNecessary(IProjectSpecifier projectSpecifier, IRequest request, string sessionToken)
        {
            // Retrieve the parameters
            var parameters = new Dictionary<string, string>();
            foreach (string parameterName in HttpContext.Current.Request.Form.AllKeys)
            {
                if (parameterName.StartsWith("param_"))
                {
                    parameters.Add(parameterName.Substring(6), HttpContext.Current.Request.Form[parameterName]);
                }
            }

            if (!string.IsNullOrEmpty(request.FindParameterStartingWith("StopBuild")))
            {
                farmService.Stop(projectSpecifier, sessionToken);
                return string.Format("Stopping project {0}", projectSpecifier.ProjectName);
            }
            else if (!string.IsNullOrEmpty(request.FindParameterStartingWith("StartBuild")))
            {
                farmService.Start(projectSpecifier, sessionToken);
                return string.Format("Starting project {0}", projectSpecifier.ProjectName);
            }
            else if (!string.IsNullOrEmpty(request.FindParameterStartingWith("ForceBuild")))
            {
                farmService.ForceBuild(projectSpecifier, sessionToken, parameters);
                return string.Format("Build successfully forced for {0}", projectSpecifier.ProjectName);
            }
            else if (!string.IsNullOrEmpty(request.FindParameterStartingWith("AbortBuild")))
            {
                farmService.AbortBuild(projectSpecifier, sessionToken);
                return string.Format("Abort successfully forced for {0}", projectSpecifier.ProjectName);
            }
            else
            {
                return string.Empty;
            }
        }

        public string LinkDescription
        {
            get { return "Project Report"; }
        }

        public INamedAction[] NamedActions
        {
            get { return new INamedAction[] { new ImmutableNamedAction(ACTION_NAME, this) }; }
        }

        private string GetPluginSubReport(ICruiseRequest cruiseRequest,
                                          IProjectSpecifier projectSpecifier, IBuildSpecifier[] buildSpecifiers)
        {
            if (buildSpecifiers.Length > 0 && pluginNames != null)
            {
                string outputResponse = String.Empty;

                ModifiedCruiseRequest req = new ModifiedCruiseRequest(cruiseRequest.Request, cruiseRequest.UrlBuilder);
                req.ReplaceBuildSpecifier(buildSpecifiers[0]);

                foreach (IBuildPlugin buildPlugIn in pluginNames)
                {
                    if (buildPlugIn != null && buildPlugIn.IsDisplayedForProject(projectSpecifier) &&
                        buildPlugIn.NamedActions != null)
                    {
                        foreach (INamedAction namedAction in buildPlugIn.NamedActions)
                        {
                            IResponse resp = namedAction.Action.Execute(req);

                            if (resp != null && resp is HtmlFragmentResponse)
                                outputResponse += ((HtmlFragmentResponse)resp).ResponseFragment;
                        }
                    }
                }
                return outputResponse;
            }
            return null;
        }

        private class ModifiedCruiseRequest : ICruiseRequest
        {
            private readonly IRequest request;

            private IServerSpecifier serverSpecifier = null;
            private IProjectSpecifier projectSpecifier = null;
            private IBuildSpecifier buildSpecifier = null;
            private ICruiseUrlBuilder urlBuilder;

            public ModifiedCruiseRequest(IRequest request, ICruiseUrlBuilder urlBuilder)
            {
                this.request = request;
                this.urlBuilder = urlBuilder;
            }

            public ICruiseUrlBuilder UrlBuilder
            {
				get { return this.urlBuilder; }
            }

            public string ServerName
            {
                get { return (serverSpecifier != null) ? serverSpecifier.ServerName : FindRESTSpecifiedResource(DefaultCruiseUrlBuilder.ServerRESTSpecifier); }
            }

            public string ProjectName
            {
                get { return (projectSpecifier != null) ? projectSpecifier.ProjectName : FindRESTSpecifiedResource(DefaultCruiseUrlBuilder.ProjectRESTSpecifier); }
            }

            public string BuildName
            {
                get { return (buildSpecifier != null) ? buildSpecifier.BuildName : FindRESTSpecifiedResource(DefaultCruiseUrlBuilder.BuildRESTSpecifier); }
            }

            private string FindRESTSpecifiedResource(string specifier)
            {
                string[] subFolders = request.SubFolders;

                for (int i = 0; i < subFolders.Length; i += 2)
                {
                    if (subFolders[i] == specifier)
                    {
                        if (i < subFolders.Length)
                        {
                            return HttpUtility.UrlDecode(subFolders[i + 1]);
                        }
                        else
                        {
                            throw new CruiseControlException(
                                string.Format("unexpected URL format - found {0} REST Specifier, but no following value", specifier));
                        }
                    }
                }

                return string.Empty;
            }

            public IServerSpecifier ServerSpecifier
            {
                get { return (serverSpecifier != null) ? serverSpecifier : new DefaultServerSpecifier(ServerName); }
            }

            public IProjectSpecifier ProjectSpecifier
            {
                get { return (projectSpecifier != null) ? projectSpecifier : new DefaultProjectSpecifier(ServerSpecifier, ProjectName); }
            }

            public IBuildSpecifier BuildSpecifier
            {
                get { return (buildSpecifier != null) ? buildSpecifier : new DefaultBuildSpecifier(ProjectSpecifier, BuildName); }
            }

            public IRequest Request
            {
                get { return request; }
            }

            public void ReplaceBuildSpecifier(IBuildSpecifier buildSpecifier)
            {
                this.buildSpecifier = buildSpecifier;
            }

            /// <summary>
            /// Attempt to retrieve a session token
            /// </summary>
            /// <returns></returns>
            public virtual string RetrieveSessionToken()
            {
                return RetrieveSessionToken(null);
            }

            /// <summary>
            /// Attempt to retrieve a session token
            /// </summary>
            /// <returns></returns>
            public virtual string RetrieveSessionToken(ISessionRetriever sessionRetriever)
            {
                // Attempt to find a session token
                string sessionToken = request.GetText("sessionToken");
                if (string.IsNullOrEmpty(sessionToken) && (sessionRetriever != null))
                {
                    sessionToken = sessionRetriever.RetrieveSessionToken(request);
                }
                return sessionToken;
            }
        }
    }
}
