using System.Reflection;
using System.Web;
using System.Web.SessionState;
using Objection;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.ASPNET
{
	/// <summary>
	/// IRequiresSessionState used to be able to access session variables in the HttpContext.
	/// </summary>
	public class HttpHandler : IHttpHandler, IRequiresSessionState
	{
		private const string RESOLVED_TYPE_MAP = "ResolvedTypeMap";
		private const string CruiseObjectSourceInitializerName = "CruiseObjectSourceInitializer";

		public void ProcessRequest(HttpContext context)
		{
			CruiseObjectSourceInitializer sourceSetup = (CruiseObjectSourceInitializer)context.Session[CruiseObjectSourceInitializerName];
			ObjectSource objectSource = null;
			if (sourceSetup == null)
			{
				ObjectionStore objectionStore = new ObjectionStore(
					new CachingImplementationResolver(new NMockAwareImplementationResolver(), new CachedTypeMap(context.Cache, RESOLVED_TYPE_MAP)),
					new MaxLengthConstructorSelectionStrategy());
				sourceSetup = new CruiseObjectSourceInitializer(objectionStore);
				sourceSetup.SetupObjectSourceForFirstRequest(context);
				context.Session[CruiseObjectSourceInitializerName] = sourceSetup;
			}
			objectSource = sourceSetup.UpdateObjectSourceForRequest(context);

			context.Response.AppendHeader("X-CCNet-Version",
				string.Format(System.Globalization.CultureInfo.CurrentCulture, "CruiseControl.NET/{0}", Assembly.GetExecutingAssembly().GetName().Version));
			Assembly.GetExecutingAssembly().GetName().Version.ToString();

			IResponse response = ((RequestController)objectSource.GetByType(typeof(RequestController))).Do();
			response.Process(context.Response);
		}

		/// <summary>
		/// The Handler itself does not contain any member and can thus be reused.
		/// (at least for the moment being)
		/// </summary>
		public bool IsReusable
		{
			get { return true; }
		}
	}
}
