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
        /// Can force or abort builds.
        /// </summary>
        ForceBuild,
        /// <summary>
        /// Can start or stop projects.
        /// </summary>
        StartProject,
        /// <summary>
        /// Can view security information.
        /// </summary>
        ViewSecurity,
    }
}
