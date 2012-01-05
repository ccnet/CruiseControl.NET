using System;
using System.Collections;
using System.Web;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Remote.Messages;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.Security
{
    public class UserNameSecurityAction
        : ICruiseAction
    {
        #region Private fields
        private readonly IFarmService farmService;
        private readonly IVelocityViewGenerator viewGenerator;
        private readonly ISessionStorer storer;
        private bool hidePassword;
        #endregion

        #region Constructors
        public UserNameSecurityAction(IFarmService farmService, IVelocityViewGenerator viewGenerator,
            ISessionStorer storer, bool hidePassword)
        {
            this.farmService = farmService;
            this.viewGenerator = viewGenerator;
            this.storer = storer;
            this.hidePassword = hidePassword;
        }
        #endregion

        #region Public methods
        public IResponse Execute(ICruiseRequest cruiseRequest)
        {
            Hashtable velocityContext = new Hashtable();
            string userName = cruiseRequest.Request.GetText("userName");
            string template = @"UserNameLogin.vm";
            if (!string.IsNullOrEmpty(userName))
            {
                try
                {
                    LoginRequest credentials = new LoginRequest(userName);
                    string password = cruiseRequest.Request.GetText("password");
                    if (!string.IsNullOrEmpty(password)) credentials.AddCredential(LoginRequest.PasswordCredential, password);
                    string sessionToken = farmService.Login(cruiseRequest.ServerName, credentials);
                    if (string.IsNullOrEmpty(sessionToken)) throw new CruiseControlException("Login failed!");
                    storer.SessionToken = sessionToken;
                    AddSessionCookie(sessionToken);
                    template = "LoggedIn.vm";
                }
                catch (Exception error)
                {
                    velocityContext["errorMessage"] = error.Message;
                }
            }
            velocityContext["hidePassword"] = hidePassword;
            return viewGenerator.GenerateView(template, velocityContext);
        }
        #endregion

        #region Private methods
        #region AddSessionCookie()
        /// <summary>
        /// Adds the session cookie.
        /// </summary>
        /// <param name="sessionToken"></param>
        private void AddSessionCookie(string sessionToken)
        {
            var newCookie = new HttpCookie("CCNetSessionToken");
            newCookie.Value = sessionToken;
            newCookie.HttpOnly = true;
            // A session cookie is created when no newCookie.Expires is set
            HttpContext.Current.Response.Cookies.Add(newCookie);
        }
        #endregion
        #endregion
    }
}
