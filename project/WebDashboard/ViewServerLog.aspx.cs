using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ServerLogViewerPlugin;

namespace ThoughtWorks.CruiseControl.WebDashboard
{
	public class ViewServerLog : Page
	{
		protected HtmlGenericControl BodyArea;

		private void Page_Load(object sender, EventArgs e)
		{
			ServerLogViewerResults results = new PluginPageRendererFactory(new DashboardComponentFactory(Request, Context, this)).ServerLogViewerPageRenderer.Do();
			BodyArea.InnerHtml = results.LogHtml;
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);
		}
		
		private void InitializeComponent()
		{    
			this.Load += new EventHandler(this.Page_Load);

		}
		#endregion
	}
}
