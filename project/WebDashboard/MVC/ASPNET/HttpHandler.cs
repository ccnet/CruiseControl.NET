using System.Collections;
using System.Web;
using Objection;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
}

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.ASPNET
{
	// No need for session state yet, but if we do later then we should also add IRequiresSessionState to list of interfaces
	public class HttpHandler : IHttpHandler
	{
		private const string RESOLVED_TYPE_MAP = "ResolvedTypeMap";
		
		public void ProcessRequest(HttpContext context)
		{
			ObjectionStore objectionStore = new ObjectionStore(
				new CachingImplementationResolver(new NMockAwareImplementationResolver(), new CachedTypeMap(context.Cache, RESOLVED_TYPE_MAP)), new MaxLengthConstructorSelectionStrategy());
			ObjectSource objectSource = new CruiseObjectSourceInitializer(objectionStore).SetupObjectSourceForRequest(context);

			IResponse response = ((RequestController) objectSource.GetByType(typeof (RequestController))).Do();
			response.Process(context.Response);
			context.Response.Flush();
		}

		public bool IsReusable
		{
			get { return true; }
		}
	}
}
