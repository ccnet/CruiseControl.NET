using System;
using System.Web;
using Objection;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.WebDashboard.Configuration;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.ActionDecorators;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.FarmReport;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class CruiseObjectSourceInitializer
	{
		private ObjectionManager objectionManager;

		public CruiseObjectSourceInitializer(ObjectionManager objectionManager)
		{
			this.objectionManager = objectionManager;
		}

		// This all needs breaking up a bit (to make it testable, apart from anything else)
		public ObjectSource SetupObjectSourceForRequest(HttpContext context)
		{
			ObjectSource objectSource = (ObjectSource) objectionManager; // Yuch - put this in Object Wizard somewhere
			objectionManager.AddInstanceForType(typeof(ObjectSource), objectionManager);

			objectionManager.AddInstanceForType(typeof(HttpContext), context);
			HttpRequest request = context.Request;
			objectionManager.AddInstanceForType(typeof(HttpRequest), request);

			objectionManager.AddInstanceForType(typeof(IRequest), 
				new AggregatedRequest(
					new NameValueCollectionRequest(request.Form, request.Path, request.RawUrl, request.ApplicationPath), 
					new NameValueCollectionRequest(request.QueryString, request.Path, request.RawUrl, request.ApplicationPath)));

			objectionManager.AddInstanceForType(typeof(IUrlBuilder),
				new AbsolutePathUrlBuilderDecorator(
					new DefaultUrlBuilder(),
					request.ApplicationPath));

			objectionManager.SetImplementationType(typeof(ICruiseRequest), typeof(RequestWrappingCruiseRequest));

			objectionManager.SetImplementationType(typeof(IMultiTransformer), typeof(PathMappingMultiTransformer));

			objectionManager.SetDependencyImplementationForType(typeof(PathMappingMultiTransformer), typeof(IMultiTransformer), typeof (HtmlAwareMultiTransformer));

			IDashboardConfiguration config = (IDashboardConfiguration) objectSource.GetByType(typeof(IDashboardConfiguration));
			objectionManager.AddInstanceForType(typeof(IDashboardConfiguration), config);

			IRemoteServicesConfiguration remoteServicesConfig = config.RemoteServices;
			objectionManager.AddInstanceForType(typeof(IRemoteServicesConfiguration), remoteServicesConfig);

			IPluginConfiguration pluginConfig = config.PluginConfiguration;
			objectionManager.AddInstanceForType(typeof(IPluginConfiguration), pluginConfig);
			
			// ToDo - Refactor these plugin sections

			foreach (IPlugin plugin in pluginConfig.FarmPlugins)
			{
				foreach (INamedAction action in plugin.NamedActions)
				{
					objectionManager.AddInstanceForName(action.ActionName, action.Action)
						.Decorate(typeof(CruiseActionProxyAction)).Decorate(typeof(ExceptionCatchingActionProxy)).Decorate(typeof(SiteTemplateActionDecorator));
				}
			}

			foreach (IPlugin plugin in pluginConfig.ServerPlugins)
			{
				foreach (INamedAction action in plugin.NamedActions)
				{
					objectionManager.AddInstanceForName(action.ActionName, action.Action)
						.Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction)).Decorate(typeof(ExceptionCatchingActionProxy)).Decorate(typeof(SiteTemplateActionDecorator));
				}
			}

			foreach (IPlugin plugin in pluginConfig.ProjectPlugins)
			{
				foreach (INamedAction action in plugin.NamedActions)
				{
					objectionManager.AddInstanceForName(action.ActionName, action.Action)
						.Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction)).Decorate(typeof(ExceptionCatchingActionProxy)).Decorate(typeof(SiteTemplateActionDecorator));
				}
			}

			// Even if the user hasn't specified to use this plugin, we still need it registered since there are links to it elsewhere
			try
			{
				objectSource.GetByName(LatestBuildReportProjectPlugin.ACTION_NAME);
			}
			catch (ApplicationException)
			{
				IPlugin latestBuildPlugin = (IPlugin) objectSource.GetByType(typeof (LatestBuildReportProjectPlugin));
				objectionManager.AddInstanceForName(latestBuildPlugin.NamedActions[0].ActionName, latestBuildPlugin.NamedActions[0].Action)
					.Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction)).Decorate(typeof(ExceptionCatchingActionProxy)).Decorate(typeof(SiteTemplateActionDecorator));
			}
			
			foreach (IPlugin plugin in pluginConfig.BuildPlugins)
			{
				foreach (INamedAction action in plugin.NamedActions)
				{
					objectionManager.AddInstanceForName(action.ActionName,action.Action)
						.Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(BuildCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction))
						.Decorate(typeof(CachingActionProxy)).Decorate(typeof(ExceptionCatchingActionProxy)).Decorate(typeof(SiteTemplateActionDecorator));
				}
			}

			// ToDo - make this kind of thing specifiable by Plugins (note that this action is not wrapped with a SiteTemplateActionDecorator
			// See BuildLogBuildPlugin for linked todo
			objectionManager.AddTypeForName(XmlBuildLogAction.ACTION_NAME, typeof(XmlBuildLogAction))
				.Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(BuildCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction));

			// TODO - Xml Exceptions?
			objectionManager.AddTypeForName(ForceBuildXmlAction.ACTION_NAME, typeof(ForceBuildXmlAction))
				.Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction));

			objectionManager.AddTypeForName(XmlReportAction.ACTION_NAME, typeof (XmlReportAction));
			
			return objectSource;
		}
	}
}