using Exortech.NetReflector;
using System;
using System.Collections.Generic;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    /// <summary>
    /// A base class to provide basic caching functionality.
    /// </summary>
    public abstract class SessionCacheBase
        : ISessionCache
    {
        private readonly IClock clock;
        private readonly Dictionary<string, SessionDetails> cache = new Dictionary<string, SessionDetails>();
        private int durationInMinutes = 10;
        private SessionExpiryMode expiryMode = SessionExpiryMode.Sliding;

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionCacheBase" /> class.	
        /// </summary>
        /// <param name="clock">The clock.</param>
        /// <remarks></remarks>
        protected SessionCacheBase(IClock clock)
        {
            this.clock = clock;
        }

        /// <summary>
        /// The duration, in minutes, that a session is stored for. 
        /// </summary>
        /// <version>1.5</version>
        /// <default>10</default>
        [ReflectorProperty("duration", Required = false)]
        public virtual int Duration
        {
            get { return durationInMinutes; }
            set { durationInMinutes = value; }
        }

        /// <summary>
        /// The type of expiration period to use. Options are either Sliding (the expiry time is moved every time a security request is made)
        /// or Fixed (expiry time never changes).
        /// </summary>
        /// <version>1.5</version>
        /// <default>Sliding</default>
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
            SessionDetails session = new SessionDetails(userName, clock.Now.AddMinutes(durationInMinutes));
            AddToCacheInternal(sessionToken, session);
            return sessionToken;
        }

        /// <summary>
        /// Adds to cache internal.	
        /// </summary>
        /// <param name="sessionToken">The session token.</param>
        /// <param name="session">The session.</param>
        /// <remarks></remarks>
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
                if (clock.Now < session.ExpiryTime)
                {
                    if (expiryMode == SessionExpiryMode.Sliding)
                    {
                        lock (this)
                        {
                            cache[sessionToken].ExpiryTime = clock.Now.AddMinutes(durationInMinutes);
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

        /// <summary>
        /// 	
        /// </summary>
        protected class SessionDetails
        {
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            public string UserName;
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            public DateTime ExpiryTime;
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            public Dictionary<string, object> Values = new Dictionary<string, object>();

            /// <summary>
            /// Initializes a new instance of the <see cref="SessionDetails" /> class.	
            /// </summary>
            /// <param name="userName">Name of the user.</param>
            /// <param name="expiry">The expiry.</param>
            /// <remarks></remarks>
            public SessionDetails(string userName, DateTime expiry)
            {
                this.UserName = userName;
                this.ExpiryTime = expiry;
            }
        }
    }
}
