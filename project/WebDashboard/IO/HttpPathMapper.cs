using System;
using System.Web;
using System.Web.UI;

namespace ThoughtWorks.CruiseControl.WebDashboard.IO
{
	/// <summary>
	/// Maps paths by using an Http components
	/// </summary>
	public class HttpPathMapper : IPathMapper
	{
		private readonly Control webControl;
		private readonly HttpContext context;

		public HttpPathMapper(HttpContext context, Control webControl)
		{
			this.context = context;
			this.webControl = webControl;
		}

		public string GetLocalPathFromURLPath(string originalPath)
		{
			return context.Server.MapPath(originalPath);
		}

		public string GetAbsoluteURLForRelativePath(string relativePath)
		{
			return string.Format("{0}://{1}{2}", context.Request.Url.Scheme, context.Request.Url.Host, webControl.ResolveUrl(relativePath));
		}
	}
}
