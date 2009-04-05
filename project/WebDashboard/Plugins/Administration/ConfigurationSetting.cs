using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.Administration
{
    /// <summary>
    /// Defines a configuration setting to apply.
    /// </summary>
    public class ConfigurationSetting
    {
        #region Private fields
        private string path;
        private string filter;
        private string name;
        private string value;
        private ConfigurationAttribute[] attributes = new ConfigurationAttribute[0];
        #endregion

        #region Public properties
        #region Path
        /// <summary>
        /// The path for the setting.
        /// </summary>
        [XmlElement("path")]
        public string Path
        {
            get { return path; }
            set { path = value; }
        }
        #endregion

        #region Filter
        /// <summary>
        /// An optional filter to apply.
        /// </summary>
        [XmlElement("filter")]
        public string Filter
        {
            get { return filter; }
            set { filter = value; }
        }
        #endregion

        #region Name
        /// <summary>
        /// The name of the setting.
        /// </summary>
        [XmlElement("name")]
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
        [XmlElement("value")]
        public string Value
        {
            get { return value; }
            set { this.value = value; }
        }
        #endregion

        #region Attributes
        /// <summary>
        /// The attributes for the element.
        /// </summary>
        [XmlArray("attributes")]
        [XmlArrayItem("attribute")]
        public ConfigurationAttribute[] Attributes
        {
            get { return attributes; }
            set { attributes = value; }
        }
        #endregion
        #endregion
    }
}
