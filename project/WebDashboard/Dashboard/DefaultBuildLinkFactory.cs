namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class DefaultBuildLinkFactory : IBuildLinkFactory
	{
		private readonly IUrlBuilder urlBuilder;

		public DefaultBuildLinkFactory(IUrlBuilder urlBuilder)
		{
			this.urlBuilder = urlBuilder;
		}

		public IAbsoluteLink CreateBuildLink(IBuildSpecifier buildSpecifier, string description, IActionSpecifier actionSpecifier)
		{
			return new BuildLink(urlBuilder, buildSpecifier, description, actionSpecifier);
		}
	}
}
