
namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class ProjectLink : GeneralAbsoluteLink
	{
		private readonly IProjectSpecifier buildSpecifier;
		private readonly IActionSpecifier actionSpecifier;
		private readonly IUrlBuilder urlBuilder;
		public readonly string absoluteUrl;

		public ProjectLink(IUrlBuilder urlBuilder, IProjectSpecifier projectSpecifier, string text, IActionSpecifier actionSpecifier)
			: base (text)
		{
			this.urlBuilder = urlBuilder;
			this.actionSpecifier = actionSpecifier;
			this.buildSpecifier = projectSpecifier;
		}

		public override string Url
		{
			get { return urlBuilder.BuildProjectUrl(actionSpecifier, buildSpecifier); }
		}
	}
}
