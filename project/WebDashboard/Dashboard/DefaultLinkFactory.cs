using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class DefaultLinkFactory : ILinkFactory
	{
		private readonly IUrlBuilder urlBuilder;

		public DefaultLinkFactory(IUrlBuilder urlBuilder)
		{
			this.urlBuilder = urlBuilder;
		}

		public IAbsoluteLink CreateBuildLink(IBuildSpecifier buildSpecifier, string description, IActionSpecifier actionSpecifier)
		{
			return new BuildLink(urlBuilder, buildSpecifier, description, actionSpecifier);
		}

		public IAbsoluteLink CreateProjectLink(IProjectSpecifier buildSpecifier, string description, IActionSpecifier actionSpecifier)
		{
			return new ProjectLink(urlBuilder, buildSpecifier, description, actionSpecifier);
		}

		public IAbsoluteLink CreateServerLink(IServerSpecifier serverSpecifier, string description, IActionSpecifier actionSpecifier)
		{
			return new ServerLink(urlBuilder, serverSpecifier, description, actionSpecifier);
		}
	}
}
