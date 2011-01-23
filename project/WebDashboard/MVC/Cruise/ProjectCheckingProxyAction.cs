using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class ProjectCheckingProxyAction : ICruiseAction
	{
		private readonly IErrorViewBuilder errorViewBuilder;
		private readonly ICruiseAction proxiedAction;

		public ProjectCheckingProxyAction(ICruiseAction proxiedAction, IErrorViewBuilder errorViewBuilder)
		{
			this.proxiedAction = proxiedAction;
			this.errorViewBuilder = errorViewBuilder;
		}

		public IResponse Execute(ICruiseRequest cruiseRequest)
		{
			if (cruiseRequest.ProjectName == string.Empty)
			{
				return errorViewBuilder.BuildView(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Error - Action [{0}] expects Project to be specified in request", proxiedAction.GetType().FullName));
			}
			else
			{
				return proxiedAction.Execute(cruiseRequest);
			}
		}
	}
}
