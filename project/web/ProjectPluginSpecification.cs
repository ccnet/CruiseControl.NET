using System;

namespace tw.ccnet.web
{
	public class ProjectPluginSpecification
	{
		public string LinkText;
		public string LinkUrl;

		public ProjectPluginSpecification(string LinkText, string LinkUrl)
		{
			this.LinkText = LinkText;
			this.LinkUrl = LinkUrl;
		}
	}
}
