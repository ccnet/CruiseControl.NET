using System;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Remote
{
	[ReflectorType("externalLink")]
	[Serializable]
	public class ExternalLink
	{
		private string name;
		private string url;

		public ExternalLink() : this ("", "")  { }

		public ExternalLink(string name, string url)
		{
			this.name = name;
			this.url = url;
		}

		[ReflectorProperty("name")] 
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		[ReflectorProperty("url")] 
		public string Url
		{
			get { return url; }
			set { url = value; }
		}
	}
}
