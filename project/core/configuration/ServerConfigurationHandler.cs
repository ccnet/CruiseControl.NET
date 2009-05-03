using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Config
{
    /// <summary>
    /// Provides additional configuration settings for the server.
    /// </summary>
    /// <remarks>
    /// Currently this only retrieves a list of type names, but it could be extended in future
    /// to load additional settings (perhaps in the same way as the custom builders work).
    /// </remarks>
    public sealed class ServerConfigurationHandler
        : IConfigurationSectionHandler
    {
        #region Create()
        /// <summary>
        /// Retrieve the list of extensions to load.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="configContext">The context.</param>
        /// <param name="section">The section that is being loaded.</param>
        /// <returns>An array of strings containing the type names.</returns>
        public object Create(object parent, object configContext, XmlNode section)
        {
            ServerConfiguration configuration = new ServerConfiguration();

            // Load the extensions
            foreach (XmlNode node in section.SelectNodes("extension"))
            {
                ExtensionConfiguration config = new ExtensionConfiguration();
                config.Type = node.Attributes["type"].Value;
                List<XmlElement> items = new List<XmlElement>();
                foreach (XmlElement itemEl in node.SelectNodes("*"))
                {
                    items.Add(itemEl);
                }
                config.Items = items.ToArray();
                configuration.Extensions.Add(config);
            }

            return configuration;
        }
        #endregion
    }
}
