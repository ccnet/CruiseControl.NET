using Exortech.NetReflector;
using System;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
    [ReflectorType("queryStore")]
    public class QuerySessionStore
        : ISessionStore
    {
        public ISessionStorer RetrieveStorer()
        {
            return new QuerySessionStorer();
        }

        public ISessionRetriever RetrieveRetriever()
        {
            return new QuerySessionRetriever();
        }
    }
}
