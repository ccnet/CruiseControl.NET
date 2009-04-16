using System;
using System.Collections;
using Objection;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.WebDashboard.Configuration;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard.ActionDecorators
{
    public class QuerySessionActionDecorator
        : IAction
    {
        private readonly IAction decoratedAction;
        private readonly ISessionRetriever retriever;
        private readonly ISessionStorer storer;

        public QuerySessionActionDecorator(IAction decoratedAction, ISessionRetriever retriever,
            ISessionStorer storer)
        {
            this.decoratedAction = decoratedAction;
            this.retriever = retriever;
            this.storer = storer;
        }

        public IResponse Execute(IRequest request)
        {
            // Retrieve and set the session token
            retriever.RetrieveSessionToken(request);
            storer.SessionToken = retriever.SessionToken;

            // Pass on the request
            IResponse decoratedActionResponse = decoratedAction.Execute(request);
            return decoratedActionResponse;
        }
    }
}
