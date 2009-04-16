using Manoli.Utils.CSharpFormat;
using System.Collections;
using System.IO;
using System.Web;
using System.Xml;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ServerReport
{
    [ReflectorType("serverSecurityConfigurationServerPlugin")]
    public class ServerSecurityConfigurationServerPlugin : ICruiseAction, IPlugin
    {
        private const string ActionName = "ViewServerSecurityConfiguration";
        private readonly IFarmService farmService;
        private readonly IVelocityViewGenerator viewGenerator;
        private readonly ISessionRetriever sessionRetriever;

        public ServerSecurityConfigurationServerPlugin(IFarmService farmService, 
            IVelocityViewGenerator viewGenerator, 
            ISessionRetriever sessionRetriever)
        {
            this.farmService = farmService;
            this.viewGenerator = viewGenerator;
            this.sessionRetriever = sessionRetriever;
        }

        public IResponse Execute(ICruiseRequest request)
        {
            Hashtable velocityContext = new Hashtable();
            ArrayList links = new ArrayList();
            links.Add(new ServerLink(request.UrlBuilder, request.ServerSpecifier, "Server Security Configuration", ActionName));

            ProjectStatusListAndExceptions projects = farmService.GetProjectStatusListAndCaptureExceptions(request.ServerSpecifier);
            foreach (ProjectStatusOnServer projectStatusOnServer in projects.StatusAndServerList)
            {
                DefaultProjectSpecifier projectSpecifier = new DefaultProjectSpecifier(projectStatusOnServer.ServerSpecifier, projectStatusOnServer.ProjectStatus.Name);
                links.Add(new ProjectLink(request.UrlBuilder, projectSpecifier, projectSpecifier.ProjectName, ServerSecurityConfigurationServerPlugin.ActionName));
            }
            velocityContext["projectLinks"] = links;
            string sessionToken = request.RetrieveSessionToken(sessionRetriever);
            string securityConfig = farmService.GetServerSecurity(request.ServerSpecifier, sessionToken);
            XmlDocument document = new XmlDocument();
            document.LoadXml(securityConfig);
            if (StringUtil.IsBlank(request.ProjectName))
            {
                securityConfig = document.SelectSingleNode("/security/manager").OuterXml;
            }
            else
            {
                velocityContext["currentProject"] = request.ProjectSpecifier.ProjectName;
                string xpath = string.Format("/security/projects/projectSecurity[name='{0}']/authorisation", request.ProjectSpecifier.ProjectName);
                securityConfig = document.SelectSingleNode(xpath).OuterXml;
            }
            string xmlData = FormatXml(securityConfig);
            velocityContext["log"] = xmlData;

            return viewGenerator.GenerateView(@"SecurityConfiguration.vm", velocityContext);
        }

        public string LinkDescription
        {
            get { return "View Security Configuration"; }
        }

        public INamedAction[] NamedActions
        {
            get { return new INamedAction[] { new ImmutableNamedAction(ActionName, this) }; }
        }

        private string FormatXml(string projectXml)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(projectXml);
            StringWriter buffer = new StringWriter();
            XmlTextWriter writer = new XmlTextWriter(buffer);
            writer.Formatting = Formatting.Indented;
            document.WriteTo(writer);
            HtmlFormat formatter = new HtmlFormat();
            formatter.LineNumbers = false;
            formatter.Alternate = false;
            formatter.EmbedStyleSheet = false;
            return formatter.FormatCode(buffer.ToString());
        }
    }
}
