using System;
using System.Web.UI.HtmlControls;

namespace ThoughtWorks.CruiseControl.WebDashboard
{
	public class DashboardDecorator : System.Web.UI.Page
	{
		protected HtmlTableCell contentCell;
		protected SiteMesh.DecoratorControls.Title Title1;
		protected SiteMesh.DecoratorControls.Body Body1;
		protected SiteMesh.DecoratorControls.GetProperty prop1;
		protected SiteMesh.DecoratorControls.Title Title3;

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);
		}
		
		private void InitializeComponent()
		{    

		}
		#endregion
	}
}
