using System;
using System.Collections.Specialized;
using System.Web;
using Objection;
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

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
    public class CruiseObjectSourceInitializer
    {
        private readonly ObjectionManager objectionManager;

        public CruiseObjectSourceInitializer(ObjectionManager objectionManager)
        {
            this.objectionManager = objectionManager;
        }

        // This all needs breaking up a bit (to make it testable, apart from anything else)
        public ObjectSource SetupObjectSourceForRequest(HttpContext context)
        {
            ObjectSource objectSource = (ObjectSource)objectionManager; // Yuch - put this in Object Wizard somewhere
            objectionManager.AddInstanceForType(typeof(ObjectSource), objectionManager);

            objectionManager.AddInstanceForType(typeof(HttpContext), context);
            HttpRequest request = context.Request;
            objectionManager.AddInstanceForType(typeof(HttpRequest), request);

            NameValueCollection parametersCollection = new NameValueCollection();
            parametersCollection.Add(request.QueryString);
            parametersCollection.Add(request.Form);
            objectionManager.AddInstanceForType(typeof(IRequest),
                                                new NameValueCollectionRequest(parametersCollection, request.Headers, request.Path,
                                                                               request.RawUrl, request.ApplicationPath));

            DefaultUrlBuilder urlBuilder = new DefaultUrlBuilder();
            objectionManager.AddInstanceForType(typeof(IUrlBuilder),
                                                new AbsolutePathUrlBuilderDecorator(
                                                    urlBuilder,
                                                    request.ApplicationPath));

            objectionManager.SetImplementationType(typeof(ICruiseRequest), typeof(RequestWrappingCruiseRequest));

            objectionManager.SetImplementationType(typeof(IMultiTransformer), typeof(PathMappingMultiTransformer));

            objectionManager.SetDependencyImplementationForType(typeof(PathMappingMultiTransformer), typeof(IMultiTransformer), typeof(HtmlAwareMultiTransformer));

            IDashboardConfiguration config = GetDashboardConfiguration(objectSource, context);
            objectionManager.AddInstanceForType(typeof(IDashboardConfiguration), config);

            IRemoteServicesConfiguration remoteServicesConfig = config.RemoteServices;
            objectionManager.AddInstanceForType(typeof(IRemoteServicesConfiguration), remoteServicesConfig);

            IPluginConfiguration pluginConfig = config.PluginConfiguration;
            objectionManager.AddInstanceForType(typeof(IPluginConfiguration), pluginConfig);

            ISessionRetriever sessionRetriever = pluginConfig.SessionStore.RetrieveRetriever();
            objectionManager.AddInstanceForType(typeof(ISessionRetriever), sessionRetriever);

            ISessionStorer sessionStorer = pluginConfig.SessionStore.RetrieveStorer();
            objectionManager.AddInstanceForType(typeof(ISessionStorer), sessionStorer);
            urlBuilder.SessionStorer = sessionStorer;


            System.Collections.Generic.List<string> LoadedPlugins = new System.Collections.Generic.List<string>();
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
                        objectionManager.AddInstanceForName(action.ActionName, action.Action)
                                .Decorate(typeof(CruiseActionProxyAction)).Decorate(typeof(ExceptionCatchingActionProxy)).Decorate(typeof(SiteTemplateActionDecorator)).Decorate(typeof(QuerySessionActionDecorator)).Decorate(typeof(NoCacheabilityActionProxy));
                    }
                        else
                        {
                        objectionManager.AddInstanceForName(action.ActionName, action.Action)
                                .Decorate(typeof(CruiseActionProxyAction)).Decorate(typeof(ExceptionCatchingActionProxy)).Decorate(typeof(QuerySessionActionDecorator)).Decorate(typeof(NoCacheabilityActionProxy));
                    }
                }
            }
            }


            if (UnknownPluginDetected) ThrowExceptionShouwingLoadedPlugins(LoadedPlugins, "FarmPlugins");
            LoadedPlugins = new System.Collections.Generic.List<string>();

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
                        objectionManager.AddInstanceForName(action.ActionName, action.Action)
                                .Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction)).Decorate(typeof(ExceptionCatchingActionProxy)).Decorate(typeof(SiteTemplateActionDecorator)).Decorate(typeof(QuerySessionActionDecorator)).Decorate(typeof(NoCacheabilityActionProxy));
                        }
                        else
                        {
                        objectionManager.AddInstanceForName(action.ActionName, action.Action)
                                .Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction)).Decorate(typeof(ExceptionCatchingActionProxy)).Decorate(typeof(QuerySessionActionDecorator)).Decorate(typeof(NoCacheabilityActionProxy));
                        }
                    }
                }
            }

            if (UnknownPluginDetected) ThrowExceptionShouwingLoadedPlugins(LoadedPlugins, "ServerPlugins");
            LoadedPlugins = new System.Collections.Generic.List<string>();


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
                        objectionManager.AddInstanceForName(action.ActionName, action.Action)
                                .Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction)).Decorate(typeof(ExceptionCatchingActionProxy)).Decorate(typeof(SiteTemplateActionDecorator)).Decorate(typeof(QuerySessionActionDecorator));
                        }
                        else
                        {
                            objectionManager.AddInstanceForName(action.ActionName, action.Action)
                                .Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction)).Decorate(typeof(ExceptionCatchingActionProxy)).Decorate(typeof(QuerySessionActionDecorator));
                        }
                    }
                }
            }

            if (UnknownPluginDetected) ThrowExceptionShouwingLoadedPlugins(LoadedPlugins, "ProjectPlugins");


            // Even if the user hasn't specified to use this plugin, we still need it registered since there are links to it elsewhere
            try
            {
                objectSource.GetByName(LatestBuildReportProjectPlugin.ACTION_NAME);
            }
            catch (ApplicationException)
            {
                IPlugin latestBuildPlugin = (IPlugin)objectSource.GetByType(typeof(LatestBuildReportProjectPlugin));
                objectionManager.AddInstanceForName(latestBuildPlugin.NamedActions[0].ActionName, latestBuildPlugin.NamedActions[0].Action)
                    .Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction)).Decorate(typeof(ExceptionCatchingActionProxy)).Decorate(typeof(SiteTemplateActionDecorator)).Decorate(typeof(QuerySessionActionDecorator));
            }


            LoadedPlugins = new System.Collections.Generic.List<string>();

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
                        objectionManager.AddInstanceForName(action.ActionName + "_CONDITIONAL_GET_FINGERPRINT_CHAIN", action.Action)
                                .Decorate(typeof(CruiseActionProxyAction)).Decorate(typeof(SiteTemplateActionDecorator))
                                .Decorate(typeof(QuerySessionActionDecorator));
                            objectionManager.AddInstanceForName(action.ActionName, action.Action)
                                .Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(BuildCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction))
                                .Decorate(typeof(CachingActionProxy)).Decorate(typeof(ExceptionCatchingActionProxy)).Decorate(typeof(SiteTemplateActionDecorator))
                                .Decorate(typeof(QuerySessionActionDecorator));
                        }
                        else
                        {
                        objectionManager.AddInstanceForName(action.ActionName + "_CONDITIONAL_GET_FINGERPRINT_CHAIN", action.Action)
                                .Decorate(typeof(CruiseActionProxyAction))
                                .Decorate(typeof(QuerySessionActionDecorator));
                        objectionManager.AddInstanceForName(action.ActionName, action.Action)
                            .Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(BuildCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction))
                                .Decorate(typeof(CachingActionProxy)).Decorate(typeof(ExceptionCatchingActionProxy))
                                .Decorate(typeof(QuerySessionActionDecorator));
                        }
                    }
                }
            }

            if (UnknownPluginDetected) ThrowExceptionShouwingLoadedPlugins(LoadedPlugins, "BuildPlugins");
            LoadedPlugins = new System.Collections.Generic.List<string>();

            // Security plugins (for handling authentication)
            foreach (ISecurityPlugin plugin in pluginConfig.SecurityPlugins)
            {
                plugin.SessionStorer = sessionStorer;
                foreach (INamedAction action in plugin.NamedActions)
                {
                    if ((action as INoSiteTemplateAction) == null)
                    {
                        objectionManager.AddInstanceForName(action.ActionName, action.Action)
                            .Decorate(typeof(ServerCheckingProxyAction))
                            .Decorate(typeof(CruiseActionProxyAction))
                            .Decorate(typeof(ExceptionCatchingActionProxy))
                            .Decorate(typeof(SiteTemplateActionDecorator))
                            .Decorate(typeof(QuerySessionActionDecorator))
                            .Decorate(typeof(NoCacheabilityActionProxy));
                    }
                    else
                    {
                        objectionManager.AddInstanceForName(action.ActionName, action.Action)
                            .Decorate(typeof(ServerCheckingProxyAction))
                            .Decorate(typeof(CruiseActionProxyAction))
                            .Decorate(typeof(ExceptionCatchingActionProxy))
                            .Decorate(typeof(QuerySessionActionDecorator))
                            .Decorate(typeof(NoCacheabilityActionProxy));
                    }
                }
            }

            if (UnknownPluginDetected) ThrowExceptionShouwingLoadedPlugins(LoadedPlugins, "SecurityPlugins");

            AddRequiredSecurityAction(LogoutSecurityAction.ActionName, typeof(LogoutSecurityAction));
            AddRequiredSecurityAction(ChangePasswordSecurityAction.ActionName, typeof(ChangePasswordSecurityAction));

            // ToDo - make this kind of thing specifiable by Plugins (note that this action is not wrapped with a SiteTemplateActionDecorator
            // See BuildLogBuildPlugin for linked todo
            objectionManager.AddTypeForName(XmlBuildLogAction.ACTION_NAME, typeof(XmlBuildLogAction))
                .Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(BuildCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction));

            // TODO - Xml Exceptions?
            objectionManager.AddTypeForName(ForceBuildXmlAction.ACTION_NAME, typeof(ForceBuildXmlAction))
                .Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction));

            // Supporting xml project status queries from CCTray or clients earlier than version 1.3
            // Also still used by the web dashboard for displaying farm/server reports
            objectionManager.AddTypeForName(XmlReportAction.ACTION_NAME, typeof(XmlReportAction));
            objectionManager.AddTypeForName(ProjectXmlReport.ActionName, typeof(ProjectXmlReport)).Decorate(typeof(CruiseActionProxyAction));

            // Supporting cruise server project and queue status queries from CCTray or clients 1.3 or later
            objectionManager.AddTypeForName(XmlServerReportAction.ACTION_NAME, typeof(XmlServerReportAction));


            // RSS publisher
            objectionManager.AddTypeForName(Plugins.RSS.RSSFeed.ACTION_NAME, typeof(Plugins.RSS.RSSFeed)).Decorate(typeof(CruiseActionProxyAction));

            // Status data
            objectionManager.AddTypeForName(ProjectStatusAction.ActionName, typeof(ProjectStatusAction))
                .Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction));

            // File download
            objectionManager.AddTypeForName(ProjectFileDownload.ActionName, typeof(ProjectFileDownload)).Decorate(typeof(CruiseActionProxyAction));

            return objectSource;
        }

        private void AddRequiredSecurityAction(string actionName, Type actionType)
        {
            objectionManager.AddTypeForName(actionName, actionType)
                .Decorate(typeof(ServerCheckingProxyAction))
                .Decorate(typeof(CruiseActionProxyAction))
                .Decorate(typeof(ExceptionCatchingActionProxy))
                .Decorate(typeof(SiteTemplateActionDecorator))
                .Decorate(typeof(QuerySessionActionDecorator))
                .Decorate(typeof(NoCacheabilityActionProxy));
        }

        private static IDashboardConfiguration GetDashboardConfiguration(ObjectSource objectSource, HttpContext context)
        {
            return new CachingDashboardConfigurationLoader(objectSource, context);
            //			return (IDashboardConfiguration) objectSource.GetByType(typeof(IDashboardConfiguration));
        }

        private void ThrowExceptionShouwingLoadedPlugins(System.Collections.Generic.List<string> loadedPlugins, string pluginTypeName)
        {
            System.Text.StringBuilder ErrorDescription = new System.Text.StringBuilder();

            ErrorDescription.AppendLine(string.Format("Error loading {0} ", pluginTypeName));
            ErrorDescription.AppendLine("Unknown pluginnames detected");
            ErrorDescription.AppendLine("Check your config");
            ErrorDescription.AppendLine("The following plugins were loaded successfully : ");

            foreach (string item in loadedPlugins)
            {
                ErrorDescription.AppendLine(string.Format(" * {0}", item));
            }

            throw new Exception(ErrorDescription.ToString());
        }

    }
}