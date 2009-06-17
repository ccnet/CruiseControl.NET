using System;
#if !NoReflector
using Exortech.NetReflector;
#endif
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// A link to an external resource.
    /// </summary>
#if !NoReflector
	[ReflectorType("externalLink")]
#endif
	[Serializable]
    [XmlRoot("externalLink")]
	public class ExternalLink
	{
		private string name;
		private string url;

        /// <summary>
        /// Initialise a new blank <see cref="ExternalLink"/>.
        /// </summary>
		public ExternalLink() : this (string.Empty, string.Empty)  { }

        /// <summary>
        /// Initialise a new populated <see cref="ExternalLink"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="url"></param>
		public ExternalLink(string name, string url)
		{
			this.name = name;
			this.url = url;
		}

        /// <summary>
        /// The name of the link.
        /// </summary>
#if !NoReflector
		[ReflectorProperty("name")] 
#endif
        [XmlAttribute("name")]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

        /// <summary>
        /// The URL for the link.
        /// </summary>
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
