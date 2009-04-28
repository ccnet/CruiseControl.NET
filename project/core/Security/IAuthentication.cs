using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Security;


namespace ThoughtWorks.CruiseControl.Core.Security
{
    /// <summary>
    /// Defines an authentication mechanism.
    /// </summary>
    public interface IAuthentication
        : ISecuritySetting
    {
        /// <summary>
        /// Attempts to authenticate a user from the credentials.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <returns>True if the credentials are valid, false otherwise..</returns>
        bool Authenticate(ISecurityCredentials credentials);

        /// <summary>
        /// Retrieves the user name from the credentials.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <returns>The display name of the user from the credentials. If the credentials do not exist in the system
        /// then null will be returned.</returns>
        string GetUserName(ISecurityCredentials credentials);

        /// <summary>
        /// Retrieves the display name from the credentials.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <returns>The name of the user from the credentials. If the credentials do not exist in the system
        /// then null will be returned.</returns>
        string GetDisplayName(ISecurityCredentials credentials);

        /// <summary>
        /// The user name from the configuration.
        /// </summary>
        string UserName { get; }

        /// <summary>
        /// The display name from the configuration.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// The name of the authentication type.
        /// </summary>
        string AuthenticationName { get; }

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="newPassword"></param>
        void ChangePassword(string newPassword);
    }
}
