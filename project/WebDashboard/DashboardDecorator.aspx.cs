using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using SiteMesh.DecoratorControls;

namespace ThoughtWorks.CruiseControl.WebDashboard
{
	public class DashboardDecorator : Page
	{
		protected HtmlTableCell contentCell;
		protected Title Title1;
		protected Body Body1;
		protected GetProperty prop1;
		protected Title Title3;

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
