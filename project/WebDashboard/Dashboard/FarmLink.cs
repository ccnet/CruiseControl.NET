using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class FarmLink : GeneralAbsoluteLink
	{
		private readonly string action;
		private readonly IUrlBuilder urlBuilder;
		public readonly string absoluteUrl;

		public FarmLink(IUrlBuilder urlBuilder, string text, string action)
			: base (text)
		{
			this.urlBuilder = urlBuilder;
			this.action = action;
		}

		public override string Url
		{
			get { return urlBuilder.BuildUrl(action); }
		}
	}
}
