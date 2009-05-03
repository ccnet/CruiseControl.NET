using System;
using System.Xml;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// Defines the configuration for a server extension.
    /// </summary>
    public class ExtensionConfiguration
    {
        #region Private fields
        private string type;
        private XmlElement[] configurationItems;
        #endregion

        #region Public properties
        #region Type
        /// <summary>
        /// Gets or sets the type of the component.
        /// </summary>
        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        #endregion

        #region Items
        /// <summary>
        /// Gets or sets the additional items.
        /// </summary>
        public XmlElement[] Items
        {
            get { return configurationItems; }
            set { configurationItems = value; }
        }
        #endregion
        #endregion
    }
}
