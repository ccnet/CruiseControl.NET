using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard
{
	public class Decorator : Page
	{
		protected HtmlGenericControl TopControlsLocation;
		protected HtmlGenericControl SideBarLocation;

		private void Page_Load(object sender, EventArgs e)
		{
			ObjectGiver objectGiver = CruiseObjectGiverFactory.CreateGiverForRequest(Request, Context, this);
			SideBarLocation.Controls.Add(((SideBarViewBuilder) objectGiver.GiveObjectByType(typeof(SideBarViewBuilder))).Execute());
			TopControlsLocation.Controls.Add(((TopControlsViewBuilder) objectGiver.GiveObjectByType(typeof(TopControlsViewBuilder))).Execute());
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
