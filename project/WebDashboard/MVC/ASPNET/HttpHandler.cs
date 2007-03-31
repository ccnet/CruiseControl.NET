using System.Collections;
using System.IO;
using System.Web;
using Objection;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.ASPNET
{
	// No need for session state yet, but if we do later then we should also add IRequiresSessionState to list of interfaces
	public class HttpHandler : IHttpHandler
	{
		private const string RESOLVED_TYPE_MAP = "ResolvedTypeMap";
		
		public void ProcessRequest(HttpContext context)
		{
			// ToDo - more on MimeTypes?
			if (context.Request.Path.EndsWith(".xml"))
			{
				// ToDo - if we are specifying XML, shouldn't we throw valid XML exceptions?
				context.Response.ContentType = "Text/XML";
			}
			else
			{
				string[] splits = context.Request.Path.Split('.');
				if (MimeType.Jpg.HasExtension(splits[splits.Length - 1]))
				{
					context.Response.ContentType = MimeType.Jpg.ContentType;
				}
			}

			ObjectionStore objectionStore = new ObjectionStore(
				new CachingImplementationResolver(new NMockAwareImplementationResolver(), new CachedTypeMap(context.Cache, RESOLVED_TYPE_MAP)), new MaxLengthConstructorSelectionStrategy());
			ObjectSource objectSource = new CruiseObjectSourceInitializer(objectionStore).SetupObjectSourceForRequest(context);
			IResponse response = ((RequestController) objectSource.GetByType(typeof (RequestController))).Do();

			if (response is RedirectResponse)
			{
				context.Response.Redirect(((RedirectResponse) response).Url);
			}
			else if (response is BinaryResponse )
			{
				context.Response.BinaryWrite(((BinaryResponse)response).Content);
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
	}

	internal class MimeType
	{
		//TODO: read from config file?
		public static readonly MimeType Jpg = new MimeType("image/jpeg", "jpg", "jpe");
		public static readonly MimeType Png = new MimeType("image/png", "png");
		public static readonly MimeType Xml = new MimeType("text/xml", "xml");

		private ArrayList mimeExtension;
		private string mimeType;

		public MimeType(string mimeType, params string[] extensions)
		{
			mimeExtension = new ArrayList();
			mimeExtension.AddRange(extensions);
			this.mimeType = mimeType;
		}

		public bool HasExtension(string extension)
		{
			return mimeExtension.Contains(extension);
		}

		public string ContentType
		{
			get { return mimeType; }
		}
	}
}
