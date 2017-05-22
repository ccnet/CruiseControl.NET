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
			HttpContext.Current.Session["CCNetSessionToken"] = sessionToken;
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
			return (String)HttpContext.Current.Session["CCNetSessionToken"];
		}
		#endregion

        #region DisplayName
        /// <summary>
        /// Stores the display name in a cookie or deletes the cookie.
        /// </summary>
        public void StoreDisplayName(string displayName)
        {
            HttpContext.Current.Session["CCNetDisplayName"] = displayName;
        }
        #endregion

        #region RetrieveDisplayName()
        /// <summary>
        /// Retrieve the display name.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string RetrieveDisplayName(IRequest request)
        {
            return (String)HttpContext.Current.Session["CCNetDisplayName"];
        }
        #endregion

		#endregion
	}
}
