using System;
using System.Net;
using System.Web;
using System.Web.UI;

namespace ThoughtWorks.CruiseControl.WebDashboard.IO
{
	public class PathMapperUsingHostName : IPathMapper
	{
		private readonly HttpContext context;
		private readonly Control webControl;

		public PathMapperUsingHostName(HttpContext context, Control webControl)
		{
			this.webControl = webControl;
			this.context = context;
		}

		public string GetLocalPathFromURLPath(string originalPath)
		{
			return context.Server.MapPath(originalPath);
		}

		public string GetAbsoluteURLForRelativePath(string relativePath)
		{
			return string.Format("{0}://{1}:{2}{3}", context.Request.Url.Scheme, Dns.GetHostName(), context.Request.Url.Port, webControl.ResolveUrl(relativePath));
		}

		public string PhysicalApplicationPath
		{
			get { return context.Request.PhysicalApplicationPath; }
		}
	}
}
