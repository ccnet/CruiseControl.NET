using System.Collections;
using ThoughtWorks.CruiseControl.WebDashboard.Configuration;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class DefaultPluginLinkCalculator : IPluginLinkCalculator
	{
		private readonly ILinkFactory LinkFactory;
		private readonly IPluginConfiguration pluginConfiguration;

		public DefaultPluginLinkCalculator(ILinkFactory LinkFactory, IPluginConfiguration pluginConfiguration)
		{
			this.LinkFactory = LinkFactory;
			this.pluginConfiguration = pluginConfiguration;
		}

		public IAbsoluteLink[] GetBuildPluginLinks(IBuildSpecifier buildSpecifier)
		{
			ArrayList links = new ArrayList();
			foreach (IPlugin plugin in pluginConfiguration.BuildPlugins)
			{
				links.Add(LinkFactory.CreateBuildLink(buildSpecifier, plugin.LinkDescription, new ActionSpecifierWithName(plugin.NamedActions[0].ActionName)));
			}
			return (IAbsoluteLink[]) links.ToArray(typeof (IAbsoluteLink));
		}

		public IAbsoluteLink[] GetServerPluginLinks(IServerSpecifier serverSpecifier)
		{
			ArrayList links = new ArrayList();
			foreach (IPlugin plugin in pluginConfiguration.ServerPlugins)
			{
				links.Add(LinkFactory.CreateServerLink(serverSpecifier, plugin.LinkDescription, new ActionSpecifierWithName(plugin.NamedActions[0].ActionName)));
			}
			return (IAbsoluteLink[]) links.ToArray(typeof (IAbsoluteLink));
		}

		public IAbsoluteLink[] GetProjectPluginLinks(IProjectSpecifier projectSpecifier)
		{
			ArrayList links = new ArrayList();
			foreach (IPlugin plugin in pluginConfiguration.ProjectPlugins)
			{
				links.Add(LinkFactory.CreateProjectLink(projectSpecifier, plugin.LinkDescription, new ActionSpecifierWithName(plugin.NamedActions[0].ActionName)));
			}
			return (IAbsoluteLink[]) links.ToArray(typeof (IAbsoluteLink));
		}

		public IAbsoluteLink[] GetFarmPluginLinks()
		{
			ArrayList links = new ArrayList();
			foreach (IPlugin plugin in pluginConfiguration.FarmPlugins)
			{
				links.Add(LinkFactory.CreateFarmLink(plugin.LinkDescription, new ActionSpecifierWithName(plugin.NamedActions[0].ActionName)));
			}
			return (IAbsoluteLink[]) links.ToArray(typeof (IAbsoluteLink));
		}
	}
}
