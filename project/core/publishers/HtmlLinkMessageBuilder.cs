
namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	public class HtmlLinkMessageBuilder : IMessageBuilder
	{
		private bool includeAnchorTag;

		public HtmlLinkMessageBuilder(bool includeAnchorTag)
		{
			this.includeAnchorTag = includeAnchorTag;
		}

		public string BuildMessage(IIntegrationResult result, string projectUrl)
		{
			string link = LogFileUtil.CreateUrl(projectUrl, result);
			if (includeAnchorTag) link = string.Format(@"<a href=""{0}"">web page</a>", link);
			return string.Format("CruiseControl.NET Build Results for project {0} ({1})", result.ProjectName, link);
		}
	}
}
