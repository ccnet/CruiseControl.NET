namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class DefaultBuildLinkFactory : IBuildLinkFactory
	{
		private readonly IUrlBuilder urlBuilder;

		public DefaultBuildLinkFactory(IUrlBuilder urlBuilder)
		{
			this.urlBuilder = urlBuilder;
		}

		public IAbsoluteLink CreateBuildLink(string serverName, string projectName, string buildName, string description, IActionSpecifier actionSpecifier)
		{
			return new BuildLink(urlBuilder, serverName, projectName, buildName, description, actionSpecifier);
		}
	}
}
