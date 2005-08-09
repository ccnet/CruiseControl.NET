using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class ServerCheckingProxyAction : ICruiseAction
	{
		private readonly IErrorViewBuilder errorViewBuilder;
		private readonly ICruiseAction proxiedAction;

		public ServerCheckingProxyAction(ICruiseAction proxiedAction, IErrorViewBuilder errorViewBuilder)
		{
			this.proxiedAction = proxiedAction;
			this.errorViewBuilder = errorViewBuilder;
		}

		public IResponse Execute(ICruiseRequest cruiseRequest)
		{
			if (cruiseRequest.ServerName == string.Empty)
			{
				return errorViewBuilder.BuildView(string.Format("Error - Action [{0}] expects Server to be specified in request", proxiedAction.GetType().FullName));
			}
			else
			{
				return proxiedAction.Execute(cruiseRequest);
			}
		}
	}
}
