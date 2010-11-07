using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    /// <summary>
    /// Checks the authorisation for a permission.
    /// </summary>
    /// <title>Project Level Security</title>
    public interface IProjectAuthorisation
    {
        /// <summary>
        /// Does this authorisation require security to be configured on the server?
        /// </summary>
        bool RequiresServerSecurity { get; }

        /// <summary>
        /// The name of the account to use for guests.
        /// </summary>
        string GuestAccountName { get; }

        /// <summary>
        /// Does this authorisation require a valid session?
        /// </summary>
        bool RequiresSession(ISecurityManager manager);

        /// <summary>
        /// Checks whether the user can perform the specified action.
        /// </summary>
        /// <param name="userName">The name of the user that is being checked.</param>
        /// <param name="permission">The permission to check.</param>
        /// <param name="defaultRight">The default right to use.</param>
        /// <returns>True if the permission is valid, false otherwise.</returns>
        /// <param name="manager"></param>
        bool CheckPermission(ISecurityManager manager, 
            string userName, 
            SecurityPermission permission,
            SecurityRight defaultRight);
    }
}
