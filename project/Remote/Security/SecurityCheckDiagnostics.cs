using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote.Security
{
    /// <summary>
    /// Provides diagnostics on a security check.
    /// </summary>
    [Serializable]
    public class SecurityCheckDiagnostics
    {
        private string permissionName;
        private string projectName;
        private string userName;
        private bool isAllowed;

        /// <summary>
        /// The name of the permission being diagnosed.
        /// </summary>
        [XmlAttribute("permission")]
        public string Permission
        {
            get { return permissionName; }
            set { permissionName = value; }
        }

        /// <summary>
        /// The name of the project the permission is being checked against.
        /// </summary>
        [XmlAttribute("project")]
        public string Project
        {
            get { return projectName; }
            set { projectName = value; }
        }

        /// <summary>
        /// The name of the user being the permission is being checked for.
        /// </summary>
        [XmlAttribute("user")]
        public string User
        {
            get { return userName; }
            set { userName = value; }
        }

        /// <summary>
        /// Whether this permission is allowed.
        /// </summary>
        [XmlAttribute("allowed")]
        public bool IsAllowed
        {
            get { return isAllowed; }
            set { isAllowed = value; }
        }
    }
}
