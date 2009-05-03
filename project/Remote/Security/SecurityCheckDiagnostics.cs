using System;
using System.Collections.Generic;

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
        public string Permission
        {
            get { return permissionName; }
            set { permissionName = value; }
        }

        /// <summary>
        /// The name of the project the permission is being checked against.
        /// </summary>
        public string Project
        {
            get { return projectName; }
            set { projectName = value; }
        }

        /// <summary>
        /// The name of the user being the permission is being checked for.
        /// </summary>
        public string User
        {
            get { return userName; }
            set { userName = value; }
        }

        /// <summary>
        /// Whether this permission is allowed.
        /// </summary>
        public bool IsAllowed
        {
            get { return isAllowed; }
            set { isAllowed = value; }
        }
    }
}
