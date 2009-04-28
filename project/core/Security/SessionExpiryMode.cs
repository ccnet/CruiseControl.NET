using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    /// <summary>
    /// Defines the expirt mode to use for sessions.
    /// </summary>
    public enum SessionExpiryMode
    {
        /// <summary>
        /// When started the session's expiry time never changes.
        /// </summary>
        Fixed,
        /// <summary>
        /// The sessions expiry time increases as it is used.
        /// </summary>
        Sliding,
    }
}
