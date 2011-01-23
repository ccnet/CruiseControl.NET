namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class GeneralAbsoluteLink : IAbsoluteLink
	{
		private readonly string text;
		private readonly string url;
		private string linkClass;

		public GeneralAbsoluteLink(string text) : this (text, string.Empty, string.Empty) { }

		public GeneralAbsoluteLink(string text, string url) : this (text, url, string.Empty) { }

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

        public override bool Equals(object obj)
        {
            GeneralAbsoluteLink other = obj as GeneralAbsoluteLink;

            if (other == null) return false;

            return other.LinkClass == linkClass &&
                   other.Text == Text &&
                   other.Url == Url;            
        }

        public override int GetHashCode()
        {
            return (LinkClass + Text + Url).GetHashCode();
        }
	}
}
