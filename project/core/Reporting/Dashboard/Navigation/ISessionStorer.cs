using System;

namespace ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation
{
    /// <summary>
    /// Interface for storing sessions.
    /// </summary>
    public interface ISessionStorer
    {
        /// <summary>
        /// Generates a token to add to a query string for a session.
        /// </summary>
        /// <param name="sessionToken"></param>
        /// <returns></returns>
        string GenerateQueryToken();

        /// <summary>
        /// The session token to store.
        /// </summary>
        string SessionToken { get; set; }
    }
}
