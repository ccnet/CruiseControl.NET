using System.Web.UI;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class ServerAndProjectCheckingProxyAction : ICruiseAction
	{
		private readonly IErrorViewBuilder errorViewBuilder;
		private readonly ICruiseAction proxiedAction;

		public ServerAndProjectCheckingProxyAction(ICruiseAction proxiedAction, IErrorViewBuilder errorViewBuilder)
		{
			this.proxiedAction = proxiedAction;
			this.errorViewBuilder = errorViewBuilder;
		}

		public Control Execute(ICruiseRequest cruiseRequest)
		{
			if (cruiseRequest.ServerName == string.Empty || cruiseRequest.ProjectName == string.Empty)
			{
				return errorViewBuilder.BuildView(string.Format("Error - Action [{0}] expects both Server and Project to be specified in request", proxiedAction.GetType().FullName));
			}
			else
			{
				return proxiedAction.Execute(cruiseRequest);
			}
		}
	}
}
