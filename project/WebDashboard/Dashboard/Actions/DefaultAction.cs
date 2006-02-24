using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.FarmReport;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions
{
	public class DefaultAction : IAction
	{
		private readonly ILinkFactory linkFactory;

		public DefaultAction(ILinkFactory linkFactory)
		{
			this.linkFactory = linkFactory;
		}

		public IResponse Execute(IRequest request)
		{
			return new RedirectResponse(linkFactory.CreateFarmLink("", FarmReportFarmPlugin.ACTION_NAME).Url);
		}
	}
}