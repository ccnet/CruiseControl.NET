using System.Collections;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.WebDashboard.Config;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class BuildPlugins : IBuildPluginLinkCalculator
	{
		private readonly ObjectGiver objectGiver;
		private readonly IConfigurationGetter configurationGetter;
		private readonly IBuildLinkFactory buildLinkFactory;

		public BuildPlugins(IBuildLinkFactory buildLinkFactory, IConfigurationGetter configurationGetter, ObjectGiver objectGiver)
		{
			this.buildLinkFactory = buildLinkFactory;
			this.configurationGetter = configurationGetter;
			this.objectGiver = objectGiver;
		}

		public IAbsoluteLink[] GetBuildPluginLinks(string serverName, string projectName, string buildName)
		{
			ArrayList links = new ArrayList();

			foreach (IPluginSpecification pluginSpecification in (IPluginSpecification[]) configurationGetter.GetConfigFromSection("CCNet/buildPlugins"))
			{
				IPluginLinkRenderer linkRenderer = objectGiver.GiveObjectByType(pluginSpecification.Type) as IPluginLinkRenderer;
				if (linkRenderer == null)
				{
					throw new CruiseControlException(pluginSpecification.TypeName + " is not a IPluginLinkRenderer");
				}
				links.Add(buildLinkFactory.CreateBuildLink(serverName, projectName, buildName, linkRenderer.Description, new ActionSpecifierWithName(linkRenderer.ActionName)));
			}

			return (IAbsoluteLink[]) links.ToArray(typeof (IAbsoluteLink));
		}
	}
}
