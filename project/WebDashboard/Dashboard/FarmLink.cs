
namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class FarmLink : GeneralAbsoluteLink
	{
		private readonly IActionSpecifier actionSpecifier;
		private readonly IUrlBuilder urlBuilder;
		public readonly string absoluteUrl;

		public FarmLink(IUrlBuilder urlBuilder, string text, IActionSpecifier actionSpecifier)
			: base (text)
		{
			this.urlBuilder = urlBuilder;
			this.actionSpecifier = actionSpecifier;
		}

		public override string Url
		{
			get { return urlBuilder.BuildUrl(actionSpecifier); }
		}
	}
}
