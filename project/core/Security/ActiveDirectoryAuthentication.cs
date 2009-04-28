using System.DirectoryServices;
using System.Runtime.InteropServices;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    /// <summary>
    /// Stores a user name - authentication will come from Active Directory.
    /// </summary>
    [ReflectorType("ldapUser")]
    public class ActiveDirectoryAuthentication
        : IAuthentication
    {
        private const string userNameCredential = "username";

        private string userName;
        private string domainName;
        private ISecurityManager manager;

        /// <summary>
        /// Start a new blank authentication.
        /// </summary>
        public ActiveDirectoryAuthentication() { }

        /// <summary>
        /// Start a new authentication with a user name.
        /// </summary>
        /// <param name="userName"></param>
        public ActiveDirectoryAuthentication(string userName)
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
        /// The user name for this user.
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
        public string DisplayName
        {
            get { return null; }
        }

        /// <summary>
        /// The name of the authentication type.
        /// </summary>
        public string AuthenticationName
        {
            get { return "LDAP"; }
        }

        /// <summary>
        /// The AD domain to use.
        /// </summary>
        [ReflectorProperty("domain")]
        public string DomainName
        {
            get { return domainName; }
            set { domainName = value; }
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
            if (isValid)
            {
                string displayName = FindUser(userName);
                isValid = (displayName != null);
            }
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
            string userName = GetUserName(credentials);
            string nameToReturn = FindUser(userName);
            if (string.IsNullOrEmpty(nameToReturn)) nameToReturn = userName;
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

        /// <summary>
        /// Attempts to find a user.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        private string FindUser(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return null;

            // Setup the domain connection
            DirectoryEntry domain = new DirectoryEntry("LDAP://OU=Domain,DC=" + domainName);
            domain.AuthenticationType = AuthenticationTypes.Secure;

            // Attempt to find the user
            DirectorySearcher searcher = new DirectorySearcher(domain);
            searcher.Filter = "(SAMAccountName=" + userName + ")";
            searcher.PropertiesToLoad.Add("displayName");
            try
            {
                SearchResult result = searcher.FindOne();

                // Check the result
                if (result != null)
                {
                    return result.Properties["displayname"][0].ToString();
                }
                else
                {
                    return null;
                }
            }
            catch (COMException)
            {
                return null;
            }
        }
    }
}
