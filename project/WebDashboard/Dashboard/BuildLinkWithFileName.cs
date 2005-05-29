using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class BuildLinkWithFileName : GeneralAbsoluteLink
	{
		private readonly IBuildSpecifier buildSpecifier;
		private readonly string action;
		private readonly ICruiseUrlBuilder urlBuilder;
		public readonly string absoluteUrl;
		private readonly string fileName;

		public BuildLinkWithFileName(ICruiseUrlBuilder urlBuilder, IBuildSpecifier buildSpecifier, string text, string action, string fileName)
			: base (text)
		{
			this.urlBuilder = urlBuilder;
			this.action = action;
			this.buildSpecifier = buildSpecifier;
			this.fileName = fileName;
		}

		public override string Url
		{
			get { return urlBuilder.BuildBuildUrl(action, buildSpecifier, fileName); }
		}
	}
}
