using Exortech.NetReflector;
using System;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    /// <summary>
    /// <para>
    /// User password authentication checks that the user name and password combination is valid.
    /// </para>
    /// </summary>
    /// <title>User Password Authentication</title>
    /// <version>1.5</version>
    /// <example>
    /// <code title="Simple example">
    /// &lt;passwordUser name="johndoe" password="whoareyou" display="John Doe" /&gt;
    /// </code>
    /// <para>
    /// The following example shows how this user definition can be used with an internal security definition.
    /// </para>
    /// <code title="Example in Context">
    /// &lt;internalSecurity&gt;
    /// &lt;users&gt;
    /// &lt;passwordUser name="johndoe" password="whoareyou" display="John Doe"/&gt;
    /// &lt;/users&gt;
    /// &lt;permissions&gt;
    /// &lt;!-- Omitted for brevity --&gt;
    /// &lt;/permissions&gt;
    /// &lt;/internalSecurity&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <para>
    /// This element simply stores a user name and password - authentication is that the password is valid for the user name.
    /// </para>
    /// <para>
    /// It is possible to use wildcards in this element - see <link>Wildcards in User Names</link>.
    /// </para>
    /// </remarks>
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
        /// <param name="password"></param>
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
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("name")]
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        /// <summary>
        /// The password for this user.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
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
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// If this is not set, the name will be used as the display name.
        /// </remarks>
        [ReflectorProperty("display", Required = false)]
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
