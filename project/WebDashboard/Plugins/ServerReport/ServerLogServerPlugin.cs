using System.Web.UI;
using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ServerReport
{
	public class ServerLogServerPlugin : ICruiseAction, IPluginLinkRenderer, IPlugin
	{
		private readonly IFarmService farmService;

		public ServerLogServerPlugin(IFarmService farmService)
		{
			this.farmService = farmService;
		}

		public Control Execute(ICruiseRequest request)
		{
			HtmlGenericControl control = new HtmlGenericControl("p");
			control.InnerHtml = string.Format(@"<pre class=""log"">{0}</pre>", farmService.GetServerLog(request.ServerSpecifier));
			return control;
		}

		public string LinkDescription
		{
			get { return "View Server Log"; }
		}

		public string LinkActionName
		{
			get { return "ViewServerLog"; }
		}

		public TypedAction[] Actions
		{
			get {  return new TypedAction[] { new TypedAction(LinkActionName, this.GetType()) }; }
		}
	}
}
