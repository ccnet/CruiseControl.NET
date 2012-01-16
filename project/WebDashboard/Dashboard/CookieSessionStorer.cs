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
        /// Stores the session token in a cookie or deletes the cookie.
        /// </summary>
        public void StoreSessionToken(string sessionToken)
		{
			var newCookie = new HttpCookie("CCNetSessionToken");
			newCookie.HttpOnly = true;
			if (string.IsNullOrEmpty(sessionToken))
			{
				newCookie.Expires = DateTime.Now.AddDays(-1);
			}
			else
			{
				newCookie.Value = sessionToken;
				// A session cookie is created when no newCookie.Expires is set
			}
			HttpContext.Current.Response.Cookies.Add(newCookie);
		}
        #endregion
        #endregion
    }
}
