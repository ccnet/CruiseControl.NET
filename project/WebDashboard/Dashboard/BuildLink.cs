
namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class BuildLink : GeneralAbsoluteLink
	{
		private readonly IBuildSpecifier buildSpecifier;
		private readonly IActionSpecifier actionSpecifier;
		private readonly IUrlBuilder urlBuilder;
		public readonly string absoluteUrl;

		public BuildLink(IUrlBuilder urlBuilder, IBuildSpecifier buildSpecifier, string text, IActionSpecifier actionSpecifier)
			: base (text)
		{
			this.urlBuilder = urlBuilder;
			this.actionSpecifier = actionSpecifier;
			this.buildSpecifier = buildSpecifier;
		}

		public override string Url
		{
			get { return urlBuilder.BuildBuildUrl(actionSpecifier, buildSpecifier); }
		}
	}
}
