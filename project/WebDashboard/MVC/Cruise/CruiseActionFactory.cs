using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.DeleteProject;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewBuildLog;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewServerLog;

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
		public static readonly string EDIT_PROJECT_DISPLAY_ACTION_NAME = "EditProjectDisplay";
		public static readonly string EDIT_PROJECT_SAVE_ACTION_NAME = "EditProjectSave";
		public static readonly string VIEW_ALL_BUILDS_ACTION_NAME = "ViewAllBuilds";
		public static readonly string SHOW_DELETE_PROJECT_ACTION_NAME = "ShowDeleteProject";
		public static readonly string DO_DELETE_PROJECT_ACTION_NAME = "DoDeleteProject";
		public static readonly string VIEW_SERVER_LOG_ACTION_NAME = "ViewServerLog";
		public static readonly string VIEW_BUILD_LOG_ACTION_NAME = "ViewBuildLog";
		public static readonly string VIEW_PROJECT_REPORT_ACTION_NAME = "ViewProjectReport";

		public IAction Create(IRequest request)
		{
			return ExceptionCatchingActionProxy(CreateSpecificAction(request));
		}

		public IAction CreateSpecificAction(IRequest request)
		{
			string actionName = request.FindParameterStartingWith(ACTION_PARAMETER_PREFIX);
			if (actionName == "")
			{
				return new DefaultCruiseAction();
			}
			actionName = actionName.Substring(ACTION_PARAMETER_PREFIX.Length);
			if (actionName == VIEW_ALL_BUILDS_ACTION_NAME)
			{
				return CruiseActionProxyAction(ProjectCheckingProxyAction(ServerCheckingProxyAction(ViewAllBuildsAction)));
			}
			else if (actionName == ADD_PROJECT_DISPLAY_ACTION_NAME)
			{
				return SecurityCheckingProxyAction(DisplayAddProjectPageAction);
			}
			else if (actionName == ADD_PROJECT_SAVE_ACTION_NAME)
			{
				return SecurityCheckingProxyAction(SaveNewProjectAction);
			}
			else if (actionName == EDIT_PROJECT_DISPLAY_ACTION_NAME)
			{
				return SecurityCheckingProxyAction(CruiseActionProxyAction(ProjectCheckingProxyAction(ServerCheckingProxyAction(DisplayEditProjectPageAction))));
			}
			else if (actionName == EDIT_PROJECT_SAVE_ACTION_NAME)
			{
				return SecurityCheckingProxyAction(CruiseActionProxyAction(ProjectCheckingProxyAction(ServerCheckingProxyAction(SaveEditProjectAction))));
			}
			else if (actionName == VIEW_PROJECT_REPORT_ACTION_NAME)
			{
				return CruiseActionProxyAction(ProjectCheckingProxyAction(ServerCheckingProxyAction(ViewProjectReportAction)));
			}
			else if (actionName == SHOW_DELETE_PROJECT_ACTION_NAME)
			{
				return SecurityCheckingProxyAction(CruiseActionProxyAction(ProjectCheckingProxyAction(ServerCheckingProxyAction(ShowDeleteProjectAction))));
			}
			else if (actionName == DO_DELETE_PROJECT_ACTION_NAME)
			{
				return SecurityCheckingProxyAction(CruiseActionProxyAction(ProjectCheckingProxyAction(ServerCheckingProxyAction(DoDeleteProjectAction))));
			}
			else if (actionName == VIEW_SERVER_LOG_ACTION_NAME)
			{
				return CruiseActionProxyAction(ServerCheckingProxyAction(ViewServerLogAction));
			}
			else if (actionName == VIEW_BUILD_LOG_ACTION_NAME)
			{
				return CruiseActionProxyAction(BuildCheckingProxyAction(ProjectCheckingProxyAction(ServerCheckingProxyAction(ViewBuildLogAction))));
			}
			else
			{
				return new UnknownActionAction();
			}
		}

		public ExceptionCatchingActionProxy ExceptionCatchingActionProxy(IAction proxied)
		{
			return new ExceptionCatchingActionProxy(proxied)	;
		}

		public SecurityCheckingProxyAction SecurityCheckingProxyAction(IAction proxied)
		{
			return new SecurityCheckingProxyAction(proxied, dcFactory.ConfigurationSettingsConfigGetter);
		}

		public CruiseActionProxyAction CruiseActionProxyAction(ICruiseAction proxied)
		{
			return new CruiseActionProxyAction(proxied, dcFactory.NameValueCruiseRequestFactory);
		}

		public BuildCheckingProxyAction BuildCheckingProxyAction(ICruiseAction proxied)
		{
			return new BuildCheckingProxyAction(proxied, SimpleErrorViewBuilder);
		}

		public ProjectCheckingProxyAction ProjectCheckingProxyAction(ICruiseAction proxied)
		{
			return new ProjectCheckingProxyAction(proxied, SimpleErrorViewBuilder);
		}

		public ServerCheckingProxyAction ServerCheckingProxyAction(ICruiseAction proxied)
		{
			return new ServerCheckingProxyAction(proxied, SimpleErrorViewBuilder);
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

		public ViewProjectReportAction ViewProjectReportAction
		{
			get { return new ViewProjectReportAction();}
		}

		public DeleteProjectHtmlViewBuilder DeleteProjectHtmlViewBuilder
		{
			get { return new DeleteProjectHtmlViewBuilder(dcFactory.DefaultHtmlBuilder, dcFactory.DefaultUrlBuilderWithHttpPathMapper);}
		}

		public SaveNewProjectAction SaveNewProjectAction
		{
			get { return new SaveNewProjectAction( AddProjectModelGenerator, AddProjectViewBuilder, dcFactory.ServerAggregatingCruiseManagerWrapper, dcFactory.NetReflectorProjectSerializer, dcFactory.DefaultUrlBuilderWithPathMapperUsingHostName); }
		}

		public DisplayAddProjectPageAction DisplayAddProjectPageAction
		{
			get { return new DisplayAddProjectPageAction(AddProjectModelGenerator, AddProjectViewBuilder); }
		}

		public DisplayEditProjectPageAction DisplayEditProjectPageAction
		{
			get { return new DisplayEditProjectPageAction(AddProjectModelGenerator, AddProjectViewBuilder, dcFactory.ServerAggregatingCruiseManagerWrapper, dcFactory.NetReflectorProjectSerializer); }
		}

		public SaveEditProjectAction SaveEditProjectAction
		{
			get { return new SaveEditProjectAction(AddProjectModelGenerator, AddProjectViewBuilder, dcFactory.ServerAggregatingCruiseManagerWrapper, dcFactory.NetReflectorProjectSerializer, dcFactory.DefaultUrlBuilderWithPathMapperUsingHostName); }
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
			get { return new ViewAllBuildsAction(new RecentBuildLister( dcFactory.DefaultHtmlBuilder, dcFactory.DefaultUrlBuilderWithHttpPathMapper, dcFactory.ServerAggregatingCruiseManagerWrapper, dcFactory.DefaultBuildNameFormatter)); } 
		}

		private ViewServerLogAction ViewServerLogAction
		{
			get { return new ViewServerLogAction(dcFactory.ServerAggregatingCruiseManagerWrapper); }
		}

		private ViewBuildLogAction ViewBuildLogAction
		{
			get { return new ViewBuildLogAction(dcFactory.CachingBuildRetriever); }
		}
	}
}
