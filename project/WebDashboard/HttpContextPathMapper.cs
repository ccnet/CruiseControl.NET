using System.Web;

namespace ThoughtWorks.CruiseControl.WebDashboard
{
	/// <summary>
	/// Maps paths by using an Http Context
	/// </summary>
	public class HttpContextPathMapper : IPathMapper
	{
		private readonly HttpContext context;

		public HttpContextPathMapper(HttpContext context)
		{
			this.context = context;
		}

		public string MapPath(string originalPath)
		{
			return context.Server.MapPath(originalPath);
		}
	}
}
