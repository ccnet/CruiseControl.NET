namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class GeneralAbsoluteLink : IAbsoluteLink
	{
		private readonly string text;
		private readonly string url;
		private string linkClass;

		public GeneralAbsoluteLink(string text) : this (text, "", "") { }

		public GeneralAbsoluteLink(string text, string url) : this (text, url, "") { }

		public GeneralAbsoluteLink(string text, string url, string linkClass)
		{
			this.text = text;
			this.url = url;
			this.linkClass = linkClass;
		}

		public virtual string Text
		{
			get { return text; }
		}

		public virtual string Url
		{
			get { return url; }
		}

		public virtual string LinkClass
		{
			set { linkClass = value; }
			get { return linkClass; }
		}
	}
}
