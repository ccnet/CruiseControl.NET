using System.Collections;
using Objection;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard.ActionDecorators
{
    // ToDo - test - I think doing so will change the design a bit - will probably get more in on
    // the constructor - should do this after 1.0
    public class SiteTemplateActionDecorator : IAction, IConditionalGetFingerprintProvider
    {
        private const string TEMPLATE_NAME = "SiteTemplate.vm";
        private readonly IAction decoratedAction;
        private readonly IVelocityViewGenerator velocityViewGenerator;
        private readonly ObjectSource objectSource;
        private readonly IVersionProvider versionProvider;
        private readonly IFingerprintFactory fingerprintFactory;
        private TopControlsViewBuilder topControlsViewBuilder;
        private SideBarViewBuilder sideBarViewBuilder;

        public SiteTemplateActionDecorator(IAction decoratedAction, IVelocityViewGenerator velocityViewGenerator,
                                           ObjectSource objectSource, IVersionProvider versionProvider,
                                           IFingerprintFactory fingerprintFactory)
        {
            this.decoratedAction = decoratedAction;
            this.velocityViewGenerator = velocityViewGenerator;
            this.objectSource = objectSource;
            this.versionProvider = versionProvider;
            this.fingerprintFactory = fingerprintFactory;
        }

        private TopControlsViewBuilder TopControlsViewBuilder
        {
            get
            {
                if (topControlsViewBuilder == null)
                {
                    topControlsViewBuilder =
                        (TopControlsViewBuilder) objectSource.GetByType(typeof (TopControlsViewBuilder));
                }
                return topControlsViewBuilder;
            }
        }

        private SideBarViewBuilder SideBarViewBuilder
        {
            get
            {
                if (sideBarViewBuilder == null)
                {
                    sideBarViewBuilder =
                        (SideBarViewBuilder) objectSource.GetByType(typeof (SideBarViewBuilder));
                }
                return sideBarViewBuilder;
            }
        }

        public IResponse Execute(IRequest request)
        {
            Hashtable velocityContext = new Hashtable();
            IResponse decoratedActionResponse = decoratedAction.Execute(request);
            if (decoratedActionResponse is HtmlFragmentResponse)
            {
                velocityContext["breadcrumbs"] = (TopControlsViewBuilder.Execute()).ResponseFragment;
                velocityContext["sidebar"] = (SideBarViewBuilder.Execute()).ResponseFragment;
                velocityContext["mainContent"] = ((HtmlFragmentResponse) decoratedActionResponse).ResponseFragment;
                velocityContext["dashboardversion"] = versionProvider.GetVersion();
                if (request.ApplicationPath == "/")
                    velocityContext["applicationPath"] = string.Empty;
                else
                    velocityContext["applicationPath"] = request.ApplicationPath;

                return velocityViewGenerator.GenerateView(TEMPLATE_NAME, velocityContext);
            }
            else
            {
                return decoratedActionResponse;
            }
        }

        public ConditionalGetFingerprint GetFingerprint(IRequest request)
        {
            ConditionalGetFingerprint fingerprint = CalculateLocalFingerprint(request);
            return fingerprint.Combine(((IConditionalGetFingerprintProvider) decoratedAction).GetFingerprint(request));
        }

        private ConditionalGetFingerprint CalculateLocalFingerprint(IRequest request)
        {
            return fingerprintFactory.BuildFromFileNames(TEMPLATE_NAME)
                .Combine(TopControlsViewBuilder.GetFingerprint(request))
                .Combine(SideBarViewBuilder.GetFingerprint(request));
        }
    }
}