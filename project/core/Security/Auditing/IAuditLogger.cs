using System;
using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.Core.Security.Auditing
{
    /// <summary>
    /// Provides an interface for logging security events.
    /// </summary>
    public interface IAuditLogger
    {
        /// <summary>
        /// Logs a security event.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <param name="userName">The name of the user.</param>
        /// <param name="eventType">The type of event.</param>
        /// <param name="eventRight">The right of the event.</param>
        /// <param name="message">Any security message.</param>
        void LogEvent(string projectName, string userName, SecurityEvent eventType, SecurityRight eventRight, string message);

        /// <summary>
        /// Logs an audit record.
        /// </summary>
        /// <param name="record">The record to log.</param>
        void LogEvent(AuditRecord record);
    }
}
