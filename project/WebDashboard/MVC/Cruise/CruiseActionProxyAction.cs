using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class CruiseActionProxyAction : IAction
	{
		private readonly ICruiseRequestFactory cruiseRequestFactory;
		private readonly ICruiseAction proxiedAction;

		public CruiseActionProxyAction(ICruiseAction proxiedAction, ICruiseRequestFactory cruiseRequestFactory)
		{
			this.proxiedAction = proxiedAction;
			this.cruiseRequestFactory = cruiseRequestFactory;
		}

		public IView Execute(IRequest request)
		{
			return proxiedAction.Execute(cruiseRequestFactory.CreateCruiseRequest(request));
		}
	}
}
