using System.Web.UI;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class ServerAndProjectCheckingProxyAction : IAction
	{
		private readonly ICruiseRequestFactory cruiseRequestFactory;
		private readonly IErrorViewBuilder errorViewBuilder;
		private readonly IAction proxiedAction;

		public ServerAndProjectCheckingProxyAction(IAction proxiedAction, IErrorViewBuilder errorViewBuilder, ICruiseRequestFactory cruiseRequestFactory)
		{
			this.proxiedAction = proxiedAction;
			this.errorViewBuilder = errorViewBuilder;
			this.cruiseRequestFactory = cruiseRequestFactory;
		}

		public Control Execute(IRequest request)
		{
			ICruiseRequest cruiseRequest = cruiseRequestFactory.CreateCruiseRequest(request);
			if (cruiseRequest.ServerName == string.Empty || cruiseRequest.ProjectName == string.Empty)
			{
				return errorViewBuilder.BuildView(string.Format("Error - Action [{0}] expects both Server and Project to be specified in request", proxiedAction.GetType().FullName));
			}
			else
			{
				return proxiedAction.Execute(request);
			}
		}
	}
}
