using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
    public class RequestController
    {
        private readonly IRequest request;
        private readonly IFingerprintFactory fingerprintFactory;
        private readonly IActionFactory actionFactory;

        public RequestController(IActionFactory actionFactory, IRequest request, IFingerprintFactory fingerprintFactory)
        {
            this.actionFactory = actionFactory;
            this.request = request;
            this.fingerprintFactory = fingerprintFactory;
        }

        public IResponse Do()
        {
            ConditionalGetFingerprint serverFingerprint = GetServerFingerprint();
            ConditionalGetFingerprint clientFingerprint = fingerprintFactory.BuildFromRequest(request);
            if (serverFingerprint.Equals(clientFingerprint))
            {
                return new NotModifiedResponse(serverFingerprint);
            }

            IAction action = actionFactory.Create(request);
            IResponse response = action.Execute(request);
            response.ServerFingerprint = serverFingerprint;
            return response;
        }

        private ConditionalGetFingerprint GetServerFingerprint()
        {
            IConditionalGetFingerprintProvider fingerPrintProvider = actionFactory.CreateFingerprintProvider(request);
            if (fingerPrintProvider == null)
            {
                return ConditionalGetFingerprint.NOT_AVAILABLE;
            }
            else
            {
                return fingerPrintProvider.GetFingerprint(request);
            }
        }
    }
}