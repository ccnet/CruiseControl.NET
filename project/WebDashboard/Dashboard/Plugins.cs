using System;
using System.Collections;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.WebDashboard.Config;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class Plugins : IPluginLinkCalculator
	{
		private readonly ObjectGiver objectGiver;
		private readonly IConfigurationGetter configurationGetter;
		private readonly ILinkFactory LinkFactory;

		public Plugins(ILinkFactory LinkFactory, IConfigurationGetter configurationGetter, ObjectGiver objectGiver)
		{
			this.LinkFactory = LinkFactory;
			this.configurationGetter = configurationGetter;
			this.objectGiver = objectGiver;
		}

		// TODO Some refactoring here.

		public IAbsoluteLink[] GetBuildPluginLinks(IBuildSpecifier buildSpecifier)
		{
			ArrayList links = new ArrayList();

			foreach (IPluginSpecification pluginSpecification in (IPluginSpecification[]) configurationGetter.GetConfigFromSection("CCNet/buildPlugins"))
			{
				IPluginLinkRenderer linkRenderer = objectGiver.GiveObjectByType(pluginSpecification.Type) as IPluginLinkRenderer;
				if (linkRenderer == null)
				{
					throw new CruiseControlException(pluginSpecification.TypeName + " is not a IPluginLinkRenderer");
				}
				links.Add(LinkFactory.CreateBuildLink(buildSpecifier, linkRenderer.LinkDescription, new ActionSpecifierWithName(linkRenderer.LinkActionName)));
			}

			return (IAbsoluteLink[]) links.ToArray(typeof (IAbsoluteLink));
		}

		public IAbsoluteLink[] GetServerPluginLinks(IServerSpecifier serverSpecifier)
		{
			ArrayList links = new ArrayList();

			foreach (IPluginSpecification pluginSpecification in (IPluginSpecification[]) configurationGetter.GetConfigFromSection("CCNet/serverPlugins"))
			{
				IPluginLinkRenderer linkRenderer = objectGiver.GiveObjectByType(pluginSpecification.Type) as IPluginLinkRenderer;
				if (linkRenderer == null)
				{
					throw new CruiseControlException(pluginSpecification.TypeName + " is not a IPluginLinkRenderer");
				}
				links.Add(LinkFactory.CreateServerLink(serverSpecifier, linkRenderer.LinkDescription, new ActionSpecifierWithName(linkRenderer.LinkActionName)));
			}

			return (IAbsoluteLink[]) links.ToArray(typeof (IAbsoluteLink));
		}

		public IAbsoluteLink[] GetProjectPluginLinks(IProjectSpecifier projectSpecifier)
		{
			ArrayList links = new ArrayList();

			foreach (IPluginSpecification pluginSpecification in (IPluginSpecification[]) configurationGetter.GetConfigFromSection("CCNet/projectPlugins"))
			{
				IPluginLinkRenderer linkRenderer = objectGiver.GiveObjectByType(pluginSpecification.Type) as IPluginLinkRenderer;
				if (linkRenderer == null)
				{
					throw new CruiseControlException(pluginSpecification.TypeName + " is not a IPluginLinkRenderer");
				}
				links.Add(LinkFactory.CreateProjectLink(projectSpecifier, linkRenderer.LinkDescription, new ActionSpecifierWithName(linkRenderer.LinkActionName)));
			}

			return (IAbsoluteLink[]) links.ToArray(typeof (IAbsoluteLink));
		}
	}
}
