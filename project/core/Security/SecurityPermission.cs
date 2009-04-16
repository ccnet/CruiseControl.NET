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
        /// Can force build the project.
        /// </summary>
        ForceBuild,
        /// <summary>
        /// Can start the project.
        /// </summary>
        StartProject,
        /// <summary>
        /// Can stop the project.
        /// </summary>
        StopProject,
        /// <summary>
        /// Can view security information.
        /// </summary>
        ViewSecurity,
    }
}
