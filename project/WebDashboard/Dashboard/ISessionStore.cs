using System;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
    /// <summary>
    /// Provides a session store.
    /// </summary>
    public interface ISessionStore
    {
        ISessionStorer RetrieveStorer();
        ISessionRetriever RetrieveRetriever();
    }
}
