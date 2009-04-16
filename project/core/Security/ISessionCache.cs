using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    /// <summary>
    /// Defines a cache for holding session details.
    /// </summary>
    public interface ISessionCache
    {
        /// <summary>
        /// Initialises the cache.
        /// </summary>
        void Initialise();

        /// <summary>
        /// Adds a session to the cache and generates a session token.
        /// </summary>
        /// <param name="userName">The user name to add.</param>
        /// <returns>The session token.</returns>
        string AddToCache(string userName);

        /// <summary>
        /// Retrieves a user name from the cache based on a session token.
        /// </summary>
        /// <param name="sessionToken">The session token to use.</param>
        /// <returns>The user name if the token is valid, null otherwise.</returns>
        string RetrieveFromCache(string sessionToken);

        /// <summary>
        /// Removes a user name from the cache on a session token.
        /// </summary>
        /// <param name="sessionToken">The session token to use.</param>
        void RemoveFromCache(string sessionToken);

        /// <summary>
        /// Stores a value for a session.
        /// </summary>
        /// <param name="sessionToken">The session token to use.</param>
        /// <param name="key">The key of the value.</param>
        /// <param name="value">The value to store.</param>
        void StoreSessionValue(string sessionToken, string key, object value);

        /// <summary>
        /// Retrieves a value from a session.
        /// </summary>
        /// <param name="sessionToken">The session token to use.</param>
        /// <param name="key">The key of the value.</param>
        /// <returns>The value if available, null otherwise.</returns>
        object RetrieveSessionValue(string sessionToken, string key);
    }
}
