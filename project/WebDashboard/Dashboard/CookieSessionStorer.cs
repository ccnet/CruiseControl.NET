using System;
using System.Web;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
    /// <summary>
    /// Store the session token in a cookie.
    /// </summary>
    public class CookieSessionStorer
        : ISessionStorer
    {
        #region Public properties
        #region SessionToken
        /// <summary>
        /// The session token.
        /// </summary>
        public string SessionToken { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region RetrieveSessionToken()
        /// <summary>
        /// Generate a query value containing the session token.
        /// </summary>
        /// <returns></returns>
        public string GenerateQueryToken()
        {
            return string.Empty;
        }
        #endregion
        #endregion
    }
}
