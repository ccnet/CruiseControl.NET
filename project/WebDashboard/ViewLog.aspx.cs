using System;
using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.LogViewerPlugin;

namespace ThoughtWorks.CruiseControl.WebDashboard
{
	public class ViewLog : System.Web.UI.Page
	{
		protected HtmlAnchor LogLink;

		private void Page_Load(object sender, System.EventArgs e)
		{
			LogViewer logViewer = new PluginFactory(new DashboardComponentFactory(Request, Context, this)).LogViewer;

			LogViewerResults results = logViewer.Do();
			LogLink.HRef = results.RedirectURL;
		}

		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);
		}
		
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
		}
	}
}
