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
        Unknown,
        Login,
        Logout,
        ForceBuild,
        AbortBuild,
        StartProject,
        StopProject,
        CancelRequest,
        SendMessage,
        GetSecurityConfiguration,
        ListAllUsers,
        DiagnoseSecurityPermissions,
        ViewAuditLog,
    }
}
