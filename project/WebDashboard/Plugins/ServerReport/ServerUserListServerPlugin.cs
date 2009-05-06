using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Xml;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;
using System;
using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ServerReport
{
    [ReflectorType("serverUserListServerPlugin")]
    public class ServerUserListServerPlugin : ICruiseAction, IPlugin
    {
        #region Private consts
        private const string ActionName = "ViewServerUserList";
        private const string DiagnosticsActionName = "ViewUserSecurityDiagnostics";
        #endregion

        #region Private fields
        private readonly IFarmService farmService;
        private readonly IVelocityViewGenerator viewGenerator;
        private readonly ISessionRetriever sessionRetriever;
        private readonly IUrlBuilder urlBuilder;
        private string resetPassword = "password";
        #endregion

        #region Constructors
        public ServerUserListServerPlugin(IFarmService farmService, 
            IVelocityViewGenerator viewGenerator, 
            ISessionRetriever sessionRetriever,
            IUrlBuilder urlBuilder)
        {
            this.farmService = farmService;
            this.viewGenerator = viewGenerator;
            this.sessionRetriever = sessionRetriever;
            this.urlBuilder = urlBuilder;
        }
        #endregion

        #region Public properties
        public string LinkDescription
        {
            get { return "View User List"; }
        }

        public INamedAction[] NamedActions
        {
            get
            {
                return new INamedAction[] { new ImmutableNamedAction(ActionName, this),
                        new ImmutableNamedActionWithoutSiteTemplate(DiagnosticsActionName, this)
                    };
            }
        }

        /// <summary>
        /// The password to use when reseting the password.
        /// </summary>
        [ReflectorProperty("resetPassword", Required = false)]
        public string ResetPassword
        {
            get { return resetPassword; }
            set { resetPassword = value; }
        }
        #endregion

        #region Public methods
        public IResponse Execute(ICruiseRequest request)
        {
            string userName = HttpContext.Current.Request.Form["user"];
            string action = HttpContext.Current.Request.Form["action"];
            if (string.IsNullOrEmpty(userName))
            {
                return GenerateUserList(request, string.Empty, string.Empty);
            }
            else
            {
                if (action == "Reset password")
                {
                    string message = string.Empty;
                    string errorMsg = string.Empty;
                    try
                    {
                        farmService.ResetPassword(request.ServerName, sessionRetriever.SessionToken, userName, resetPassword);
                        message = "The password has been reset";
                    }
                    catch (Exception error)
                    {
                        errorMsg = error.Message;
                    }
                    return GenerateUserList(request, message, errorMsg);
                }
                else
                {
                    return GenerateUserDiagnostics(request, userName);
                }
            }
        }
        #endregion

        #region Private methdods
        private IResponse GenerateUserDiagnostics(ICruiseRequest request, string userName)
        {
            Hashtable velocityContext = new Hashtable();

            velocityContext["userName"] = userName;
            velocityContext["message"] = string.Empty;
            string sessionToken = request.RetrieveSessionToken(sessionRetriever);
            if (!StringUtil.IsBlank(request.ProjectName))
            {
                velocityContext["projectName"] = request.ProjectName;
                velocityContext["diagnostics"] = farmService.DiagnoseSecurityPermissions(request.ProjectSpecifier, sessionToken, userName);
            }
            else
            {
                velocityContext["diagnostics"] = farmService.DiagnoseSecurityPermissions(request.ServerSpecifier, sessionToken, userName);
            }

            return viewGenerator.GenerateView(@"UserDiagnostics.vm", velocityContext);
        }

        private IResponse GenerateUserList(ICruiseRequest request, string message, string error)
        {
            Hashtable velocityContext = new Hashtable();
            velocityContext["message"] = message;
            velocityContext["error"] = error;






            ArrayList links = new ArrayList();
            links.Add(new ServerLink(request.UrlBuilder, request.ServerSpecifier, "User List", ActionName));

            ProjectStatusListAndExceptions projects = farmService.GetProjectStatusListAndCaptureExceptions(request.ServerSpecifier, request.RetrieveSessionToken());
            foreach (ProjectStatusOnServer projectStatusOnServer in projects.StatusAndServerList)
            {
                DefaultProjectSpecifier projectSpecifier = new DefaultProjectSpecifier(projectStatusOnServer.ServerSpecifier, projectStatusOnServer.ProjectStatus.Name);
                links.Add(new ProjectLink(request.UrlBuilder, projectSpecifier, projectSpecifier.ProjectName, ServerUserListServerPlugin.ActionName));
            }
            velocityContext["projectLinks"] = links;
            string sessionToken = request.RetrieveSessionToken(sessionRetriever);
            List<UserDetails> allUsers = farmService.ListAllUsers(request.ServerSpecifier, sessionToken);
            foreach (UserDetails user in allUsers)
            {
                if (user.DisplayName == null) user.DisplayName = string.Empty;
            }
            velocityContext["users"] = allUsers;
            if (!StringUtil.IsBlank(request.ProjectName))
            {
                velocityContext["currentProject"] = request.ProjectName;
                velocityContext["diagnosticsCall"] = new ProjectLink(request.UrlBuilder, request.ProjectSpecifier, string.Empty, DiagnosticsActionName);
            }
            else
            {
                velocityContext["diagnosticsCall"] = new ServerLink(request.UrlBuilder, request.ServerSpecifier, string.Empty, DiagnosticsActionName);
            }

            return viewGenerator.GenerateView(@"UserList.vm", velocityContext);
        }
        #endregion
    }
}
