using System.Web.UI;
using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewServerLog
{
	public class ViewServerLogAction : ICruiseAction
	{
		public static readonly string ACTION_NAME = "ViewServerLog";

		private readonly IFarmService farmService;

		public ViewServerLogAction(IFarmService farmService)
		{
			this.farmService = farmService;
		}

		public Control Execute(ICruiseRequest request)
		{
			HtmlGenericControl control = new HtmlGenericControl("p");
			control.InnerHtml = string.Format(@"<pre class=""log"">{0}</pre>", farmService.GetServerLog(request.ServerSpecifier));
			return control;
		}
	}
}
