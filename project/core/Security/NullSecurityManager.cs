using Exortech.NetReflector;
using System;
using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Security;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    /// <summary>
    /// A default implementation of a security manager where there is no security (e.g. every right is allowed);
    /// </summary>
    /// <title>Null Server Security</title>
    /// <version>1.5</version>
    /// <example>
    /// <code>
    /// &lt;nullSecurity /&gt;
    /// </code>
    /// </example>
    [ReflectorType("nullSecurity")]
    public class NullSecurityManager
        : ISecurityManager
    {
        /// <summary>
        /// Initialise the security manager.
        /// </summary>
        public void Initialise()
        {
        }

        /// <summary>
        /// Does this manager require a session?
        /// </summary>
        public bool RequiresSession
        {
            get { return false; }
        }

        #region Channel
        /// <summary>
        /// The channel security requirements.
        /// </summary>
        public IChannelSecurity Channel
        {
            get { return null; }
        }
        #endregion

        /// <summary>
        /// Starts a new session for a user.
        /// </summary>
        /// <param name="credentials">The credentials to use.</param>
        /// <returns>The session token if the credentials are valid, null otherwise.</returns>
        public string Login(LoginRequest credentials)
        {
            // Since we need a user name, let's attempt to find a user name in the credentials, otherwise 
            // we'll just have to use string.Empty
            string userName = NameValuePair.FindNamedValue(credentials.Credentials, 
                LoginRequest.UserNameCredential);
            return userName;
        }

        /// <summary>
        /// Terminates a user session.
        /// </summary>
        /// <param name="sessionToken">The token of the user session.</param>
        public void Logout(string sessionToken)
        {
            // Don't do anything because we don't have any sessions
        }

        /// <summary>
        /// Checks that a session is still validate (e.g. hasn't timed out or been terminated.)
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <returns>True if the session is valid, false otherwise.</returns>
        public bool ValidateSession(string sessionToken)
        {
            // Always return true
            return true;
        }

        /// <summary>
        /// Retrieves the user name from a session token.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <returns>The name of the user if the session is valid, null otherwise.</returns>
        /// <remarks>
        /// The session token must be the user name.
        /// </remarks>
        public string GetUserName(string sessionToken)
        {
            // Assume the user name is the session token
            return sessionToken == null ? string.Empty : sessionToken;
        }

        /// <summary>
        /// Retrieves the display name from a session token.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <param name="displayName">The display name that was sent from the client.</param>
        /// <returns>
        /// The name of the user if the session is valid, null otherwise.
        /// </returns>
        public string GetDisplayName(string sessionToken, string displayName)
        {
            // Assume the user name is the session token
            return sessionToken == null ? displayName : sessionToken;
        }

        /// <summary>
        /// Retrieves a user from the store.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public IAuthentication RetrieveUser(string identifier)
        {
            return null;
        }

        /// <summary>
        /// Retrieves a permission from the store.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public IPermission RetrievePermission(string identifier)
        {
            return null;
        }

        /// <summary>
        /// Sends a security event to the audit loggers.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <param name="userName">The name of the user.</param>
        /// <param name="eventType">The type of event.</param>
        /// <param name="eventRight">The right of the event.</param>
        /// <param name="message">Any security message.</param>
        public void LogEvent(string projectName, string userName, SecurityEvent eventType, SecurityRight eventRight, string message)
        {
            // Do nothing
        }

        /// <summary>
        /// Lists all the users who have been defined in the system.
        /// </summary>
        /// <returns>
        /// A list of <see cref="UserDetails"/> containing the details on all the users
        /// who have been defined.
        /// </returns>
        public virtual List<UserDetails> ListAllUsers()
        {
            return new List<UserDetails>();
        }

        /// <summary>
        /// Checks the security permissions for a user against one or more projects.
        /// </summary>
        /// <param name="userName">The name of the user.</param>
        /// <param name="projectNames">The names of the projects to check.</param>
        /// <returns>A set of diagnostics information.</returns>
        public virtual List<SecurityCheckDiagnostics> DiagnoseSecurityPermissions(string userName, params string[] projectNames)
        {
            return new List<SecurityCheckDiagnostics>();
        }

        /// <summary>
        /// Checks whether the user can perform the specified action at the server level.
        /// </summary>
        /// <param name="userName">The name of the user that is being checked.</param>
        /// <param name="permission">The permission to check.</param>
        /// <returns>True if the permission is valid, false otherwise.</returns>
        public virtual bool CheckServerPermission(string userName, SecurityPermission permission)
        {
            return true;
        }

        /// <summary>
        /// Reads all the specified number of audit events.
        /// </summary>
        /// <param name="startPosition">The starting position.</param>
        /// <param name="numberOfRecords">The number of records to read.</param>
        /// <returns>A list of <see cref="AuditRecord"/>s containing the audit details.</returns>
        public virtual List<AuditRecord> ReadAuditRecords(int startPosition, int numberOfRecords)
        {
            List<AuditRecord> records = new List<AuditRecord>();
            return records;
        }

        /// <summary>
        /// Reads all the specified number of filtered audit events.
        /// </summary>
        /// <param name="startPosition">The starting position.</param>
        /// <param name="numberOfRecords">The number of records to read.</param>
        /// <param name="filter">The filter to use.</param>
        /// <returns>A list of <see cref="AuditRecord"/>s containing the audit details that match the filter.</returns>
        public virtual List<AuditRecord> ReadAuditRecords(int startPosition, int numberOfRecords, AuditFilterBase filter)
        {
            List<AuditRecord> records = new List<AuditRecord>();
            return records;
        }

        #region ChangePassword()
        /// <summary>
        /// Changes the password of the user.
        /// </summary>
        /// <param name="sessionToken">The session token for the current user.</param>
        /// <param name="oldPassword">The person's old password.</param>
        /// <param name="newPassword">The person's new password.</param>
        public virtual void ChangePassword(string sessionToken, string oldPassword, string newPassword)
        {
            throw new NotImplementedException("Password management is not allowed for this security manager");
        }
        #endregion

        #region ResetPassword()
        /// <summary>
        /// Resets the password for a user.
        /// </summary>
        /// <param name="sessionToken">The session token for the current user.</param>
        /// <param name="userName">The user name to reset the password for.</param>
        /// <param name="newPassword">The person's new password.</param>
        public virtual void ResetPassword(string sessionToken, string userName, string newPassword)
        {
            throw new NotImplementedException("Password management is not allowed for this security manager");
        }
        #endregion

        #region RetrieveComponent()
        /// <summary>
        /// Retrieves a component from the security manager.
        /// </summary>
        /// <typeparam name="TComponent">The type of component to retrieve.</typeparam>
        /// <returns>The component of the specified type, if available, null otherwise.</returns>
        public TComponent RetrieveComponent<TComponent>()
            where TComponent : class
        {
            return null;
        }
        #endregion

        #region GetDefaultRight()
        /// <summary>
        /// Gets the default right for a permission.
        /// </summary>
        /// <param name="permission">The permission to retrieve the default for.</param>
        /// <returns>The default right.</returns>
        public virtual SecurityRight GetDefaultRight(SecurityPermission permission)
        {
            return SecurityRight.Allow;
        }
        #endregion
    }
}
