using System;
using System.Web.UI;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard
{
	public class Controller : Page
	{
		protected System.Web.UI.HtmlControls.HtmlGenericControl ParentControl;

		private void Page_Load(object sender, EventArgs e)
		{
			RequestController controller = new RequestController(new CruiseActionFactory(new DashboardComponentFactory(Request, Context, this)));
			controller.Do(ParentControl, 
				new AggregatedRequest(
					new NameValueCollectionRequest(Request.Form),
					new NameValueCollectionRequest(Request.QueryString)
				));
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);
		}
		
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}
