
namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class BuildLinkWithFileName : GeneralAbsoluteLink
	{
		private readonly IBuildSpecifier buildSpecifier;
		private readonly IActionSpecifier actionSpecifier;
		private readonly IUrlBuilder urlBuilder;
		public readonly string absoluteUrl;
		private readonly string fileName;

		public BuildLinkWithFileName(IUrlBuilder urlBuilder, IBuildSpecifier buildSpecifier, string text, IActionSpecifier actionSpecifier, string fileName)
			: base (text)
		{
			this.urlBuilder = urlBuilder;
			this.actionSpecifier = actionSpecifier;
			this.buildSpecifier = buildSpecifier;
			this.fileName = fileName;
		}

		public override string Url
		{
			get { return urlBuilder.BuildBuildUrl(actionSpecifier, buildSpecifier, fileName); }
		}
	}
}
