using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
    public class CruiseActionProxyAction : IAction, IConditionalGetFingerprintProvider
    {
        private readonly ICruiseRequestFactory cruiseRequestFactory;
        private readonly ICruiseAction proxiedAction;

        public CruiseActionProxyAction(ICruiseAction proxiedAction, ICruiseRequestFactory cruiseRequestFactory)
        {
            this.proxiedAction = proxiedAction;
            this.cruiseRequestFactory = cruiseRequestFactory;
        }

        public IResponse Execute(IRequest request)
        {
            return proxiedAction.Execute(cruiseRequestFactory.CreateCruiseRequest(request));
        }


        public ConditionalGetFingerprint GetFingerprint(IRequest request)
        {
            return ((IConditionalGetFingerprintProvider) proxiedAction).GetFingerprint(request);
        }
    }
}