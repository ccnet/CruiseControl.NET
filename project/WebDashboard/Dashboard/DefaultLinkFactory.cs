using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class DefaultLinkFactory : ILinkFactory
	{
		private readonly IUrlBuilder urlBuilder;
		private readonly ICruiseUrlBuilder cruiseUrlBuilder;
		private readonly IBuildNameFormatter buildNameFormatter;

		public DefaultLinkFactory(IUrlBuilder urlBuilder, ICruiseUrlBuilder cruiseUrlBuilder, IBuildNameFormatter buildNameFormatter)
		{
			this.urlBuilder = urlBuilder;
			this.cruiseUrlBuilder = cruiseUrlBuilder;
			this.buildNameFormatter = buildNameFormatter;
		}

		public IAbsoluteLink CreateBuildLink(IBuildSpecifier buildSpecifier, string text, string action)
		{
			return new BuildLink(cruiseUrlBuilder, buildSpecifier, text, action);
		}

		public IAbsoluteLink CreateBuildLink(IBuildSpecifier buildSpecifier, string action)
		{
			return new BuildLink(cruiseUrlBuilder, buildSpecifier, buildNameFormatter.GetPrettyBuildName(buildSpecifier), action);
		}

		public IAbsoluteLink CreateProjectLink(IProjectSpecifier projectSpecifier, string text, string action)
		{
			return new ProjectLink(cruiseUrlBuilder, projectSpecifier, text, action);
		}

		public IAbsoluteLink CreateProjectLink(IProjectSpecifier projectSpecifier, string action)
		{
			return new ProjectLink(cruiseUrlBuilder, projectSpecifier, projectSpecifier.ProjectName, action);
		}

		public IAbsoluteLink CreateServerLink(IServerSpecifier serverSpecifier, string text, string action)
		{
			return new ServerLink(cruiseUrlBuilder, serverSpecifier, text, action);
		}

		public IAbsoluteLink CreateServerLink(IServerSpecifier serverSpecifier, string action)
		{
			return new ServerLink(cruiseUrlBuilder, serverSpecifier, serverSpecifier.ServerName, action);
		}

		public IAbsoluteLink CreateFarmLink(string text, string action)
		{
			return new FarmLink(urlBuilder, text, action);
		}

		public IAbsoluteLink CreateStyledBuildLink(IBuildSpecifier specifier, string action)
		{
			IAbsoluteLink link = CreateBuildLink(specifier, buildNameFormatter.GetPrettyBuildName(specifier), action);
			link.LinkClass = buildNameFormatter.GetCssClassForBuildLink(specifier);
			return link;
		}
	}
}
