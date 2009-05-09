using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    /// <summary>
    /// Defines the allowed security permissions.
    /// </summary>
    public enum SecurityPermission
    {
        /// <summary>
        /// Can send a message for the project.
        /// </summary>
        SendMessage,
        /// <summary>
        /// Can force a build or abort a build on a project.
        /// </summary>
        ForceAbortBuild,
        /// <summary>
        /// Can start or stop a project.
        /// </summary>
        StartStopProject,
        /// <summary>
        /// Can change project configuration.
        /// </summary>
        ChangeProjectConfiguration,
        /// <summary>
        /// Can view security information.
        /// </summary>
        ViewSecurity,
        /// <summary>
        /// Can modify security information.
        /// </summary>
        ModifySecurity,
        /// <summary>
        /// Can view a project and all its details.
        /// </summary>
        ViewProject,
        /// <summary>
        /// Can view configuration and log information.
        /// </summary>
        ViewConfiguration,
    }
}
