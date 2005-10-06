using System;
using System.Web;
using ObjectWizard;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.WebDashboard.Configuration;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.ActionDecorators;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.FarmReport;

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
			giverManager.AddTypedInstance(typeof(IRequest), new AggregatedRequest(new NameValueCollectionRequest(request.Form, request.Path), new NameValueCollectionRequest(request.QueryString, request.Path)));

			giverManager.SetImplementationType(typeof(IUrlBuilder), typeof(DefaultUrlBuilder));
			giverManager.SetImplementationType(typeof(IMultiTransformer), typeof(PathMappingMultiTransformer));

			giverManager.SetDependencyImplementationForType(typeof(PathMappingMultiTransformer), typeof(IMultiTransformer), typeof (HtmlAwareMultiTransformer));

			IDashboardConfiguration config = (IDashboardConfiguration) giver.GiveObjectByType(typeof(IDashboardConfiguration));
			giverManager.AddTypedInstance(typeof(IDashboardConfiguration), config);

			IRemoteServicesConfiguration remoteServicesConfig = config.RemoteServices;
			giverManager.AddTypedInstance(typeof(IRemoteServicesConfiguration), remoteServicesConfig);

			IPluginConfiguration pluginConfig = config.PluginConfiguration;
			giverManager.AddTypedInstance(typeof(IPluginConfiguration), pluginConfig);
			
			// Need to get these into plugin setup
			// These plugins are currently disabled - this code will be required again when they are needed
//			giverAndRegistrar.SetDependencyImplementationForIdentifer(SaveNewProjectAction.ACTION_NAME, typeof(IPathMapper), typeof(PathMapperUsingHostName));
//			giverAndRegistrar.SetDependencyImplementationForIdentifer(SaveEditProjectAction.ACTION_NAME, typeof(IPathMapper), typeof(PathMapperUsingHostName));

			// ToDo - Refactor these plugin sections

			foreach (IPlugin plugin in pluginConfig.FarmPlugins)
			{
				foreach (INamedAction action in plugin.NamedActions)
				{
					giverManager.CreateInstanceMapping(action.ActionName, action.Action)
						.Decorate(typeof(CruiseActionProxyAction)).Decorate(typeof(ExceptionCatchingActionProxy)).Decorate(typeof(SiteTemplateActionDecorator));
				}
			}

			foreach (IPlugin plugin in pluginConfig.ServerPlugins)
			{
				foreach (INamedAction action in plugin.NamedActions)
				{
					giverManager.CreateInstanceMapping(action.ActionName, action.Action)
						.Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction)).Decorate(typeof(ExceptionCatchingActionProxy)).Decorate(typeof(SiteTemplateActionDecorator));
				}
			}

			foreach (IPlugin plugin in pluginConfig.ProjectPlugins)
			{
				foreach (INamedAction action in plugin.NamedActions)
				{
					giverManager.CreateInstanceMapping(action.ActionName, action.Action)
						.Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction)).Decorate(typeof(ExceptionCatchingActionProxy)).Decorate(typeof(SiteTemplateActionDecorator));
				}
			}

			// Even if the user hasn't specified to use this plugin, we still need it registered since there are links to it elsewhere
			try
			{
				giver.GiveObjectById(LatestBuildReportProjectPlugin.ACTION_NAME);
			}
			catch (ApplicationException)
			{
				IPlugin latestBuildPlugin = (IPlugin) giver.GiveObjectByType(typeof (LatestBuildReportProjectPlugin));
				giverManager.CreateInstanceMapping(latestBuildPlugin.NamedActions[0].ActionName, latestBuildPlugin.NamedActions[0].Action)
					.Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction)).Decorate(typeof(ExceptionCatchingActionProxy)).Decorate(typeof(SiteTemplateActionDecorator));
			}
			
			foreach (IPlugin plugin in pluginConfig.BuildPlugins)
			{
				foreach (INamedAction action in plugin.NamedActions)
				{
					giverManager.CreateInstanceMapping(action.ActionName,action.Action)
						.Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(BuildCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction)).Decorate(typeof(ExceptionCatchingActionProxy)).Decorate(typeof(SiteTemplateActionDecorator));
				}
			}

			// ToDo - make this kind of thing specifiable by Plugins (note that this action is not wrapped with a SiteTemplateActionDecorator
			// See BuildLogBuildPlugin for linked todo
			giverManager.CreateImplementationMapping(XmlBuildLogAction.ACTION_NAME, typeof(XmlBuildLogAction))
				.Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(BuildCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction));

			giverManager.CreateImplementationMapping(XmlReportAction.ACTION_NAME, typeof (XmlReportAction));
			
			return giver;
		}
	}
}