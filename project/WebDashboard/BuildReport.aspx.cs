using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReporterPlugin;

namespace ThoughtWorks.CruiseControl.WebDashboard
{
	public class BuildReport : Page
	{
		protected HtmlTableCell HeaderCell;
		protected HtmlTableCell DetailsCell;
		protected HtmlGenericControl PluginLinks;
		protected HyperLink TestDetailsLink;
		protected HyperLink LogLink;
		protected HtmlGenericControl BodyLabel;
		protected HtmlGenericControl BodyArea;

		private void Page_Load(object sender, EventArgs e)
		{
			BuildReportResults results = new PluginPageRendererFactory(new DashboardComponentFactory(Request, Context, this)).BuildReporterPageRenderer.Do();
			BodyArea.InnerHtml = results.Html;
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
