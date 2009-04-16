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

        public CruiseActionProxyAction(ICruiseAction proxiedAction, 
            ICruiseRequestFactory cruiseRequestFactory,
            ICruiseUrlBuilder urlBuilder)
        {
            this.proxiedAction = proxiedAction;
            this.cruiseRequestFactory = cruiseRequestFactory;
            this.urlBuilder = urlBuilder;
        }

        public IResponse Execute(IRequest request)
        {
            return proxiedAction.Execute(cruiseRequestFactory.CreateCruiseRequest(request, urlBuilder));
        }


        public ConditionalGetFingerprint GetFingerprint(IRequest request)
        {
            return ((IConditionalGetFingerprintProvider) proxiedAction).GetFingerprint(request);
        }
    }
}