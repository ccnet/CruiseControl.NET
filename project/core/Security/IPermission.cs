using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    /// <summary>
    /// A security assertion.
    /// </summary>
    /// <title>Security Permissions</title>
    public interface IPermission
        : ISecuritySetting
    {
        /// <summary>
        /// Checks if the user should use this assertion.
        /// </summary>
        /// <param name="userName">The name of the user that is being checked.</param>
        /// <param name="manager"></param>
        /// <returns>True if the assertion is valid for the user, false otherwise.</returns>
        bool CheckUser(ISecurityManager manager, string userName);

        /// <summary>
        /// Checks the result of this assertion.
        /// </summary>
        /// <param name="permission">The permission to check.</param>
        /// <param name="manager"></param>
        /// <returns>The security right.</returns>
        SecurityRight CheckPermission(ISecurityManager manager, SecurityPermission permission);
    }
}
