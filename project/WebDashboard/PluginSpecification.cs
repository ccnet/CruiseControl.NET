using System;

namespace ThoughtWorks.CruiseControl.WebDashboard
{
	public class PluginSpecification
	{
		public string LinkText;
		public string LinkUrl;

		public PluginSpecification(string LinkText, string LinkUrl)
		{
			this.LinkText = LinkText;
			this.LinkUrl = LinkUrl;
		}
	}
}
