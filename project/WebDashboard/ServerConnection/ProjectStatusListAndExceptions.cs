using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.WebDashboard.ServerConnection
{
    public class ProjectStatusListAndExceptions
    {
        private readonly ProjectStatusOnServer[] statusAndServerList;
        private readonly CruiseServerException[] exceptions;

        public ProjectStatusListAndExceptions(ProjectStatusOnServer[] statusAndServerList, CruiseServerException[] exceptions)
        {
            this.statusAndServerList = statusAndServerList;
            this.exceptions = exceptions;
        }

        public ProjectStatusOnServer[] StatusAndServerList
        {
            get { return statusAndServerList; }
        }

        public CruiseServerException[] Exceptions
        {
            get { return exceptions; }
        }

        public ProjectStatus GetStatusForProject(string projectName)
        {
            foreach (ProjectStatusOnServer projectStatusOnServer in statusAndServerList)
            {
                if (projectStatusOnServer.ProjectStatus.Name == projectName)
                {
                    return projectStatusOnServer.ProjectStatus;
                }
            }
            throw new NoSuchProjectException(projectName);
        }
    }
}