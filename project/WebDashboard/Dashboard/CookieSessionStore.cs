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
        #region Public methods
        #region RetrieveStorer()
        /// <summary>
        /// Retrieve the session storer.
        /// </summary>
        /// <returns>Returns an object that will store a session token.</returns>
        public ISessionStorer RetrieveStorer()
        {
            var storer = new CookieSessionStorer();
            return storer;
        }
        #endregion

        #region RetrieveRetriever()
        /// <summary>
        /// Retrieve the session retriever.
        /// </summary>
        /// <returns>Returns an object that will retrieve a session token.</returns>
        public ISessionRetriever RetrieveRetriever()
        {
            var retriever = new CookieSessionRetriever();
            return retriever;
        }
        #endregion
        #endregion
    }
}
