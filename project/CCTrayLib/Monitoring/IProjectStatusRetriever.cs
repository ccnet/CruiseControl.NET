using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
    public interface IProjectStatusRetriever
    {
        /// <summary>
        /// Lookup the last project status retrieved for this project.
        /// </summary>
        ProjectStatus GetProjectStatus(string projectName);
    }
}
