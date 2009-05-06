using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
    public class CruiseActionProxyAction : IAction, IConditionalGetFingerprintProvider
    {
        private readonly ICruiseRequestFactory cruiseRequestFactory;
        private readonly ICruiseAction proxiedAction;
        private readonly ICruiseUrlBuilder urlBuilder;
        private readonly ISessionRetriever retriever;

        public CruiseActionProxyAction(ICruiseAction proxiedAction, 
            ICruiseRequestFactory cruiseRequestFactory,
            ICruiseUrlBuilder urlBuilder,
            ISessionRetriever retriever)
        {
            this.proxiedAction = proxiedAction;
            this.cruiseRequestFactory = cruiseRequestFactory;
            this.urlBuilder = urlBuilder;
            this.retriever = retriever;
        }

        public IResponse Execute(IRequest request)
        {
            return proxiedAction.Execute(cruiseRequestFactory.CreateCruiseRequest(request, urlBuilder, retriever));
        }


        public ConditionalGetFingerprint GetFingerprint(IRequest request)
        {
            return ((IConditionalGetFingerprintProvider) proxiedAction).GetFingerprint(request);
        }
    }
}