using System.Web;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
    /// <summary>
    /// Retrieve a session token from a cookie.
    /// </summary>
    public class CookieSessionRetriever
        : ISessionRetriever
    {
        #region Private fields
        private readonly string cookieName;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new <see cref="CookieSessionRetriever"/>.
        /// </summary>
        /// <param name="cookieName"></param>
        public CookieSessionRetriever(string cookieName)
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
        /// Retrieve the session token.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string RetrieveSessionToken(IRequest request)
        {
            var cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie != null)
            {
                SessionToken = cookie.Value;
            }
            else
            {
                SessionToken = string.Empty;
            }
            return SessionToken;
        }
        #endregion
        #endregion
    }
}
