using System.Net;
using System.Web;

namespace ThoughtWorks.CruiseControl.WebDashboard.IO
{
	public class PathMapperUsingHostName : IPathMapper
	{
		private readonly HttpContext context;

		public PathMapperUsingHostName(HttpContext context)
		{
			this.context = context;
		}

		public string GetLocalPathFromURLPath(string originalPath)
		{
			return context.Server.MapPath(originalPath);
		}

		public string GetAbsoluteURLForRelativePath(string relativePath)
		{
			return string.Format("{0}://{1}:{2}{3}/{4}", context.Request.Url.Scheme, Dns.GetHostName(), context.Request.Url.Port, context.Request.ApplicationPath, relativePath);
		}

		public string PhysicalApplicationPath
		{
			get { return context.Request.PhysicalApplicationPath; }
		}
	}
}
