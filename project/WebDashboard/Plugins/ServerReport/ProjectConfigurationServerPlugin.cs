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

    [ReflectorType("projectConfigurationServerPlugin")]
    public class ProjectConfigurationServerPlugin : ICruiseAction, IPlugin
    {

        private readonly IFarmService farmService;
        private readonly IVelocityViewGenerator viewGenerator;

        public ProjectConfigurationServerPlugin(IFarmService farmService, IVelocityViewGenerator viewGenerator)
        {
            this.farmService = farmService;
            this.viewGenerator = viewGenerator;
        }

        public IResponse Execute(ICruiseRequest request)
		{
			Hashtable velocityContext = new Hashtable();

            ProjectStatusListAndExceptions projectList = farmService.GetProjectStatusListAndCaptureExceptions(request.ServerSpecifier);
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
