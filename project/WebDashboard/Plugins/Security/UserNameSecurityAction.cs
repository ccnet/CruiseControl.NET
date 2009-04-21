using Objection;
using System;
using System.Collections;
using System.Text;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Security;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;
using System.Web;

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
                    UserNameCredentials credentials = new UserNameCredentials(userName);
                    string password = cruiseRequest.Request.GetText("password");
                    if (!string.IsNullOrEmpty(password)) credentials["password"] = password;
                    string sessionToken = farmService.Login(cruiseRequest.ServerName, credentials);
                    if (string.IsNullOrEmpty(sessionToken)) throw new Exception("Login failed!");
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
            newCookie.Expires = DateTime.Now.AddMinutes(15);
            HttpContext.Current.Response.Cookies.Add(newCookie);
        }
        #endregion
        #endregion
    }
}
