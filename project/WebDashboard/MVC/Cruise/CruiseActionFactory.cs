using System;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.DeleteProject;

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
		public static readonly string SHOW_DELETE_PROJECT_ACTION_NAME = "ShowDeleteProject";
		public static readonly string DO_DELETE_PROJECT_ACTION_NAME = "DoDeleteProject";

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
				return CruiseActionProxyAction(ServerAndProjectCheckingProxyAction(ViewAllBuildsAction));
			}
			else if (actionName == ADD_PROJECT_DISPLAY_ACTION_NAME)
			{
				return DisplayAddProjectPageAction;
			}
			else if (actionName == ADD_PROJECT_SAVE_ACTION_NAME)
			{
				return SaveNewProjectAction;
			}
			else if (actionName == SHOW_DELETE_PROJECT_ACTION_NAME)
			{
				return CruiseActionProxyAction(ServerAndProjectCheckingProxyAction(ShowDeleteProjectAction));
			}
			else if (actionName == DO_DELETE_PROJECT_ACTION_NAME)
			{
				return CruiseActionProxyAction(ServerAndProjectCheckingProxyAction(DoDeleteProjectAction));
			}
			else
			{
				return new UnknownActionAction();
			}
		}

		public CruiseActionProxyAction CruiseActionProxyAction(ICruiseAction proxied)
		{
			return new CruiseActionProxyAction(proxied, dcFactory.NameValueCruiseRequestFactory);
		}

		public ServerAndProjectCheckingProxyAction ServerAndProjectCheckingProxyAction(ICruiseAction proxied)
		{
			return new ServerAndProjectCheckingProxyAction(proxied, SimpleErrorViewBuilder);
		}

		public SimpleErrorViewBuilder SimpleErrorViewBuilder
		{
			get { return new SimpleErrorViewBuilder(dcFactory.DefaultHtmlBuilder);}
		}

		public ShowDeleteProjectAction ShowDeleteProjectAction
		{
			get { return new ShowDeleteProjectAction( DeleteProjectHtmlViewBuilder );}
		}

		public DoDeleteProjectAction DoDeleteProjectAction
		{
			get { return new DoDeleteProjectAction( DeleteProjectHtmlViewBuilder, dcFactory.ServerAggregatingCruiseManagerWrapper );}
		}

		public DeleteProjectHtmlViewBuilder DeleteProjectHtmlViewBuilder
		{
			get { return new DeleteProjectHtmlViewBuilder(dcFactory.DefaultHtmlBuilder, dcFactory.DefaultUrlBuilder);}
		}

		public SaveNewProjectAction SaveNewProjectAction
		{
			get { return new SaveNewProjectAction( AddProjectModelGenerator, AddProjectViewBuilder, dcFactory.ServerAggregatingCruiseManagerWrapper, dcFactory.NetReflectorProjectSerializer); }
		}

		public DisplayAddProjectPageAction DisplayAddProjectPageAction
		{
			get { return new DisplayAddProjectPageAction(AddProjectModelGenerator, AddProjectViewBuilder); }
		}

		public AddProjectModelGenerator AddProjectModelGenerator
		{
			get { return new AddProjectModelGenerator(dcFactory.ServerAggregatingCruiseManagerWrapper); }
		}

		public AddProjectViewBuilder AddProjectViewBuilder
		{
			get { return new AddProjectViewBuilder(dcFactory.DefaultHtmlBuilder); }
		}

		public ViewAllBuildsAction ViewAllBuildsAction
		{
			get { return new ViewAllBuildsAction(new RecentBuildLister( dcFactory.DefaultHtmlBuilder, dcFactory.DefaultUrlBuilder, dcFactory.ServerAggregatingCruiseManagerWrapper, dcFactory.DefaultBuildNameFormatter)); } 
		}
	}
}
