using System;
#if !NoReflector
using Exortech.NetReflector;
#endif
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote
{
#if !NoReflector
	[ReflectorType("externalLink")]
#endif
	[Serializable]
    [XmlRoot("externalLink")]
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

#if !NoReflector
		[ReflectorProperty("name")] 
#endif
        [XmlAttribute("name")]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

#if !NoReflector
		[ReflectorProperty("url")] 
#endif
        [XmlAttribute("url")]
		public string Url
		{
			get { return url; }
			set { url = value; }
		}
	}
}
