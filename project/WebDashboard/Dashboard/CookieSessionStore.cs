using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
    /// <summary>
    /// Use a cookie as the store for the session token.
    /// </summary>
    [ReflectorType("cookieStore")]    
    public class CookieSessionStore
        : ISessionStore
    {
        #region Constructors
        /// <summary>
        /// Initialise a new <see cref="CookieSessionStore"/>.
        /// </summary>
        public CookieSessionStore()
        {
            CookieName = "CCNetSessionToken";
        }
        #endregion

        #region Public properties
        #region CookieName
        /// <summary>
        /// The name of the cookie.
        /// </summary>
        [ReflectorProperty("name", Required = false)]
        public string CookieName { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region RetrieveStorer()
        /// <summary>
        /// Retrieve the session storer.
        /// </summary>
        /// <returns>Returns an object that will store a session token.</returns>
        public ISessionStorer RetrieveStorer()
        {
            return new CookieSessionStorer(CookieName);
        }
        #endregion

        #region RetrieveStorer()
        /// <summary>
        /// Retrieve the session retriever.
        /// </summary>
        /// <returns>Returns an object that will retrieve a session token.</returns>
        public ISessionRetriever RetrieveRetriever()
        {
            return new CookieSessionRetriever(CookieName);
        }
        #endregion
        #endregion
    }
}
