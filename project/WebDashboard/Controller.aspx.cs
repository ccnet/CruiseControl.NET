using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.WebDashboard
{
	public class Controller : Page
	{
		protected HtmlGenericControl ParentControl;

		private void Page_Load(object sender, EventArgs e)
		{
			ObjectGiver objectGiver = CruiseObjectGiverFactory.CreateGiverForRequest(Request, Context, this);
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
			this.Load += new EventHandler(this.Page_Load);
		}
		#endregion
	}
}
