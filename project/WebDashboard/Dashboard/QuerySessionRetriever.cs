using System;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
    public class QuerySessionRetriever
        : ISessionRetriever
    {
        private string sessionToken;

        public string RetrieveSessionToken(IRequest request)
        {
            sessionToken = request.GetText("session");
            return sessionToken;
        }

        public string SessionToken
        {
            get { return sessionToken; }
            set { sessionToken = value; }
        }
    }
}
