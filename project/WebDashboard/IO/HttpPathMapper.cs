using System.IO;
using System.Web;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.WebDashboard.IO
{
	/// <summary>
	/// Maps paths by using an Http components
	/// </summary>
	public class HttpPathMapper : IPhysicalApplicationPathProvider
	{
		private readonly HttpContext context;

		public HttpPathMapper(HttpContext context)
		{
			this.context = context;
		}

	    public string GetFullPathFor(string appRelativePath)
	    {
	        return Path.Combine(context.Request.PhysicalApplicationPath, appRelativePath);
	    }
	}
}