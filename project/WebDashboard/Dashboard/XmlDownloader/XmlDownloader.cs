using System;
using System.Web;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.WebDashboard.Config;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard.XmlDownloader
{
	// ToDo - this is untested. We are going to make the main MVC controller an HttpHandler too, and at that point 
	// this can become just another action (albeit one without UI decorators and setting a Content Type)
	public class XmlDownloader : IHttpHandler
	{
		public void ProcessRequest(HttpContext context)
		{
			ObjectGiver objectGiver = CreateObjectGiver(context);

			ICruiseRequest cruiseRequest = (ICruiseRequest) objectGiver.GiveObjectByType(typeof(ICruiseRequest));
			if (cruiseRequest.ServerName == "" || cruiseRequest.ProjectName == "" || cruiseRequest.BuildName == "")
			{
				throw new Exception("All of Server, Project and Build Names must be specified on request in order to retrieve a build log");
			}

			string log = ((IBuildRetriever) objectGiver.GiveObjectByType(typeof(IBuildRetriever))).GetBuild(cruiseRequest.BuildSpecifier).Log;

			context.Response.ContentType = "Text/XML";

			// None of this seems to have an effect - doh!
//			context.Response.Cache.SetExpires(DateTime.Now.AddDays(1));
//			context.Response.Cache.SetCacheability(HttpCacheability.Public);
//			context.Response.Cache.VaryByParams[RequestWrappingCruiseRequest.ServerQueryStringParameter] = true;
//			context.Response.Cache.VaryByParams[RequestWrappingCruiseRequest.ProjectQueryStringParameter] = true;
//			context.Response.Cache.VaryByParams[RequestWrappingCruiseRequest.BuildQueryStringParameter] = true;

			context.Response.Write(log);
			context.Response.Flush();
		}

		public bool IsReusable
		{
			get { return true; }
		}

		private ObjectGiver CreateObjectGiver(HttpContext context)
		{
			// ToDo - merge this with the main request controller setup
			ObjectGiverAndRegistrar giverAndRegistrar = new ObjectGiverAndRegistrar();
			HttpRequest request = context.Request;
			giverAndRegistrar.AddTypedObject(typeof(HttpRequest), request);
			giverAndRegistrar.AddTypedObject(typeof(HttpContext), context);
			giverAndRegistrar.AddTypedObject(typeof(ObjectGiver), giverAndRegistrar);

			// Add functionality to object giver to handle this?
			giverAndRegistrar.AddTypedObject(typeof(IRequest), new AggregatedRequest(new NameValueCollectionRequest(request.Form), new NameValueCollectionRequest(request.QueryString)));
			
			giverAndRegistrar.SetImplementationType(typeof(IPathMapper), typeof(HttpPathMapper));

			giverAndRegistrar.SetDependencyImplementationForType(typeof(PathMappingMultiTransformer), typeof(IMultiTransformer), typeof (HtmlAwareMultiTransformer));

			IConfigurationGetter configurationGetter = (IConfigurationGetter) giverAndRegistrar.GiveObjectByType(typeof(IConfigurationGetter));
			if (configurationGetter == null)
			{
				throw new CruiseControlException("Unable to instantiate configuration getter");
			}
			
			return giverAndRegistrar;
		}
	}
}
