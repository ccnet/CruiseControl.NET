using System;
#if !NoReflector
using Exortech.NetReflector;
#endif
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <title>External Links</title>
    /// <version>1.0</version>
    /// <summary>
    /// The &lt;externalLinks&gt; section can contain any number of &lt;externalLink&gt; sub-sections. Each of these 
    /// are used to display project related links on the project report page of the Web Dashboard, and are meant as a
    /// convenient shortcut to project-related web sites outside of CruiseControl.NET.
    /// </summary>
    /// <example>
    /// <code>
    /// &lt;externalLinks&gt;
    /// &lt;externalLink name="My Link" url="http://somewhere" /&gt;
    /// &lt;/externalLinks&gt;
    /// </code>
    /// </example>
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
        /// <version>1.0</version>
        /// <default>n/a</default>
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
        /// <version>1.0</version>
        /// <default>n/a</default>
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
