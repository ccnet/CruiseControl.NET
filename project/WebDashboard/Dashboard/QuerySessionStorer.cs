using System;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
    public class QuerySessionStorer
        : ISessionStorer
    {
        private string sessionToken;

        public string GenerateQueryToken()
        {
            if (!string.IsNullOrEmpty(sessionToken))
            {
                return "session=" + sessionToken;
            }
            else
            {
                return string.Empty;
            }
        }

        public string SessionToken
        {
            get { return sessionToken; }
            set { sessionToken = value; }
        }
    }
}
