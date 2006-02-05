using System.IO;
using System.Web;
using Objection;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.ASPNET
{
	// No need for session state yet, but if we do later then we should also add IRequiresSessionState to list of interfaces
	public class HttpHandler : IHttpHandler
	{
		public void ProcessRequest(HttpContext context)
		{
			DoSecurityChecks(context);

			// ToDo - more on MimeTypes?
			if (context.Request.Path.EndsWith(".xml"))
			{
				// ToDo - if we are specifying XML, shouldn't we throw valid XML exceptions?
				context.Response.ContentType = "Text/XML";
			}

			ObjectSource objectSource =
				new CruiseObjectSourceInitializer(new ObjectionStore()).SetupObjectSourceForRequest(context);
			IResponse response = ((RequestController) objectSource.GetByType(typeof (RequestController))).Do();

			if (response is RedirectResponse)
			{
				context.Response.Redirect(((RedirectResponse) response).Url);
			}
			else
			{
				context.Response.Write(((HtmlFragmentResponse) response).ResponseFragment);
			}

			context.Response.Flush();
		}

		public bool IsReusable
		{
			get { return true; }
		}

		private void DoSecurityChecks(HttpContext context)
		{
			// Security Fix - see http://www.microsoft.com/security/incident/aspnet.mspx
			if (context.Request.Path.IndexOf('\\') >= 0 ||
			    Path.GetFullPath(context.Request.PhysicalPath) != context.Request.PhysicalPath)
			{
				throw new HttpException(404, "not found");
			}
		}
	}
}