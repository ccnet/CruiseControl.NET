using System;
using System.Web;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
    /// <summary>
    /// Use a cookie as the store for the session token.
    /// </summary>
    [ReflectorType("cookieStore")]    
    public class CookieSessionStore
		: ISessionStore, ISessionRetriever, ISessionStorer
    {
        #region Public methods
        #region RetrieveStorer()
        /// <summary>
        /// Retrieve the session storer.
        /// </summary>
        /// <returns>Returns an object that will store a session token.</returns>
        public ISessionStorer RetrieveStorer()
        {
            return this;
        }
        #endregion

        #region RetrieveRetriever()
        /// <summary>
        /// Retrieve the session retriever.
        /// </summary>
        /// <returns>Returns an object that will retrieve a session token.</returns>
        public ISessionRetriever RetrieveRetriever()
        {
            return this;
        }
        #endregion

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
				_sessionToken = string.Empty;
				newCookie.Expires = DateTime.Now.AddDays(-1);
			}
			else
			{
				_sessionToken = sessionToken;
				newCookie.Value = sessionToken;
				// A session cookie is created when no newCookie.Expires is set
			}
			HttpContext.Current.Response.Cookies.Add(newCookie);
		}
		#endregion

		#region RetrieveSessionToken()
		/// <summary>
		/// Retrieve the session token.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public string RetrieveSessionToken(IRequest request)
		{
			if (_sessionToken == null)
			{
				var cookie = HttpContext.Current.Request.Cookies["CCNetSessionToken"];
				if (cookie != null)
				{
					_sessionToken = cookie.Value;
				}
				else
				{
					_sessionToken = string.Empty;
				}
			}
			return _sessionToken;
		}
		#endregion

		#endregion

		private string _sessionToken;
	}
}
