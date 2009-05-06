using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote.Messages
{
    /// <summary>
    /// A request message for security diagnostics.
    /// </summary>
    [XmlRoot("diagnoseSecurityMessage")]
    [Serializable]
    public class DiagnoseSecurityRequest
        : ServerRequest
    {
        #region Private fields
        private List<string> projects = new List<string>();
        private string userName;
        #endregion

        #region Public properties
        #region Projects
        /// <summary>
        /// The projects to diagnose.
        /// </summary>
        [XmlElement("project")]
        public List<string> Projects
        {
            get { return projects; }
            set { projects = value; }
        }
        #endregion

        #region UserName
        /// <summary>
        /// The user name.
        /// </summary>
        [XmlAttribute("userName")]
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }
        #endregion
        #endregion
    }
}
