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
        #region Private fields
        private readonly string cookieName;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new <see cref="CookieSessionStorer"/>.
        /// </summary>
        /// <param name="cookieName"></param>
        public CookieSessionStorer(string cookieName)
        {
            this.cookieName = cookieName;
        }
        #endregion

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
            if (!string.IsNullOrEmpty(SessionToken))
            {
                var newCookie = new HttpCookie(cookieName);
                newCookie.Value = SessionToken;
                newCookie.HttpOnly = true;
                newCookie.Expires = DateTime.Now.AddMinutes(15);
                HttpContext.Current.Response.Cookies.Add(newCookie);
            }
            return string.Empty;
        }
        #endregion
        #endregion
    }
}
