using System.Web.UI;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class BuildCheckingProxyAction : ICruiseAction
	{
		private readonly IErrorViewBuilder errorViewBuilder;
		private readonly ICruiseAction proxiedAction;

		public BuildCheckingProxyAction(ICruiseAction proxiedAction, IErrorViewBuilder errorViewBuilder)
		{
			this.proxiedAction = proxiedAction;
			this.errorViewBuilder = errorViewBuilder;
		}

		public IView Execute(ICruiseRequest cruiseRequest)
		{
			if (cruiseRequest.BuildName == string.Empty)
			{
				return errorViewBuilder.BuildView(string.Format("Error - Action [{0}] expects Build Name to be specified in request", proxiedAction.GetType().FullName));
			}
			else
			{
				return proxiedAction.Execute(cruiseRequest);
			}
		}
	}
}
