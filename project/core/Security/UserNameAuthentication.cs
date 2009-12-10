using Exortech.NetReflector;
using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    /// <summary>
    /// <para>
    /// User name authentication checks that the user name is valid.
    /// </para>
    /// </summary>
    /// <title>User Name Authentication</title>
    /// <version>1.5</version>
    /// <example>
    /// <code title="Simple example">
    /// &lt;simpleUser name="johndoe" display="John Doe" /&gt;
    /// </code>
    /// <code title="Wildcard example">
    /// &lt;simpleUser name="*" /&gt;
    /// </code>
    /// <para>
    /// The following example shows how this user definition can be used with an internal security definition.
    /// </para>
    /// <code title="Example in Context">
    /// &lt;internalSecurity&gt;
    /// &lt;users&gt;
    /// &lt;simpleUser name="johndoe" display="John Doe"/&gt;
    /// &lt;/users&gt;
    /// &lt;permissions&gt;
    /// &lt;!-- Omitted for brevity --&gt;
    /// &lt;/permissions&gt;
    /// &lt;/internalSecurity&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <para>
    /// This element simply stores a user name - authentication is that the user name is a valid name.
    /// </para>
    /// <para>
    /// It is possible to use wildcards in this element - see <link>Wildcards in User Names</link>.
    /// </para>
    /// </remarks>
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
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("name")]
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        /// <summary>
        /// The display name for this user.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// If this element is not set, the name will be used for the display name.
        /// </remarks>
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
        public bool Authenticate(LoginRequest credentials)
        {
            // Check that the user name matches
            string userName = GetUserName(credentials);
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
            // Do nothing since this authentication does not have a password
        }
    }
}
