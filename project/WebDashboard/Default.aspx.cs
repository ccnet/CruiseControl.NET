using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.WebDashboard
{
	public class Default : Page
	{
		protected HtmlGenericControl ParentControl;
		protected HtmlGenericControl TopControlsLocation;
		protected System.Web.UI.HtmlControls.HtmlForm Form1;
		protected HtmlGenericControl SideBarLocation;

		private void Page_Load(object sender, EventArgs e)
		{
			ObjectGiver objectGiver = new CruiseObjectGiverInitializer(new ObjectGiverAndRegistrar()).InitializeGiverForRequest(Request, Context, this);

			SideBarLocation.Controls.Add(((SideBarViewBuilder) objectGiver.GiveObjectByType(typeof(SideBarViewBuilder))).Execute().Control);
			TopControlsLocation.Controls.Add(((TopControlsViewBuilder) objectGiver.GiveObjectByType(typeof(TopControlsViewBuilder))).Execute().Control);
			((RequestController) objectGiver.GiveObjectByType(typeof(RequestController))).Do(ParentControl);
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
