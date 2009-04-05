using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.Administration
{
    /// <summary>
    /// Defines a configuration attribute.
    /// </summary>
    public class ConfigurationAttribute
    {
        #region Private fields
        private string name;
        private string value;
        #endregion

        #region Public properties
        #region Name
        /// <summary>
        /// The name of the attribute.
        /// </summary>
        [XmlAttribute("name")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        #endregion

        #region Value
        /// <summary>
        /// The value of the setting.
        /// </summary>
        [XmlAttribute("value")]
        public string Value
        {
            get { return value; }
            set { this.value = value; }
        }
        #endregion
        #endregion
    }
}
