using System.Collections;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.Configuration;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.Security;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
    public class LoginViewBuilder
    {
        private readonly ICruiseRequest request;
        private readonly ILinkFactory linkFactory;
        private readonly IVelocityViewGenerator velocityViewGenerator;
        private readonly IDashboardConfiguration configuration;
        private readonly ISessionStorer storer;

        public LoginViewBuilder(ICruiseRequest request, ILinkFactory linkFactory, 
            IVelocityViewGenerator velocityViewGenerator, IDashboardConfiguration configuration,
            ISessionStorer storer)
        {
            this.request = request;
            this.linkFactory = linkFactory;
            this.velocityViewGenerator = velocityViewGenerator;
            this.configuration = configuration;
            this.storer = storer;
        }

        public HtmlFragmentResponse Execute()
        {
            Hashtable velocityContext = new Hashtable();
            velocityContext["changePassword"] = string.Empty;

            string serverName = request.ServerName;
            if (!string.IsNullOrEmpty(serverName) && (configuration.PluginConfiguration.SecurityPlugins.Length > 0))
            {
                if (string.IsNullOrEmpty(storer.SessionToken))
                {
                    velocityContext["action"] = linkFactory.CreateServerLink(request.ServerSpecifier,
                        "Login",
                        configuration.PluginConfiguration.SecurityPlugins[0].NamedActions[0].ActionName);
                }
                else
                {
                    velocityContext["action"] = linkFactory.CreateServerLink(request.ServerSpecifier,
                        "Logout",
                        LogoutSecurityAction.ActionName);

                    velocityContext["changePassword"] = linkFactory.CreateServerLink(request.ServerSpecifier,
                        "Change Password",
                        ChangePasswordSecurityAction.ActionName);
                }
            }
            else
            {
                velocityContext["action"] = string.Empty;
            }

            return velocityViewGenerator.GenerateView("LoginAction.vm", velocityContext);
        }
    }
}
