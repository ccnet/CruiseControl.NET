using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.WebDashboard.Config;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class CruiseActionFactory : IActionFactory
	{
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
				return new ViewAllBuildsAction();
			}
			else if (actionName == ADD_PROJECT_DISPLAY_ACTION_NAME)
			{
				return new DisplayAddProjectPageAction(
					new AddProjectModelGenerator(
					new ServerAggregatingCruiseManagerWrapper(new ConfigurationSettingsConfigGetter(), new RemoteCruiseManagerFactory())), 
					new AddProjectViewBuilder(new DefaultHtmlBuilder()));
			}
			else if (actionName == ADD_PROJECT_SAVE_ACTION_NAME)
			{
				return new SaveNewProjectAction(
					new AddProjectModelGenerator(
					new ServerAggregatingCruiseManagerWrapper(new ConfigurationSettingsConfigGetter(), new RemoteCruiseManagerFactory())), 
					new AddProjectViewBuilder(new DefaultHtmlBuilder()),
					new ServerAggregatingCruiseManagerWrapper(new ConfigurationSettingsConfigGetter(), new RemoteCruiseManagerFactory()),
					new NetReflectorProjectSerializer());
			}
			else
			{
				return new UnknownActionAction();
			}
		}
	}
}
