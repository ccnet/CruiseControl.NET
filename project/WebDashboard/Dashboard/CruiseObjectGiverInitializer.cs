using System.Web;
using System.Web.UI;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.WebDashboard.Config;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.AddProject;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.DeleteProject;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.EditProject;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewAllBuilds;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewProjectReport;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewServerLog;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class CruiseObjectGiverInitializer
	{
		private ObjectGiverAndRegistrar giverAndRegistrar;

		public CruiseObjectGiverInitializer(ObjectGiverAndRegistrar giverAndRegistrar)
		{
			this.giverAndRegistrar = giverAndRegistrar;
		}

		public ObjectGiver InitializeGiverForRequest(HttpRequest request, HttpContext context, Control control)
		{
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
				typeof(SaveNewProjectAction)).Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction)).Decorate(typeof(SecurityCheckingProxyAction));

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

			IConfigurationGetter configurationGetter = (IConfigurationGetter) giverAndRegistrar.GiveObjectByType(typeof(IConfigurationGetter));

			if (configurationGetter == null)
			{
				throw new CruiseControlException("Unable to instantiate configuration getter");
			}

			foreach (IPluginSpecification pluginSpecification in (IPluginSpecification[]) configurationGetter.GetConfigFromSection("CCNet/buildPlugins"))
			{
				IBuildPlugin buildPlugin = giverAndRegistrar.GiveObjectByType(pluginSpecification.Type) as IBuildPlugin;
				if (buildPlugin == null)
				{
					throw new CruiseControlException(pluginSpecification.TypeName + " is not a IBuildPlugin");
				}
				giverAndRegistrar.CreateImplementationMapping(buildPlugin.ActionName,buildPlugin.ActionType)
					.Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(BuildCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction));
			}

			return giverAndRegistrar;
		}
	}
}