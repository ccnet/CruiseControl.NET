using System.Web;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.WebDashboard.Config;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.ActionDecorators;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class CruiseObjectGiverInitializer
	{
		private ObjectGiverAndRegistrar giverAndRegistrar;

		public CruiseObjectGiverInitializer(ObjectGiverAndRegistrar giverAndRegistrar)
		{
			this.giverAndRegistrar = giverAndRegistrar;
		}

		public ObjectGiver InitializeGiverForRequest(HttpContext context)
		{
			giverAndRegistrar.AddTypedObject(typeof(ObjectGiver), giverAndRegistrar);
			giverAndRegistrar.AddTypedObject(typeof(HttpContext), context);
			HttpRequest request = context.Request;
			giverAndRegistrar.AddTypedObject(typeof(HttpRequest), request);

			// Add functionality to object giver to handle this?
			giverAndRegistrar.AddTypedObject(typeof(IRequest), new AggregatedRequest(new NameValueCollectionRequest(request.Form), new NameValueCollectionRequest(request.QueryString)));
			
			giverAndRegistrar.SetImplementationType(typeof(IPathMapper), typeof(HttpPathMapper));
			giverAndRegistrar.SetImplementationType(typeof(IMultiTransformer), typeof(PathMappingMultiTransformer));

			giverAndRegistrar.SetDependencyImplementationForType(typeof(PathMappingMultiTransformer), typeof(IMultiTransformer), typeof (HtmlAwareMultiTransformer));

			// Need to get these into plugin setup
			// These plugins are currently disabled - this code will be required again when they are needed
//			giverAndRegistrar.SetDependencyImplementationForIdentifer(SaveNewProjectAction.ACTION_NAME, typeof(IPathMapper), typeof(PathMapperUsingHostName));
//			giverAndRegistrar.SetDependencyImplementationForIdentifer(SaveEditProjectAction.ACTION_NAME, typeof(IPathMapper), typeof(PathMapperUsingHostName));

			IConfigurationGetter configurationGetter = (IConfigurationGetter) giverAndRegistrar.GiveObjectByType(typeof(IConfigurationGetter));
			if (configurationGetter == null)
			{
				throw new CruiseControlException("Unable to instantiate configuration getter");
			}

			// ToDo - Refactor these plugin sections

			foreach (IPluginSpecification pluginSpecification in (IPluginSpecification[]) configurationGetter.GetConfigFromSection("CCNet/farmPlugins"))
			{
				IPlugin plugin = giverAndRegistrar.GiveObjectByType(pluginSpecification.Type) as IPlugin;
				if (plugin == null)
				{
					throw new CruiseControlException(pluginSpecification.TypeName + " is not a IPlugin");
				}
				foreach (TypedAction action in plugin.Actions)
				{
					giverAndRegistrar.CreateImplementationMapping(action.ActionName, action.ActionType)
						.Decorate(typeof(SiteTemplateActionDecorator)).Decorate(typeof(CruiseActionProxyAction)).Decorate(typeof(ExceptionCatchingActionProxy));
				}
			}

			foreach (IPluginSpecification pluginSpecification in (IPluginSpecification[]) configurationGetter.GetConfigFromSection("CCNet/serverPlugins"))
			{
				IPlugin plugin = giverAndRegistrar.GiveObjectByType(pluginSpecification.Type) as IPlugin;
				if (plugin == null)
				{
					throw new CruiseControlException(pluginSpecification.TypeName + " is not a IPlugin");
				}
				foreach (TypedAction action in plugin.Actions)
				{
					giverAndRegistrar.CreateImplementationMapping(action.ActionName, action.ActionType)
						.Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(SiteTemplateActionDecorator)).Decorate(typeof(CruiseActionProxyAction)).Decorate(typeof(ExceptionCatchingActionProxy));
				}
			}

			foreach (IPluginSpecification pluginSpecification in (IPluginSpecification[]) configurationGetter.GetConfigFromSection("CCNet/projectPlugins"))
			{
				IPlugin plugin = giverAndRegistrar.GiveObjectByType(pluginSpecification.Type) as IPlugin;
				if (plugin == null)
				{
					throw new CruiseControlException(pluginSpecification.TypeName + " is not a IPlugin");
				}
				foreach (TypedAction action in plugin.Actions)
				{
					giverAndRegistrar.CreateImplementationMapping(action.ActionName, action.ActionType)
						.Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(SiteTemplateActionDecorator)).Decorate(typeof(CruiseActionProxyAction)).Decorate(typeof(ExceptionCatchingActionProxy));
				}
			}

			foreach (IPluginSpecification pluginSpecification in (IPluginSpecification[]) configurationGetter.GetConfigFromSection("CCNet/buildPlugins"))
			{
				IPlugin plugin = giverAndRegistrar.GiveObjectByType(pluginSpecification.Type) as IPlugin;
				if (plugin == null)
				{
					throw new CruiseControlException(pluginSpecification.TypeName + " is not a IPlugin");
				}
				foreach (TypedAction action in plugin.Actions)
				{
					giverAndRegistrar.CreateImplementationMapping(action.ActionName,action.ActionType)
						.Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(BuildCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(SiteTemplateActionDecorator)).Decorate(typeof(CruiseActionProxyAction)).Decorate(typeof(ExceptionCatchingActionProxy));
				}
			}

			// ToDo - make this kind of thing specifiable by Plugins (note that this action is not wrapped with a SiteTemplateActionDecorator
			// See BuildLogBuildPlugin for linked todo
			giverAndRegistrar.CreateImplementationMapping(XmlBuildLogAction.ACTION_NAME, typeof(XmlBuildLogAction))
				.Decorate(typeof(ServerCheckingProxyAction)).Decorate(typeof(BuildCheckingProxyAction)).Decorate(typeof(ProjectCheckingProxyAction)).Decorate(typeof(CruiseActionProxyAction));

			return giverAndRegistrar;
		}
	}
}