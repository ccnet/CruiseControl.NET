
namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class ProjectLink : IAbsoluteLink
	{
		private readonly IProjectSpecifier buildSpecifier;
		private readonly IActionSpecifier actionSpecifier;
		private readonly IUrlBuilder urlBuilder;
		private readonly string description;
		public readonly string absoluteUrl;

		public ProjectLink(IUrlBuilder urlBuilder, IProjectSpecifier buildSpecifier, string description, IActionSpecifier actionSpecifier)
		{
			this.description = description;
			this.urlBuilder = urlBuilder;
			this.actionSpecifier = actionSpecifier;
			this.buildSpecifier = buildSpecifier;
		}

		public string Description
		{
			get { return description; }
		}

		public string AbsoluteURL
		{
			get { return urlBuilder.BuildProjectUrl(actionSpecifier, buildSpecifier); }
		}
	}
}
