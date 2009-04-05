using System.Collections.Generic;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.Administration
{
    /// <summary>
    /// Defines a file location for a package and the contained files.
    /// </summary>
    public class FileLocation
    {
        #region Private fields
        private string location;
        private List<string> files = new List<string>();
        #endregion

        #region Public properties
        #region Location
        /// <summary>
        /// The location for the files.
        /// </summary>
        [XmlElement("location")]
        public string Location
        {
            get { return location; }
            set { location = value; }
        }
        #endregion

        #region Files
        /// <summary>
        /// The files to include in the location.
        /// </summary>
        [XmlArray("files")]
        [XmlArrayItem("file")]
        public List<string> Files
        {
            get { return files; }
        }
        #endregion
        #endregion
    }
}
