using System.Web;
using ObjectWizard;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.WebDashboard.Configuration;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.ActionDecorators;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class CruiseObjectGiverInitializer
	{
		private ObjectGiverManager giverManager;

		public CruiseObjectGiverInitializer(ObjectGiverManager giverAndRegistrar)
		{
			this.giverManager = giverAndRegistrar;
		}

		// This all needs breaking up a bit (to make it testable, apart from anything else)
		public ObjectGiver SetupObjectGiverForRequest(HttpContext context)
		{
			ObjectGiver giver = (ObjectGiver) giverManager; // Yuch - put this in Object Wizard somewhere
			giverManager.AddTypedInstance(typeof(ObjectGiver), giverManager);
			giverManager.AddTypedInstance(typeof(HttpContext), context);
			HttpRequest request = context.Request;
			giverManager.AddTypedInstance(typeof(HttpRequest), request);

			// Add functionality to object giver to handle this?
			giverManager.AddTypedInstance(typeof(IRequest), new AggregatedRequest(new NameValueCollectionRequest(request.Form), new NameValueCollectionRequest(request.QueryString)));
			
			giverManager.SetImplementationType(typeof(IPathMapper), typeof(HttpPathMapper));
			giverManager.SetImplementationType(typeof(IMultiTransformer), typeof(PathMappingMultiTransformer));

			giverManager.SetDependencyImplementationForType(typeof(PathMappingMultiTransformer), typeof(IMultiTransformer), typeof (HtmlAwareMultiTransformer));

			// Need to get these into plugin setup
			// These plugins are currently disabled - this code will be required again when they are needed
//			giverAndRegistrar.SetDependencyImplementationForIdentifer(SaveNewProjectAction.ACTION_NAME, typeof(IPathMapper), typeof(PathMapperUsingHostName));
//			giverAndRegistrar.SetDependencyImplementationForIdentifer(SaveEditProjectAction.ACTION_NAME, typeof(IPathMapper), typeof(PathMapperUsingHostName));

//			IConfigurationGetter configurationGetter = (IConfigurationGetter) giver.GiveObjectByType(typeof(IConfigurationGetter));
//			if (configurationGetter == null)
//			{
//				throw new CruiseControlException("Unable to instantiate configuration getter");
//			}

			giverManager.SetImplementationType(typeof(IPluginConfiguration), typeof(PluginConfigurationLoader));
			IPluginConfiguration config = (IPluginConfiguration) giver.GiveObjectByType(typeof(IPluginConfiguration));

			// ToDo - Refactor these plugin sections

			foreach (IPlugin plugin in config.FarmPlugins)
			{
				foreach (INamedAction action in plugin.NamedActions)
				{
					giverManager.CreateInstanceMapping(action.ActionName, action.Action)
						.Decorate(typeof(SiteTemplateActionDecorator)).Decorate(typeof(CruiseActionProxyAction)).Decorate(typeof(ExceptionCatchingActionProxy));
				}
			}

			foreach (IPlugin plugin in config.ServerPlugins)
			{
				foreach (INamedAction action in plugin.NamedActions)
				{
					giverManager.CreateInstanceMapping(action.ActionName, action.Action)
						.Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(SiteTemplateActionDecorator)).Decorate(typeof(CruiseActionProxyAction)).Decorate(typeof(ExceptionCatchingActionProxy));
				}
			}

			foreach (IPlugin plugin in config.ProjectPlugins)
			{
				foreach (INamedAction action in plugin.NamedActions)
				{
					giverManager.CreateInstanceMapping(action.ActionName, action.Action)
						.Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(SiteTemplateActionDecorator)).Decorate(typeof(CruiseActionProxyAction)).Decorate(typeof(ExceptionCatchingActionProxy));
				}
			}

			foreach (IPlugin plugin in config.BuildPlugins)
			{
				foreach (INamedAction action in plugin.NamedActions)
				{
					giverManager.CreateInstanceMapping(action.ActionName,action.Action)
						.Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(BuildCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(SiteTemplateActionDecorator)).Decorate(typeof(CruiseActionProxyAction)).Decorate(typeof(ExceptionCatchingActionProxy));
				}
			}

			// ToDo - make this kind of thing specifiable by Plugins (note that this action is not wrapped with a SiteTemplateActionDecorator
			// See BuildLogBuildPlugin for linked todo
			giverManager.CreateImplementationMapping(XmlBuildLogAction.ACTION_NAME, typeof(XmlBuildLogAction))
				.Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(BuildCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction));

			return giver;
		}
	}
}