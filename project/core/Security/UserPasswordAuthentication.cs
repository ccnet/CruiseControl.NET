using Exortech.NetReflector;
using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    /// <summary>
    /// Stores a user name and password - authentication is that the password is valid for the user name.
    /// </summary>
    [ReflectorType("passwordUser")]
    public class UserPasswordAuthentication
        : IAuthentication
    {
        private const string userNameCredential = "username";
        private const string passwordCredential = "password";

        private string userName;
        private string password;
        private string displayName;
        private ISecurityManager manager;

        /// <summary>
        /// Start a new blank authentication.
        /// </summary>
        public UserPasswordAuthentication() { }

        /// <summary>
        /// Start a new authentication with a user name and password.
        /// </summary>
        /// <param name="userName"></param>
        public UserPasswordAuthentication(string userName, string password)
        {
            this.userName = userName;
            this.password = password;
        }

        /// <summary>
        /// A unique identifier for an authentication instance.
        /// </summary>
        public string Identifier
        {
            get { return userName; }
        }

        /// <summary>
        /// The name for this user.
        /// </summary>
        [ReflectorProperty("name")]
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        /// <summary>
        /// The password for this user.
        /// </summary>
        [ReflectorProperty("password")]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        /// <summary>
        /// The name of the authentication type.
        /// </summary>
        public string AuthenticationName
        {
            get { return "Password"; }
        }

        /// <summary>
        /// The display name for this user.
        /// </summary>
        [ReflectorProperty("display", Required=false)]
        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
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
        /// <returns>True if the credentials are valid, false otherwise..</returns>
        public bool Authenticate(LoginRequest credentials)
        {
            // Check that both the user name and the password match
            string userName = GetUserName(credentials);
            string password = NameValuePair.FindNamedValue(credentials.Credentials,
                LoginRequest.PasswordCredential);
            bool isValid = !string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password);
            if (isValid)
            {
                isValid = SecurityHelpers.IsWildCardMatch(userName, this.userName) &&
                 string.Equals(password, this.password, StringComparison.InvariantCulture);
            }
            return isValid;
        }

        /// <summary>
        /// Retrieves the user name from the credentials.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <returns>The name of the user from the credentials. If the credentials not not exist in the system
        /// then null will be returned.</returns>
        public string GetUserName(LoginRequest credentials)
        {
            string userName = NameValuePair.FindNamedValue(credentials.Credentials,
                LoginRequest.UserNameCredential);
            return userName;
        }

        /// <summary>
        /// Retrieves the display name from the credentials.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <returns>The name of the user from the credentials. If the credentials do not exist in the system
        /// then null will be returned.</returns>
        public string GetDisplayName(LoginRequest credentials)
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
            password = newPassword;
        }
    }
}
