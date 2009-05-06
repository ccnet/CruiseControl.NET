using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// Provides diagnostics on a security check.
    /// </summary>
    [Serializable]
    public class SecurityCheckDiagnostics
    {
        #region Private fields
        private string permissionName;
        private string projectName;
        private string userName;
        private bool isAllowed;
        #endregion

        #region Public properties
        #region Permission
        /// <summary>
        /// The name of the permission being diagnosed.
        /// </summary>
        [XmlAttribute("permission")]
        public string Permission
        {
            get { return permissionName; }
            set { permissionName = value; }
        }
        #endregion

        #region Project
        /// <summary>
        /// The name of the project the permission is being checked against.
        /// </summary>
        [XmlAttribute("project")]
        public string Project
        {
            get { return projectName; }
            set { projectName = value; }
        }
        #endregion

        #region User
        /// <summary>
        /// The name of the user being the permission is being checked for.
        /// </summary>
        [XmlAttribute("user")]
        public string User
        {
            get { return userName; }
            set { userName = value; }
        }
        #endregion

        #region IsAllowed
        /// <summary>
        /// Whether this permission is allowed.
        /// </summary>
        [XmlAttribute("allowed")]
        public bool IsAllowed
        {
            get { return isAllowed; }
            set { isAllowed = value; }
        }
        #endregion
        #endregion
    }
}
