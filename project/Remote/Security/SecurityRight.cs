using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.Remote.Security
{
    /// <summary>
    /// The security levels.
    /// </summary>
    public enum SecurityRight
    {
        /// <summary>
        /// The security right is allowed.
        /// </summary>
        Allow,
        /// <summary>
        /// The security right is denied.
        /// </summary>
        Deny,
        /// <summary>
        /// The security right will be inherited.
        /// </summary>
        Inherit
    }
}
