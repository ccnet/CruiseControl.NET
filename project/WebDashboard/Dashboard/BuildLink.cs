
namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class BuildLink : IAbsoluteLink
	{
		private readonly IActionSpecifier actionSpecifier;
		private readonly string buildName;
		private readonly string projectName;
		private readonly string serverName;
		private readonly IUrlBuilder urlBuilder;
		private readonly string description;
		public readonly string absoluteUrl;

		public BuildLink(IUrlBuilder urlBuilder, string serverName, string projectName, string buildName, string description, IActionSpecifier actionSpecifier)
		{
			this.description = description;
			this.urlBuilder = urlBuilder;
			this.serverName = serverName;
			this.projectName = projectName;
			this.buildName = buildName;
			this.actionSpecifier = actionSpecifier;
		}

		public string Description
		{
			get { return description; }
		}

		public string AbsoluteURL
		{
			get { return urlBuilder.BuildBuildUrl(actionSpecifier, serverName, projectName, buildName); }
		}
	}
}
