using System.Collections;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class DefaultLinkListFactory : ILinkListFactory
	{
		private readonly ILinkFactory linkFactory;

		public DefaultLinkListFactory(ILinkFactory linkFactory)
		{
			this.linkFactory = linkFactory;
		}

		public IAbsoluteLink[] CreateStyledBuildLinkList(IBuildSpecifier[] buildSpecifiers, IActionSpecifier actionSpecifier)
		{
			ArrayList displayableBuildLinkList = new ArrayList();
			
			foreach (IBuildSpecifier buildSpecifier in buildSpecifiers)
			{
				displayableBuildLinkList.Add(linkFactory.CreateStyledBuildLink(buildSpecifier, actionSpecifier));
			}

			return (IAbsoluteLink[]) displayableBuildLinkList.ToArray(typeof (IAbsoluteLink));
		}
	}
}
