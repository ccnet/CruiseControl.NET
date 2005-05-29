using System.Web;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

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
	}
}
