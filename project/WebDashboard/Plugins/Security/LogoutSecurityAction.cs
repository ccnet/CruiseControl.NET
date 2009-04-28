using Objection;
using System;
using System.Collections;
using System.Text;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;
using System.Web;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.Security
{
    public class LogoutSecurityAction
        : ICruiseAction
    {
        #region Public consts
        public const string ActionName = "ServerLogout";
        #endregion

        #region Private fields
        private readonly IFarmService farmService;
        private readonly IVelocityViewGenerator viewGenerator;
        private readonly ISessionStorer storer;
        #endregion

        #region Constructors
        public LogoutSecurityAction(IFarmService farmService, IVelocityViewGenerator viewGenerator,
            ISessionStorer storer)
        {
            this.farmService = farmService;
            this.viewGenerator = viewGenerator;
            this.storer = storer;
        }
        #endregion

        #region Public methods
        public IResponse Execute(ICruiseRequest cruiseRequest)
        {
            Hashtable velocityContext = new Hashtable();
            if (!string.IsNullOrEmpty(storer.SessionToken)) farmService.Logout(cruiseRequest.ServerName, storer.SessionToken);
            storer.SessionToken = null;
            var newCookie = new HttpCookie("CCNetSessionToken");
            newCookie.HttpOnly = true;
            newCookie.Expires = DateTime.Now.AddDays(-1);
            HttpContext.Current.Response.Cookies.Add(newCookie);
            return viewGenerator.GenerateView("LoggedOut.vm", velocityContext);
        }
        #endregion
    }
}
