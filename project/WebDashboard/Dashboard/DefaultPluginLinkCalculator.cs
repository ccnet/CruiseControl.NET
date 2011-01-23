using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
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
			var links = new List<IAbsoluteLink>();
			foreach (IBuildPlugin plugin in pluginConfiguration.BuildPlugins)
			{
				if (plugin.IsDisplayedForProject(buildSpecifier.ProjectSpecifier))
				{
					links.Add(LinkFactory.CreateBuildLink(buildSpecifier, plugin.LinkDescription, plugin.NamedActions[0].ActionName));
				}
			}
            return links.ToArray();
		}

		public IAbsoluteLink[] GetServerPluginLinks(IServerSpecifier serverSpecifier)
		{
            var links = new List<IAbsoluteLink>();
			foreach (IPlugin plugin in pluginConfiguration.ServerPlugins)
			{
				links.Add(LinkFactory.CreateServerLink(serverSpecifier, plugin.LinkDescription, plugin.NamedActions[0].ActionName));
			}
			return links.ToArray();
		}

		public IAbsoluteLink[] GetProjectPluginLinks(IProjectSpecifier projectSpecifier)
		{
            var links = new List<IAbsoluteLink>();
			foreach (IPlugin plugin in pluginConfiguration.ProjectPlugins)
			{
				links.Add(LinkFactory.CreateProjectLink(projectSpecifier, plugin.LinkDescription, plugin.NamedActions[0].ActionName));
			}
            return links.ToArray();
		}

		public IAbsoluteLink[] GetFarmPluginLinks()
		{
            var links = new List<IAbsoluteLink>();
			foreach (IPlugin plugin in pluginConfiguration.FarmPlugins)
			{
				links.Add(LinkFactory.CreateFarmLink(plugin.LinkDescription, plugin.NamedActions[0].ActionName));
			}
            return links.ToArray();
		}
	}
}
