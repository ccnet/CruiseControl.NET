using System;
using System.Collections.Specialized;
using System.Web;
using Objection;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.WebDashboard.Configuration;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.ActionDecorators;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.FarmReport;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.Security;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ServerReport;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class CruiseObjectSourceInitializer
	{
		private readonly ObjectionManager _objectionManager;

		public CruiseObjectSourceInitializer(ObjectionManager objectionManager)
		{
			_objectionManager = objectionManager;
		}

		public void SetupObjectSourceForFirstRequest(HttpContext context)
		{
			_objectionManager.AddInstanceForType(typeof(ObjectSource), _objectionManager);

			ObjectSource objectSource = UpdateObjectSourceForRequest(context);

			DefaultUrlBuilder urlBuilder = new DefaultUrlBuilder();
			_objectionManager.AddInstanceForType(typeof(IUrlBuilder),
												new AbsolutePathUrlBuilderDecorator(
													urlBuilder,
													context.Request.ApplicationPath));

			_objectionManager.SetImplementationType(typeof(ICruiseRequest), typeof(RequestWrappingCruiseRequest));

			_objectionManager.SetImplementationType(typeof(IMultiTransformer), typeof(PathMappingMultiTransformer));

			_objectionManager.SetDependencyImplementationForType(typeof(PathMappingMultiTransformer), typeof(IMultiTransformer), typeof(HtmlAwareMultiTransformer));

			IDashboardConfiguration config = GetDashboardConfiguration(objectSource, context);
			_objectionManager.AddInstanceForType(typeof(IDashboardConfiguration), config);

			IRemoteServicesConfiguration remoteServicesConfig = config.RemoteServices;
			_objectionManager.AddInstanceForType(typeof(IRemoteServicesConfiguration), remoteServicesConfig);

			IPluginConfiguration pluginConfig = config.PluginConfiguration;
			_objectionManager.AddInstanceForType(typeof(IPluginConfiguration), pluginConfig);

			ISessionRetriever sessionRetriever = pluginConfig.SessionStore.RetrieveRetriever();
			_objectionManager.AddInstanceForType(typeof(ISessionRetriever), sessionRetriever);

			ISessionStorer sessionStorer = pluginConfig.SessionStore.RetrieveStorer();
			_objectionManager.AddInstanceForType(typeof(ISessionStorer), sessionStorer);

			LoadFarmPlugins(pluginConfig);
			LoadServerPlugins(pluginConfig);
			LoadProjectPlugins(pluginConfig);

			// Even if the user hasn't specified to use this plugin, we still need it registered since there are links to it elsewhere
			try
			{
				objectSource.GetByName(LatestBuildReportProjectPlugin.ACTION_NAME.ToLowerInvariant());
			}
			catch (ApplicationException)
			{
				IPlugin latestBuildPlugin = (IPlugin)objectSource.GetByType(typeof(LatestBuildReportProjectPlugin));
				AddActionInstance(latestBuildPlugin.NamedActions[0])
					.Decorate(typeof(ServerCheckingProxyAction))
					.Decorate(typeof(ProjectCheckingProxyAction))
					.Decorate(typeof(CruiseActionProxyAction))
					.Decorate(typeof(ExceptionCatchingActionProxy))
					.Decorate(typeof(SiteTemplateActionDecorator));
			}

			LoadBuildPlugins(pluginConfig);
			LoadSecurityPlugins(pluginConfig, sessionStorer);

			AddRequiredSecurityAction(LogoutSecurityAction.ActionName.ToLowerInvariant(), typeof(LogoutSecurityAction));
			AddRequiredSecurityAction(ChangePasswordSecurityAction.ActionName.ToLowerInvariant(), typeof(ChangePasswordSecurityAction));

			// ToDo - make this kind of thing specifiable by Plugins (note that this action is not wrapped with a SiteTemplateActionDecorator
			// See BuildLogBuildPlugin for linked todo
			_objectionManager.AddTypeForName(XmlBuildLogAction.ACTION_NAME.ToLowerInvariant(), typeof(XmlBuildLogAction))
				.Decorate(typeof(ServerCheckingProxyAction))
				.Decorate(typeof(BuildCheckingProxyAction))
				.Decorate(typeof(ProjectCheckingProxyAction))
				.Decorate(typeof(CruiseActionProxyAction));

			// TODO - Xml Exceptions?
			_objectionManager.AddTypeForName(ForceBuildXmlAction.ACTION_NAME.ToLowerInvariant(), typeof(ForceBuildXmlAction))
				.Decorate(typeof(ServerCheckingProxyAction))
				.Decorate(typeof(ProjectCheckingProxyAction))
				.Decorate(typeof(CruiseActionProxyAction));

			// Supporting xml project status queries from CCTray or clients earlier than version 1.3
			// Also still used by the web dashboard for displaying farm/server reports
			_objectionManager.AddTypeForName(XmlReportAction.ACTION_NAME.ToLowerInvariant(), typeof(XmlReportAction));
			_objectionManager.AddTypeForName(ProjectXmlReport.ActionName.ToLowerInvariant(), typeof(ProjectXmlReport))
				.Decorate(typeof(CruiseActionProxyAction));

			// Supporting cruise server project and queue status queries from CCTray or clients 1.3 or later
			_objectionManager.AddTypeForName(XmlServerReportAction.ACTION_NAME.ToLowerInvariant(), typeof(XmlServerReportAction));

			// Security handler for CCTray or client 1.5 or later
			_objectionManager.AddTypeForName(XmlServerSecurityAction.ACTION_NAME.ToLowerInvariant(), typeof(XmlServerSecurityAction));

			// RSS publisher
			_objectionManager.AddTypeForName(Plugins.RSS.RSSFeed.ACTION_NAME.ToLowerInvariant(), typeof(Plugins.RSS.RSSFeed))
				.Decorate(typeof(CruiseActionProxyAction));

			// Status data
			_objectionManager.AddTypeForName(ProjectStatusAction.ActionName.ToLowerInvariant(), typeof(ProjectStatusAction))
				.Decorate(typeof(ServerCheckingProxyAction))
				.Decorate(typeof(ProjectCheckingProxyAction))
				.Decorate(typeof(CruiseActionProxyAction));

			// File downloads
			_objectionManager.AddTypeForName(ProjectFileDownload.ActionName.ToLowerInvariant(), typeof(ProjectFileDownload))
				.Decorate(typeof(CruiseActionProxyAction));
			_objectionManager.AddTypeForName(BuildFileDownload.ActionName.ToLowerInvariant(), typeof(BuildFileDownload))
				.Decorate(typeof(CruiseActionProxyAction));

			// Parameters handler for CCTray or client 1.5 or later
			_objectionManager.AddInstanceForName(XmlProjectParametersReportAction.ACTION_NAME.ToLowerInvariant(),
				objectSource.GetByType(typeof(XmlProjectParametersReportAction)));

			// Raw XML request handler
			_objectionManager.AddTypeForName(MessageHandlerPlugin.ActionName.ToLowerInvariant(), typeof(MessageHandlerPlugin))
				.Decorate(typeof(CruiseActionProxyAction));
		}

		public ObjectSource UpdateObjectSourceForRequest(HttpContext context)
		{
			_objectionManager.AddInstanceForType(typeof(HttpContext), context);
			HttpRequest request = context.Request;
			_objectionManager.AddInstanceForType(typeof(HttpRequest), request);

			NameValueCollection parametersCollection = new NameValueCollection();
			parametersCollection.Add(request.QueryString);
			parametersCollection.Add(request.Form);
			_objectionManager.AddInstanceForType(typeof(IRequest),
				new NameValueCollectionRequest(
					parametersCollection, request.Headers, request.Path,
					request.RawUrl, request.ApplicationPath));
			return (ObjectSource)_objectionManager; // Yuch - put this in Object Wizard somewhere
		}

		private void LoadFarmPlugins(IPluginConfiguration pluginConfig)
		{
			var LoadedPlugins = new System.Collections.Generic.List<string>();
			bool UnknownPluginDetected = false;

			foreach (IPlugin plugin in pluginConfig.FarmPlugins)
			{
				if (plugin == null)
				{
					UnknownPluginDetected = true;
				}
				else
				{
					LoadedPlugins.Add(plugin.LinkDescription);

					foreach (INamedAction action in plugin.NamedActions)
					{
						if ((action as INoSiteTemplateAction) == null)
						{
							AddActionInstance(action)
								.Decorate(typeof(CruiseActionProxyAction))
								.Decorate(typeof(ExceptionCatchingActionProxy))
								.Decorate(typeof(SiteTemplateActionDecorator))
								.Decorate(typeof(NoCacheabilityActionProxy));
						}
						else
						{
							AddActionInstance(action)
								.Decorate(typeof(CruiseActionProxyAction))
								.Decorate(typeof(ExceptionCatchingActionProxy))
								.Decorate(typeof(NoCacheabilityActionProxy));
						}
					}
				}
			}

			if (UnknownPluginDetected) ThrowExceptionShowingLoadedPlugins(LoadedPlugins, "FarmPlugins");
		}

		private void LoadServerPlugins(IPluginConfiguration pluginConfig)
		{
			var LoadedPlugins = new System.Collections.Generic.List<string>();
			bool UnknownPluginDetected = false;

			foreach (IPlugin plugin in pluginConfig.ServerPlugins)
			{
				if (plugin == null)
				{
					UnknownPluginDetected = true;
				}
				else
				{
					LoadedPlugins.Add(plugin.LinkDescription);
					foreach (INamedAction action in plugin.NamedActions)
					{
						if ((action as INoSiteTemplateAction) == null)
						{
							AddActionInstance(action)
								.Decorate(typeof(ServerCheckingProxyAction))
								.Decorate(typeof(CruiseActionProxyAction))
								.Decorate(typeof(ExceptionCatchingActionProxy))
								.Decorate(typeof(SiteTemplateActionDecorator))
								.Decorate(typeof(NoCacheabilityActionProxy));
						}
						else
						{
							AddActionInstance(action)
								.Decorate(typeof(ServerCheckingProxyAction))
								.Decorate(typeof(CruiseActionProxyAction))
								.Decorate(typeof(ExceptionCatchingActionProxy))
								.Decorate(typeof(NoCacheabilityActionProxy));
						}
					}
				}
			}

			if (UnknownPluginDetected) ThrowExceptionShowingLoadedPlugins(LoadedPlugins, "ServerPlugins");
		}

		private void LoadProjectPlugins(IPluginConfiguration pluginConfig)
		{
			var LoadedPlugins = new System.Collections.Generic.List<string>();
			bool UnknownPluginDetected = false;

			foreach (IPlugin plugin in pluginConfig.ProjectPlugins)
			{
				if (plugin == null)
				{
					UnknownPluginDetected = true;
				}
				else
				{
					LoadedPlugins.Add(plugin.LinkDescription);
					foreach (INamedAction action in plugin.NamedActions)
					{
						if ((action as INoSiteTemplateAction) == null)
						{
							AddActionInstance(action)
								.Decorate(typeof(ServerCheckingProxyAction))
								.Decorate(typeof(ProjectCheckingProxyAction))
								.Decorate(typeof(CruiseActionProxyAction))
								.Decorate(typeof(ExceptionCatchingActionProxy))
								.Decorate(typeof(SiteTemplateActionDecorator));
						}
						else
						{
							AddActionInstance(action)
								.Decorate(typeof(ServerCheckingProxyAction))
								.Decorate(typeof(ProjectCheckingProxyAction))
								.Decorate(typeof(CruiseActionProxyAction))
								.Decorate(typeof(ExceptionCatchingActionProxy));
						}
					}
				}
			}

			if (UnknownPluginDetected) ThrowExceptionShowingLoadedPlugins(LoadedPlugins, "ProjectPlugins");
		}

		private void LoadBuildPlugins(IPluginConfiguration pluginConfig)
		{
			var LoadedPlugins = new System.Collections.Generic.List<string>();
			bool UnknownPluginDetected = false;

			foreach (IBuildPlugin plugin in pluginConfig.BuildPlugins)
			{
				if (plugin == null)
				{
					UnknownPluginDetected = true;
				}
				else
				{
					LoadedPlugins.Add(plugin.LinkDescription);
					foreach (INamedAction action in plugin.NamedActions)
					{
						if ((action as INoSiteTemplateAction) == null)
						{
							_objectionManager.AddInstanceForName(action.ActionName.ToLowerInvariant() + "_CONDITIONAL_GET_FINGERPRINT_CHAIN", action.Action)
								.Decorate(typeof(CruiseActionProxyAction))
								.Decorate(typeof(SiteTemplateActionDecorator));
							AddActionInstance(action)
								.Decorate(typeof(ServerCheckingProxyAction))
								.Decorate(typeof(BuildCheckingProxyAction))
								.Decorate(typeof(ProjectCheckingProxyAction))
								.Decorate(typeof(CruiseActionProxyAction))
								.Decorate(typeof(CachingActionProxy))
								.Decorate(typeof(ExceptionCatchingActionProxy))
								.Decorate(typeof(SiteTemplateActionDecorator));
						}
						else
						{
							_objectionManager.AddInstanceForName(action.ActionName.ToLowerInvariant() + "_CONDITIONAL_GET_FINGERPRINT_CHAIN", action.Action)
								.Decorate(typeof(CruiseActionProxyAction));
							AddActionInstance(action)
								.Decorate(typeof(ServerCheckingProxyAction))
								.Decorate(typeof(BuildCheckingProxyAction))
								.Decorate(typeof(ProjectCheckingProxyAction))
								.Decorate(typeof(CruiseActionProxyAction))
								.Decorate(typeof(CachingActionProxy))
								.Decorate(typeof(ExceptionCatchingActionProxy));
						}
					}
				}
			}

			if (UnknownPluginDetected) ThrowExceptionShowingLoadedPlugins(LoadedPlugins, "BuildPlugins");
		}

		private void LoadSecurityPlugins(IPluginConfiguration pluginConfig, ISessionStorer sessionStorer)
		{
			var LoadedPlugins = new System.Collections.Generic.List<string>();
			bool UnknownPluginDetected = false;

			// Security plugins (for handling authentication)
			foreach (ISecurityPlugin plugin in pluginConfig.SecurityPlugins)
			{
				plugin.SessionStorer = sessionStorer;
				foreach (INamedAction action in plugin.NamedActions)
				{
					if ((action as INoSiteTemplateAction) == null)
					{
						AddActionInstance(action)
							.Decorate(typeof(ServerCheckingProxyAction))
							.Decorate(typeof(CruiseActionProxyAction))
							.Decorate(typeof(ExceptionCatchingActionProxy))
							.Decorate(typeof(SiteTemplateActionDecorator))
							.Decorate(typeof(NoCacheabilityActionProxy));
					}
					else
					{
						AddActionInstance(action)
							.Decorate(typeof(ServerCheckingProxyAction))
							.Decorate(typeof(CruiseActionProxyAction))
							.Decorate(typeof(ExceptionCatchingActionProxy))
							.Decorate(typeof(NoCacheabilityActionProxy));
					}
				}
			}

			if (UnknownPluginDetected) ThrowExceptionShowingLoadedPlugins(LoadedPlugins, "SecurityPlugins");
		}

		private void AddRequiredSecurityAction(string actionName, Type actionType)
		{
			_objectionManager.AddTypeForName(actionName, actionType)
				.Decorate(typeof(ServerCheckingProxyAction))
				.Decorate(typeof(CruiseActionProxyAction))
				.Decorate(typeof(ExceptionCatchingActionProxy))
				.Decorate(typeof(SiteTemplateActionDecorator))
				.Decorate(typeof(NoCacheabilityActionProxy));
		}

		private static IDashboardConfiguration GetDashboardConfiguration(ObjectSource objectSource, HttpContext context)
		{
			return new CachingDashboardConfigurationLoader(objectSource, context);
			//			return (IDashboardConfiguration) objectSource.GetByType(typeof(IDashboardConfiguration));
		}

		private void ThrowExceptionShowingLoadedPlugins(System.Collections.Generic.List<string> loadedPlugins, string pluginTypeName)
		{
			System.Text.StringBuilder ErrorDescription = new System.Text.StringBuilder();

			ErrorDescription.AppendLine(string.Format(System.Globalization.CultureInfo.CurrentCulture, "Error loading {0} ", pluginTypeName));
			ErrorDescription.AppendLine("Unknown pluginnames detected");
			ErrorDescription.AppendLine("Check your config");
			ErrorDescription.AppendLine("The following plugins were loaded successfully : ");

			foreach (string item in loadedPlugins)
			{
				ErrorDescription.AppendLine(string.Format(System.Globalization.CultureInfo.CurrentCulture, " * {0}", item));
			}
			throw new CruiseControlException(ErrorDescription.ToString());
		}

		private DecoratableByType AddActionInstance(INamedAction action)
		{
			return _objectionManager.AddInstanceForName(action.ActionName.ToLowerInvariant(), action.Action);
		}

	}
}