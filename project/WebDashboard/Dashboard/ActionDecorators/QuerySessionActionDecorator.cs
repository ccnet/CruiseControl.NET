using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

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
