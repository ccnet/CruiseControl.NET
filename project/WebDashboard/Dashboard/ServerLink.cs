
namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class ServerLink : IAbsoluteLink
	{
		private readonly IServerSpecifier serverSpecifier;
		private readonly IActionSpecifier actionSpecifier;
		private readonly IUrlBuilder urlBuilder;
		private readonly string description;
		public readonly string absoluteUrl;

		public ServerLink(IUrlBuilder urlBuilder, IServerSpecifier serverSpecifier, string description, IActionSpecifier actionSpecifier)
		{
			this.description = description;
			this.urlBuilder = urlBuilder;
			this.actionSpecifier = actionSpecifier;
			this.serverSpecifier = serverSpecifier;
		}

		public string Description
		{
			get { return description; }
		}

		public string AbsoluteURL
		{
			get { return urlBuilder.BuildServerUrl(actionSpecifier, serverSpecifier); }
		}
	}
}
