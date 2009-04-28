using Exortech.NetReflector;
using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    /// <summary>
    /// A base class to provide basic caching functionality.
    /// </summary>
    public abstract class SessionCacheBase
        : ISessionCache
    {
        private Dictionary<string, SessionDetails> cache = new Dictionary<string, SessionDetails>();
        private int durationInMinutes = 10;
        private SessionExpiryMode expiryMode = SessionExpiryMode.Sliding;

        /// <summary>
        /// How long a session is valid before it expires.
        /// </summary>
        [ReflectorProperty("duration", Required = false)]
        public virtual int Duration
        {
            get { return durationInMinutes; }
            set { durationInMinutes = value; }
        }

        /// <summary>
        /// The type of expiry mode to use.
        /// </summary>
        [ReflectorProperty("mode", Required = false)]
        public virtual SessionExpiryMode ExpiryMode
        {
            get { return expiryMode; }
            set { expiryMode = value; }
        }

        /// <summary>
        /// Initialises the cache.
        /// </summary>
        public virtual void Initialise()
        {
        }

        /// <summary>
        /// Adds a session to the cache and generates a session token.
        /// </summary>
        /// <param name="userName">The user name to add.</param>
        /// <returns>The session token.</returns>
        public virtual string AddToCache(string userName)
        {
            string sessionToken = Guid.NewGuid().ToString();
            SessionDetails session = new SessionDetails(userName, DateTime.Now.AddMinutes(durationInMinutes));
            AddToCacheInternal(sessionToken, session);
            return sessionToken;
        }

        protected virtual void AddToCacheInternal(string sessionToken, SessionDetails session)
        {
            lock (this)
            {
                cache.Add(sessionToken, session);
            }
        }

        /// <summary>
        /// Retrieves a user name from the cache based on a session token.
        /// </summary>
        /// <param name="sessionToken">The session token to use.</param>
        /// <returns>The user name if the token is valid, null otherwise.</returns>
        public virtual string RetrieveFromCache(string sessionToken)
        {
            SessionDetails details = RetrieveSessionDetails(sessionToken);
            if (details == null)
            {
                return null;
            }
            else
            {
                return details.UserName;
            }
        }

        /// <summary>
        /// Removes a user name from the cache on a session token.
        /// </summary>
        /// <param name="sessionToken">The session token to use.</param>
        public virtual void RemoveFromCache(string sessionToken)
        {
            if (cache.ContainsKey(sessionToken))
            {
                lock (this)
                {
                    cache.Remove(sessionToken);
                }
            }
        }

        /// <summary>
        /// Stores a value for a session.
        /// </summary>
        /// <param name="sessionToken">The session token to use.</param>
        /// <param name="key">The key of the value.</param>
        /// <param name="value">The value to store.</param>
        public virtual void StoreSessionValue(string sessionToken, string key, object value)
        {
            SessionDetails details = RetrieveSessionDetails(sessionToken);
            if (details != null)
            {
                lock (this)
                {
                    details.Values[key] = value;
                }
            }
        }

        /// <summary>
        /// Retrieves a value from a session.
        /// </summary>
        /// <param name="sessionToken">The session token to use.</param>
        /// <param name="key">The key of the value.</param>
        /// <returns>The value if available, null otherwise.</returns>
        public virtual object RetrieveSessionValue(string sessionToken, string key)
        {
            object value = null;
            SessionDetails details = RetrieveSessionDetails(sessionToken);
            if (details != null)
            {
                if (cache[sessionToken].Values.ContainsKey(key)) value = cache[sessionToken].Values[key];
            }
            return value;
        }

        /// <summary>
        /// Retrieves the session details based on a token.
        /// </summary>
        /// <param name="sessionToken">The session token to use.</param>
        /// <returns>The details, of valid, null otherwise.</returns>
        protected virtual SessionDetails RetrieveSessionDetails(string sessionToken)
        {
            if (cache.ContainsKey(sessionToken))
            {
                SessionDetails session = cache[sessionToken];
                if (DateTime.Now < session.ExpiryTime)
                {
                    if (expiryMode == SessionExpiryMode.Sliding)
                    {
                        lock (this)
                        {
                            cache[sessionToken].ExpiryTime = DateTime.Now.AddMinutes(durationInMinutes);
                        }
                    }
                    return session;
                }
                else
                {
                    RemoveFromCache(sessionToken);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        protected class SessionDetails
        {
            public string UserName;
            public DateTime ExpiryTime;
            public Dictionary<string, object> Values = new Dictionary<string, object>();

            public SessionDetails(string userName, DateTime expiry)
            {
                this.UserName = userName;
                this.ExpiryTime = expiry;
            }
        }
    }
}
