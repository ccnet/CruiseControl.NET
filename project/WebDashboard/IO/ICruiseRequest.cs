using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.IO
{
    public interface ICruiseRequest
    {
        string ServerName { get; }
        string ProjectName { get; }
        string BuildName { get; }

        IServerSpecifier ServerSpecifier { get; }
        IProjectSpecifier ProjectSpecifier { get; }
        IBuildSpecifier BuildSpecifier { get; }

        IRequest Request { get; }
        ICruiseUrlBuilder UrlBuilder { get; }

        /// <summary>
        /// Attempt to retrieve a session token
        /// </summary>
        /// <returns></returns>
        string RetrieveSessionToken(ISessionRetriever sessionRetriever);
    }
}