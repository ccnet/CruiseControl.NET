using System.Collections;
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
		private readonly IHashtableTransformer velocityTransformer;

		public ServerLogServerPlugin(IFarmService farmService, IHashtableTransformer velocityTransformer)
		{
			this.farmService = farmService;
			this.velocityTransformer = velocityTransformer;
		}

		public Control Execute(ICruiseRequest request)
		{
			HtmlGenericControl control = new HtmlGenericControl("p");
			Hashtable viewObjects = new Hashtable();
			viewObjects["log"] = farmService.GetServerLog(request.ServerSpecifier);
			control.InnerHtml = velocityTransformer.Transform(viewObjects, @"templates\ServerLog.vm");    //string.Format(@"<pre class=""log"">{0}</pre>", );
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
