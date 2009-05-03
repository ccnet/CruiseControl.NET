using Exortech.NetReflector;
using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Security;


namespace ThoughtWorks.CruiseControl.Core.Security
{
    /// <summary>
    /// Stores a user name - authentication is merely that the names match.
    /// </summary>
    [ReflectorType("simpleUser")]
    public class UserNameAuthentication
        : IAuthentication
    {
        private const string userNameCredential = "username";

        private string userName;
        private string displayName;
        private ISecurityManager manager;

        /// <summary>
        /// Start a new blank authentication.
        /// </summary>
        public UserNameAuthentication() { }

        /// <summary>
        /// Start a new authentication with a user name.
        /// </summary>
        /// <param name="userName"></param>
        public UserNameAuthentication(string userName)
        {
            this.userName = userName;
        }

        /// <summary>
        /// A unique identifier for an authentication instance.
        /// </summary>
        public string Identifier
        {
            get { return userName; }
        }

        /// <summary>
        /// The login name for this user.
        /// </summary>
        [ReflectorProperty("name")]
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        /// <summary>
        /// The display name for this user.
        /// </summary>
        [ReflectorProperty("display", Required = false)]
        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }

        /// <summary>
        /// The name of the authentication type.
        /// </summary>
        public string AuthenticationName
        {
            get { return "Simple"; }
        }

        #region Manager
        /// <summary>
        /// The security manager that loaded this setting.
        /// </summary>
        public ISecurityManager Manager
        {
            get { return manager; }
            set { manager = value; }
        }
        #endregion

        /// <summary>
        /// Attempts to authenticate a user from the credentials.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <returns>True if the credentials are valid, false otherwise.</returns>
        public bool Authenticate(ISecurityCredentials credentials)
        {
            // Check that the user name matches
            string userName = credentials[userNameCredential];
            bool isValid = !string.IsNullOrEmpty(userName);
            if (isValid) isValid = SecurityHelpers.IsWildCardMatch(this.userName, userName);
            return isValid;
        }

        /// <summary>
        /// Retrieves the user name from the credentials.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <returns>The name of the user from the credentials. If the credentials not not exist in the system
        /// then null will be returned.</returns>
        public string GetUserName(ISecurityCredentials credentials)
        {
            string userName = credentials[userNameCredential];
            return userName;
        }

        /// <summary>
        /// Retrieves the display name from the credentials.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <returns>The name of the user from the credentials. If the credentials do not exist in the system
        /// then null will be returned.</returns>
        public string GetDisplayName(ISecurityCredentials credentials)
        {
            string nameToReturn = displayName;
            if (string.IsNullOrEmpty(displayName)) nameToReturn = GetUserName(credentials);
            return nameToReturn;
        }

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="newPassword"></param>
        public void ChangePassword(string newPassword)
        {
            // Do nothing since this authentication does not have a password
        }
    }
}
