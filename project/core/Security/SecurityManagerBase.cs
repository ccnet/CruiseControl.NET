
using Exortech.NetReflector;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Security;
using ThoughtWorks.CruiseControl.Core.Security.Auditing;
using ThoughtWorks.CruiseControl.Core.Config;
using System.Globalization;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    /// <summary>
    /// Provides some base security manager functionality.
    /// </summary>
    public abstract class SecurityManagerBase
        : ISecurityManager
    {
        #region Private constants
        private const string displayNameKey = "DisplayName";
        #endregion

        #region Private fields
        private ISessionCache sessionCache = new InMemorySessionCache();
        private IAuditLogger[] loggers = new IAuditLogger[0];
        private IAuditReader reader;
        private Permissions permissions = new Permissions();
        #endregion

        #region Public properties
        #region SessionCache
        /// <summary>
        /// The associated session cache.
        /// </summary>
        /// <version>1.5</version>
        /// <default><link>In Memory Security Cache</link></default>
        [ReflectorProperty("cache", InstanceTypeKey = "type", Required = false)]
        public ISessionCache SessionCache
        {
            get { return sessionCache; }
            set { sessionCache = value; }
        }
        #endregion

        #region AuditLoggers
        /// <summary>
        /// The audit loggers.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("audit", Required=false)]
        public IAuditLogger[] AuditLoggers
        {
            get { return loggers; }
            set { loggers = value; }
        }
        #endregion

        #region AuditReader
        /// <summary>
        /// The audit reader.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("auditReader", InstanceTypeKey = "type", Required = false)]
        public IAuditReader AuditReader
        {
            get { return reader; }
            set { reader = value; }
        }
        #endregion

        #region Permissions
        /// <summary>
        /// The default permissions.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("defaults", Required=false, InstanceType=typeof(Permissions))]
        public Permissions DefaultPermissions
        {
            get { return permissions; }
            set { permissions = value; }
        }
        #endregion

        #region RequiresSession
        /// <summary>
        /// Does this manager require a session?
        /// </summary>
        public bool RequiresSession
        {
            get { return permissions.DefaultRight == SecurityRight.Deny; }
        }
        #endregion

        #region Channel
        /// <summary>
        /// The channel security requirements.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("channel", InstanceTypeKey = "type", Required = false)]
        public virtual IChannelSecurity Channel { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region Initialise()
        /// <summary>
        /// Initialise the security manager.
        /// </summary>
        public abstract void Initialise();
        #endregion

        #region RetrieveUser()
        /// <summary>
        /// Retrieves a user from the store.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public abstract IAuthentication RetrieveUser(string identifier);
        #endregion

        #region RetrievePermission()
        /// <summary>
        /// Retrieves a permission from the store.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public abstract IPermission RetrievePermission(string identifier);
        #endregion

        #region Login()
        /// <summary>
        /// Starts a new session for a user.
        /// </summary>
        /// <param name="credentials">The credentials to use.</param>
        /// <returns>The session token if the credentials are valid, null otherwise.</returns>
        public virtual string Login(LoginRequest credentials)
        {
            string sessionToken = null;

            // Find the authentication
            string identifier = NameValuePair.FindNamedValue(credentials.Credentials,
                LoginRequest.UserNameCredential);
            IAuthentication authentication = RetrieveUser(identifier);

            string userName = credentials.Identifier;
            string displayName = null;
            if (authentication != null)
            {
                userName = authentication.GetUserName(credentials);
                // Attempt to authenticate
                if (authentication.Authenticate(credentials))
                {
                    // Start a new session
                    sessionToken = sessionCache.AddToCache(userName);
                    displayName = authentication.GetDisplayName(credentials);
                    sessionCache.StoreSessionValue(sessionToken, displayNameKey, displayName);
                }
            }

            if (sessionToken != null)
            {
                Log.Debug(string.Format("{0} [{1}] has logged in", displayName, userName));
                LogEvent(null, userName, SecurityEvent.Login, SecurityRight.Allow, null);
            }
            else
            {
                Log.Warning(string.Format("Login failure: {0} has failed to login", userName));
                LogEvent(null, userName, SecurityEvent.Login, SecurityRight.Deny, null);
            }

            return sessionToken;
        }
        #endregion

        #region Logout()
        /// <summary>
        /// Terminates a user session.
        /// </summary>
        /// <param name="sessionToken">The token of the user session.</param>
        public virtual void Logout(string sessionToken)
        {
            string userName = sessionCache.RetrieveFromCache(sessionToken);
            if (!string.IsNullOrEmpty(userName))
            {
                sessionCache.RemoveFromCache(sessionToken);
                Log.Debug(string.Format("{0} has logged out", userName));
                LogEvent(null, userName, SecurityEvent.Logout, SecurityRight.Allow, null);
            }
            else
            {
                LogEvent(null, null, SecurityEvent.Logout, SecurityRight.Deny, "Session has already been logged out");
            }
        }
        #endregion

        #region ValidateSession()
        /// <summary>
        /// Checks that a session is still validate (e.g. hasn't timed out or been terminated.)
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <returns>True if the session is valid, false otherwise.</returns>
        public virtual bool ValidateSession(string sessionToken)
        {
            if (sessionToken == null) return false;
            string userName = sessionCache.RetrieveFromCache(sessionToken);
            return (userName != null);
        }
        #endregion

        #region GetUserName()
        /// <summary>
        /// Retrieves the user name from a session token.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <returns>The name of the user if the session is valid, null otherwise.</returns>
        /// <remarks>
        /// The session token must be the user name.
        /// </remarks>
        public virtual string GetUserName(string sessionToken)
        {
            if (sessionToken == null) return null;
            string userName = sessionCache.RetrieveFromCache(sessionToken);
            return userName;
        }
        #endregion

        #region GetDisplayName()
        /// <summary>
        /// Retrieves the display name from a session token.
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <returns>The name of the user if the session is valid, null otherwise.</returns>
        public virtual string GetDisplayName(string sessionToken)
        {
            if (sessionToken == null) return null;
            string displayName = sessionCache.RetrieveSessionValue(sessionToken, displayNameKey) as string;
            return displayName;
        }
        #endregion

        #region LogEvent()
        /// <summary>
        /// Sends a security event to the audit loggers.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <param name="userName">The name of the user.</param>
        /// <param name="eventType">The type of event.</param>
        /// <param name="eventRight">The right of the event.</param>
        /// <param name="message">Any security message.</param>
        public virtual void LogEvent(string projectName, string userName, SecurityEvent eventType, SecurityRight eventRight, string message)
        {
            if (loggers != null)
            {
                foreach (IAuditLogger logger in loggers)
                {
                    logger.LogEvent(projectName, userName, eventType, eventRight, message);
                }
            }
        }
        #endregion

        #region ListAllUsers()
        /// <summary>
        /// Lists all the users who have been defined in the system.
        /// </summary>
        /// <returns>
        /// A list of <see cref="UserDetails"/> containing the details on all the users
        /// who have been defined.
        /// </returns>
        public abstract List<UserDetails> ListAllUsers();
        #endregion

        #region CheckServerPermission()
        /// <summary>
        /// Checks whether the user can perform the specified action at the server level.
        /// </summary>
        /// <param name="userName">The name of the user that is being checked.</param>
        /// <param name="permission">The permission to check.</param>
        /// <returns>True if the permission is valid, false otherwise.</returns>
        public abstract bool CheckServerPermission(string userName, SecurityPermission permission);
        #endregion

        #region ReadAuditRecords()
        /// <summary>
        /// Reads all the specified number of audit events.
        /// </summary>
        /// <param name="startPosition">The starting position.</param>
        /// <param name="numberOfRecords">The number of records to read.</param>
        /// <returns>A list of <see cref="AuditRecord"/>s containing the audit details.</returns>
        public virtual List<AuditRecord> ReadAuditRecords(int startPosition, int numberOfRecords)
        {
            List<AuditRecord> records = new List<AuditRecord>();
            if (reader != null) records = reader.Read(startPosition, numberOfRecords);
            return records;
        }
        #endregion

        #region ReadAuditRecords()
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
            if (reader != null) records = reader.Read(startPosition, numberOfRecords, filter);
            return records;
        }
        #endregion

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
            return permissions.GetPermission(permission);
        }
        #endregion
        #endregion
    }
}
