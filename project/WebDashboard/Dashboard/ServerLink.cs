
namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class ServerLink : GeneralAbsoluteLink
	{
		private readonly IServerSpecifier serverSpecifier;
		private readonly IActionSpecifier actionSpecifier;
		private readonly IUrlBuilder urlBuilder;
		public readonly string absoluteUrl;

		public ServerLink(IUrlBuilder urlBuilder, IServerSpecifier serverSpecifier, string text, IActionSpecifier actionSpecifier)
			: base (text)
		{
			this.urlBuilder = urlBuilder;
			this.actionSpecifier = actionSpecifier;
			this.serverSpecifier = serverSpecifier;
		}

		public override string Url
		{
			get { return urlBuilder.BuildServerUrl(actionSpecifier, serverSpecifier); }
		}
	}
}
