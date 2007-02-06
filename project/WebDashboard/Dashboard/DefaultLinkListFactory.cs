using System.Collections;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class DefaultLinkListFactory : ILinkListFactory
	{
		private readonly ILinkFactory linkFactory;

		public DefaultLinkListFactory(ILinkFactory linkFactory)
		{
			this.linkFactory = linkFactory;
		}

		public IAbsoluteLink[] CreateStyledBuildLinkList(IBuildSpecifier[] buildSpecifiers, string action)
		{
			return CreateStyledBuildLinkList(buildSpecifiers, null, action);
		}

		public IAbsoluteLink[] CreateServerLinkList(IServerSpecifier[] serverSpecifiers, string action)
		{
			ArrayList lstLinks = new ArrayList();
			foreach (IServerSpecifier serverSpecifier in serverSpecifiers)
			{
				lstLinks.Add(linkFactory.CreateServerLink(serverSpecifier, action));
			}

			return (IAbsoluteLink[])lstLinks.ToArray(typeof(IAbsoluteLink));
		}

		public IAbsoluteLink[] CreateStyledBuildLinkList(IBuildSpecifier[] buildSpecifiers, IBuildSpecifier selectedBuildSpecifier, string action)
		{
			ArrayList displayableBuildLinkList = new ArrayList();
			
			foreach (IBuildSpecifier buildSpecifier in buildSpecifiers)
			{
				if (buildSpecifier.Equals(selectedBuildSpecifier))
				{
					displayableBuildLinkList.Add(linkFactory.CreateStyledSelectedBuildLink(buildSpecifier, action));
				}
				else
				{
					displayableBuildLinkList.Add(linkFactory.CreateStyledBuildLink(buildSpecifier, action));
				}
			}

			return (IAbsoluteLink[]) displayableBuildLinkList.ToArray(typeof (IAbsoluteLink));			
		}
	}
}
