using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class CruiseActionFactory : IActionFactory
	{
		private readonly DashboardComponentFactory dcFactory;

		public CruiseActionFactory (DashboardComponentFactory dcFactory)
		{
			this.dcFactory = dcFactory;
		}

		public static readonly string ACTION_PARAMETER_PREFIX = "_action_";
		public static readonly string ADD_PROJECT_DISPLAY_ACTION_NAME = "AddProjectDisplay";
		public static readonly string ADD_PROJECT_SAVE_ACTION_NAME = "AddProjectSave";
		public static readonly string VIEW_ALL_BUILDS_ACTION_NAME = "ViewAllBuilds";

		public IAction Create(IRequest request)
		{
			string actionName = request.FindParameterStartingWith(ACTION_PARAMETER_PREFIX);
			if (actionName == "")
			{
				return new DefaultCruiseAction();
			}
			actionName = actionName.Substring(ACTION_PARAMETER_PREFIX.Length);
			if (actionName == VIEW_ALL_BUILDS_ACTION_NAME)
			{
				return new ViewAllBuildsAction(
					new RecentBuildLister(dcFactory.DefaultHtmlBuilder, dcFactory.DefaultUrlBuilder, dcFactory.ServerAggregatingCruiseManagerWrapper, dcFactory.DefaultBuildNameFormatter),
					dcFactory.NameValueCruiseRequestFactory);
			}
			else if (actionName == ADD_PROJECT_DISPLAY_ACTION_NAME)
			{
				return new DisplayAddProjectPageAction(
					new AddProjectModelGenerator(dcFactory.ServerAggregatingCruiseManagerWrapper),
					new AddProjectViewBuilder(dcFactory.DefaultHtmlBuilder));
			}
			else if (actionName == ADD_PROJECT_SAVE_ACTION_NAME)
			{
				return new SaveNewProjectAction(
					new AddProjectModelGenerator(dcFactory.ServerAggregatingCruiseManagerWrapper),
					new AddProjectViewBuilder(dcFactory.DefaultHtmlBuilder),
					dcFactory.ServerAggregatingCruiseManagerWrapper, 
					dcFactory.NetReflectorProjectSerializer);
			}
			else
			{
				return new UnknownActionAction();
			}
		}
	}
}
