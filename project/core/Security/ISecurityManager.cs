using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Core.Security.Auditing;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Security;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    /// <summary>
    /// Defines the security manager interface. All security calls should go through this manager.
    /// </summary>
    public interface ISecurityManager
    {
        /// <summary>
        /// Does this manager require a session?
        /// </summary>
        bool RequiresSession { get; }

        /// <summary>
        /// Initialise the security manager.
        /// </summary>
        void Initialise();

        /// <summary>
        /// Starts a new session for a user.
        /// </summary>
        /// <param name="credentials">The credentials to use.</param>
        /// <returns>The session token if the credentials are valid, null otherwise.</returns>
        string Login(LoginRequest credentials);

        /// <summary>
        /// Terminates a user session.
        /// </summary>
        /// <param name="sessionToken">The token of the user session.</param>
        void Logout(string sessionToken);

        /// <summary>
        /// Checks that a session is still validate (e.g. hasn't timed out or been terminated.)
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <returns>True if the session is valid, false otherwise.</returns>
        bool ValidateSession(string sessionToken);

        /// <summary>
        /// Retrieves the user name from a session token.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <returns>The name of the user if the session is valid, null otherwise.</returns>
        string GetUserName(string sessionToken);

        /// <summary>
        /// Retrieves the display name from a session token.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <returns>The name of the user if the session is valid, null otherwise.</returns>
        string GetDisplayName(string sessionToken);

        /// <summary>
        /// Retrieves a user from the store.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        IAuthentication RetrieveUser(string identifier);

        /// <summary>
        /// Retrieves a permission from the store.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        IPermission RetrievePermission(string identifier);

        /// <summary>
        /// Sends a security event to the audit loggers.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <param name="userName">The name of the user.</param>
        /// <param name="eventType">The type of event.</param>
        /// <param name="eventRight">The right of the event.</param>
        /// <param name="message">Any security message.</param>
        void LogEvent(string projectName, string userName, SecurityEvent eventType, SecurityRight eventRight, string message);

        /// <summary>
        /// Lists all the users who have been defined in the system.
        /// </summary>
        /// <returns>
        /// A list of <see cref="UserDetails"/> containing the details on all the users
        /// who have been defined.
        /// </returns>
        List<UserDetails> ListAllUsers();

        #region GetDefaultRight()
        /// <summary>
        /// Gets the default right for a permission.
        /// </summary>
        /// <param name="permission">The permission to retrieve the default for.</param>
        /// <returns>The default right.</returns>
        SecurityRight GetDefaultRight(SecurityPermission permission);
        #endregion

        /// <summary>
        /// Checks whether the user can perform the specified action at the server level.
        /// </summary>
        /// <param name="userName">The name of the user that is being checked.</param>
        /// <param name="permission">The permission to check.</param>
        /// <returns>True if the permission is valid, false otherwise.</returns>
        bool CheckServerPermission(string userName, SecurityPermission permission);

        /// <summary>
        /// Reads all the specified number of audit events.
        /// </summary>
        /// <param name="startPosition">The starting position.</param>
        /// <param name="numberOfRecords">The number of records to read.</param>
        /// <returns>A list of <see cref="AuditRecord"/>s containing the audit details.</returns>
        List<AuditRecord> ReadAuditRecords(int startPosition, int numberOfRecords);

        /// <summary>
        /// Reads all the specified number of filtered audit events.
        /// </summary>
        /// <param name="startPosition">The starting position.</param>
        /// <param name="numberOfRecords">The number of records to read.</param>
        /// <param name="filter">The filter to use.</param>
        /// <returns>A list of <see cref="AuditRecord"/>s containing the audit details that match the filter.</returns>
        List<AuditRecord> ReadAuditRecords(int startPosition, int numberOfRecords, AuditFilterBase filter);

        /// <summary>
        /// Changes the password of the user.
        /// </summary>
        /// <param name="sessionToken">The session token for the current user.</param>
        /// <param name="oldPassword">The person's old password.</param>
        /// <param name="newPassword">The person's new password.</param>
        void ChangePassword(string sessionToken, string oldPassword, string newPassword);

        /// <summary>
        /// Resets the password for a user.
        /// </summary>
        /// <param name="sessionToken">The session token for the current user.</param>
        /// <param name="userName">The user name to reset the password for.</param>
        /// <param name="newPassword">The person's new password.</param>
        void ResetPassword(string sessionToken, string userName, string newPassword);

        /// <summary>
        /// Retrieves a component from the security manager.
        /// </summary>
        /// <typeparam name="TComponent">The type of component to retrieve.</typeparam>
        /// <returns>The component of the specified type, if available, null otherwise.</returns>
        TComponent RetrieveComponent<TComponent>()
            where TComponent : class;

        #region Channel
        /// <summary>
        /// The channel security requirements.
        /// </summary>
        IChannelSecurity Channel { get; }
        #endregion
    }
}
