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
			ArrayList displayableBuildLinkList = new ArrayList();
			
			foreach (IBuildSpecifier buildSpecifier in buildSpecifiers)
			{
				displayableBuildLinkList.Add(linkFactory.CreateStyledBuildLink(buildSpecifier, action));
			}

			return (IAbsoluteLink[]) displayableBuildLinkList.ToArray(typeof (IAbsoluteLink));
		}
	}
}
