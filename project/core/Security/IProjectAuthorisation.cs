using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    /// <summary>
    /// Checks the authorisation for a permission.
    /// </summary>
    public interface IProjectAuthorisation
    {
        /// <summary>
        /// Does this authorisation require a valid session?
        /// </summary>
        bool RequiresSession { get; }


        /// <summary>
        /// Checks whether the user can perform the specified action.
        /// </summary>
        /// <param name="userName">The name of the user that is being checked.</param>
        /// <param name="permission">The permission to check.</param>
        /// <returns>True if the permission is valid, false otherwise.</returns>
        bool CheckPermission(ISecurityManager manager, string userName, SecurityPermission permission);
    }
}
