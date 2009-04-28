using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote.Security
{
    /// <summary>
    /// The details on a user.
    /// </summary>
    [XmlRoot("user")]
    [Serializable]
    public class UserDetails
    {
        #region Private fields
        private string userName;
        private string displayName;
        private string type;
        #endregion

        #region Public properties
        #region UserName
        /// <summary>
        /// The login (user) name of the user.
        /// </summary>
        [XmlAttribute("name")]
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }
        #endregion

        #region DisplayName
        /// <summary>
        /// The display name of the user.
        /// </summary>
        [XmlAttribute("display")]
        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }
        #endregion

        #region Type
        /// <summary>
        /// The type of the user.
        /// </summary>
        [XmlAttribute("type")]
        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        #endregion
        #endregion
    }
}
