using System.Web;
using System.Web.UI;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.AddProject;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.DeleteProject;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.EditProject;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewAllBuilds;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewBuildLog;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewBuildReport;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewProjectReport;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewServerLog;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class CruiseObjectGiverFactory
	{
		public static ObjectGiver CreateGiverForRequest(HttpRequest request, HttpContext context, Control control)
		{
			ObjectGiverAndRegistrar giverAndRegistrar = new ObjectGiverAndRegistrar();
			giverAndRegistrar.AddTypedObject(typeof(HttpRequest), request);
			giverAndRegistrar.AddTypedObject(typeof(HttpContext), context);
			giverAndRegistrar.AddTypedObject(typeof(Control), control);
			giverAndRegistrar.AddTypedObject(typeof(ObjectGiver), giverAndRegistrar);

			// Add functionality to object giver to handle this?
			giverAndRegistrar.AddTypedObject(typeof(IRequest), new AggregatedRequest(new NameValueCollectionRequest(request.Form), new NameValueCollectionRequest(request.QueryString)));
			
			giverAndRegistrar.SetImplementationType(typeof(IPathMapper), typeof(HttpPathMapper));
			giverAndRegistrar.SetImplementationType(typeof(IMultiTransformer), typeof(PathMappingMultiTransformer));

			giverAndRegistrar.SetDependencyImplementationForType(typeof(DefaultUserRequestSpecificSideBarViewBuilder), typeof(IRecentBuildsViewBuilder), typeof(DecoratingRecentBuildsPanelBuilder));
			giverAndRegistrar.SetDependencyImplementationForType(typeof(DecoratingRecentBuildsPanelBuilder), typeof(IRecentBuildsViewBuilder), typeof(RecentBuildLister));
			giverAndRegistrar.SetDependencyImplementationForType(typeof(PathMappingMultiTransformer), typeof(IMultiTransformer), typeof (HtmlAwareMultiTransformer));

			// Would be good to test these next 2
			giverAndRegistrar.SetDependencyImplementationForIdentifer(SaveNewProjectAction.ACTION_NAME, typeof(IPathMapper), typeof(PathMapperUsingHostName));
			giverAndRegistrar.SetDependencyImplementationForIdentifer(SaveEditProjectAction.ACTION_NAME, typeof(IPathMapper), typeof(PathMapperUsingHostName));

			giverAndRegistrar.CreateImplementationMapping(ViewAllBuildsAction.ACTION_NAME, 
				typeof(ViewAllBuildsAction)).Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction));

			giverAndRegistrar.CreateImplementationMapping(DisplayAddProjectPageAction.ACTION_NAME, 
				typeof(DisplayAddProjectPageAction)).Decorate(typeof(SecurityCheckingProxyAction));

			giverAndRegistrar.CreateImplementationMapping(SaveNewProjectAction.ACTION_NAME, 
				typeof(SaveNewProjectAction)).Decorate(typeof(SecurityCheckingProxyAction));

			giverAndRegistrar.CreateImplementationMapping(DisplayEditProjectPageAction.ACTION_NAME, 
				typeof(DisplayEditProjectPageAction)).Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction)).Decorate(typeof(SecurityCheckingProxyAction));

			giverAndRegistrar.CreateImplementationMapping(SaveEditProjectAction.ACTION_NAME, 
				typeof(SaveEditProjectAction)).Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction)).Decorate(typeof(SecurityCheckingProxyAction));

			giverAndRegistrar.CreateImplementationMapping(ViewProjectReportAction.ACTION_NAME, 
				typeof(ViewProjectReportAction)).Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction));

			giverAndRegistrar.CreateImplementationMapping(ShowDeleteProjectAction.ACTION_NAME, 
				typeof(ShowDeleteProjectAction)).Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction)).Decorate(typeof(SecurityCheckingProxyAction));

			giverAndRegistrar.CreateImplementationMapping(DoDeleteProjectAction.ACTION_NAME, 
				typeof(DoDeleteProjectAction)).Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction)).Decorate(typeof(SecurityCheckingProxyAction));

			giverAndRegistrar.CreateImplementationMapping(ViewServerLogAction.ACTION_NAME, 
				typeof(ViewServerLogAction)).Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction));

			giverAndRegistrar.CreateImplementationMapping(ViewBuildReportAction.ACTION_NAME, 
				typeof(ViewBuildReportAction)).Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(BuildCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction));

			giverAndRegistrar.CreateImplementationMapping(ViewBuildLogAction.ACTION_NAME, 
				typeof(ViewBuildLogAction)).Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(BuildCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction));

			giverAndRegistrar.CreateImplementationMapping(ViewTestDetailsBuildReportAction.ACTION_NAME, 
				typeof(ViewTestDetailsBuildReportAction)).Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(BuildCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction));

			giverAndRegistrar.CreateImplementationMapping(ViewTestTimingsBuildReportAction.ACTION_NAME, 
				typeof(ViewTestTimingsBuildReportAction)).Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(BuildCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction));

			giverAndRegistrar.CreateImplementationMapping(ViewFxCopBuildReportAction.ACTION_NAME, 
				typeof(ViewFxCopBuildReportAction)).Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(BuildCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction));

			return giverAndRegistrar;
		}
	}
}