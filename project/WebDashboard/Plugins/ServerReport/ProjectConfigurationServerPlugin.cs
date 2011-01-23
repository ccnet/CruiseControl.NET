using System.Collections;
using System.Collections.Generic;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;


namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ServerReport
{
    /// <title>Project Configuration Server Plugin</title>
    /// <version>1.4.0</version>
    /// <summary>
    /// This plugin shows the basic configuration of the projects on this buildserver, such as project name, category, queue and queue
    /// priority, making it easier to spot conflicts in the queue setup for this buildserver.
    /// </summary>
    /// <example>
    /// <code>
    /// &lt;projectConfigurationServerPlugin /&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <para type="tip">
    /// This can be installed using the "Server Configuration Display" package.
    /// </para>
    /// </remarks>
    [ReflectorType("projectConfigurationServerPlugin")]
    public class ProjectConfigurationServerPlugin : ICruiseAction, IPlugin
    {

        private readonly IFarmService farmService;
        private readonly IVelocityViewGenerator viewGenerator;
        private readonly ISessionRetriever sessionRetriever;

        public ProjectConfigurationServerPlugin(IFarmService farmService, IVelocityViewGenerator viewGenerator, ISessionRetriever sessionRetriever)
        {
            this.farmService = farmService;
            this.viewGenerator = viewGenerator;
            this.sessionRetriever = sessionRetriever;
        }

        public IResponse Execute(ICruiseRequest request)
		{
			Hashtable velocityContext = new Hashtable();

            ProjectStatusListAndExceptions projectList = farmService.GetProjectStatusListAndCaptureExceptions(request.ServerSpecifier,
                request.RetrieveSessionToken(sessionRetriever));
            List<ProjectStatus> projects = new List<ProjectStatus>();
            for (int projectLoop = 0; projectLoop < projectList.StatusAndServerList.Length; projectLoop++)
            {
                ProjectStatus projectStatus = projectList.StatusAndServerList[projectLoop].ProjectStatus;

                projects.Add(projectStatus);
            
            }

            projects.Sort(CompareProjectStatusByQueueAndQueuePriority); 

            velocityContext["projects"] = projects.ToArray();

            return viewGenerator.GenerateView(@"ProjectServerConfiguration.vm", velocityContext);
		}

        public string LinkDescription
        {
            get { return "View Project Configuration"; }
        }

        public INamedAction[] NamedActions
        {
            get { return new INamedAction[] { new ImmutableNamedAction("ProjectConfigurationServer", this) }; }
        }

        private int CompareProjectStatusByQueueAndQueuePriority(ProjectStatus x, ProjectStatus y)
        {
            if (x.Queue == y.Queue)
            {
                return x.QueuePriority.CompareTo(y.QueuePriority);
            }

            return x.Queue.CompareTo(y.Queue);
        }

    }
}
