using System.Web;

namespace ThoughtWorks.CruiseControl.WebDashboard.IO
{
	/// <summary>
	/// Maps paths by using an Http components
	/// </summary>
	public class HttpPathMapper : IPathMapper
	{
		private readonly HttpContext context;

		public HttpPathMapper(HttpContext context)
		{
			this.context = context;
		}

		public string GetLocalPathFromURLPath(string originalPath)
		{
			return context.Server.MapPath(originalPath);
		}

		public string PhysicalApplicationPath
		{
			get { return context.Request.PhysicalApplicationPath; }
		}

		public string GetAbsoluteURLForRelativePath(string relativePath)
		{
			return string.Format("{0}://{1}:{2}{3}/{4}", context.Request.Url.Scheme, context.Request.Url.Host, context.Request.Url.Port, context.Request.ApplicationPath, relativePath);
		}
	}
}
