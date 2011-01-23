using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.Remote.Security
{
    /// <summary>
    /// Defines the events that can be logged.
    /// </summary>
    public enum SecurityEvent
    {
        /// <summary>
        /// The event type is unknown.
        /// </summary>
        Unknown,
        /// <summary>
        /// A login event.
        /// </summary>
        Login,
        /// <summary>
        /// A logout event.
        /// </summary>
        Logout,
        /// <summary>
        /// A force build event.
        /// </summary>
        ForceBuild,
        /// <summary>
        /// An abort build event.
        /// </summary>
        AbortBuild,
        /// <summary>
        /// A start project event.
        /// </summary>
        StartProject,
        /// <summary>
        /// A stop project event.
        /// </summary>
        StopProject,
        /// <summary>
        /// A cancel request event.
        /// </summary>
        CancelRequest,
        /// <summary>
        /// A send message event.
        /// </summary>
        SendMessage,
        /// <summary>
        /// A get security configuration event.
        /// </summary>
        GetSecurityConfiguration,
        /// <summary>
        /// A list all users event.
        /// </summary>
        ListAllUsers,
        /// <summary>
        /// A diagnose security permission event.
        /// </summary>
        DiagnoseSecurityPermissions,
        /// <summary>
        /// A view audit log event.
        /// </summary>
        ViewAuditLog,
        /// <summary>
        /// A change password event.
        /// </summary>
        ChangePassword,
        /// <summary>
        /// A reset password event.
        /// </summary>
        ResetPassword,
        /// <summary>
        /// An add project event.
        /// </summary>
        AddProject,
        /// <summary>
        /// A delete project event.
        /// </summary>
        DeleteProject,
        /// <summary>
        /// An update project event.
        /// </summary>
        UpdateProject,
        /// <summary>
        /// A request to get the final build status.
        /// </summary>
        GetFinalBuildStatus,
    }
}
