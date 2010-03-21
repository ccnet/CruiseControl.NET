namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
    using System.Collections.Generic;
    using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

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
			var lstLinks = new List<IAbsoluteLink>();
			foreach (IServerSpecifier serverSpecifier in serverSpecifiers)
			{
				lstLinks.Add(linkFactory.CreateServerLink(serverSpecifier, action));
			}

			return lstLinks.ToArray();
		}

		public IAbsoluteLink[] CreateStyledBuildLinkList(IBuildSpecifier[] buildSpecifiers, IBuildSpecifier selectedBuildSpecifier, string action)
		{
			var displayableBuildLinkList = new List<IAbsoluteLink>();
			
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

			return displayableBuildLinkList.ToArray();			
		}
	}
}
