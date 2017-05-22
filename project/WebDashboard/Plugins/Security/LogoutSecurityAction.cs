using System;
using System.Collections;
using System.Web;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

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
			string sessionToken = cruiseRequest.RetrieveSessionToken();
			if (!string.IsNullOrEmpty(sessionToken))
				farmService.Logout(cruiseRequest.ServerName, sessionToken);
			storer.StoreSessionToken(null);
			storer.StoreDisplayName(null);
			return viewGenerator.GenerateView("LoggedOut.vm", velocityContext);
		}
		#endregion
	}
}
