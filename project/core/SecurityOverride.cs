namespace ThoughtWorks.CruiseControl.Core
{
    using System;

    /// <summary>
    /// Internal class for overriding the security if required.
    /// </summary>
    internal static class SecurityOverride
    {
        /// <summary>
        /// Initializes the <see cref="SecurityOverride"/> class.
        /// </summary>
        static SecurityOverride()
        {
            SessionIdentifier = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Gets the session identifier.
        /// </summary>
        /// <value>The session identifier.</value>
        internal static string SessionIdentifier { get; private set; }
    }
}
