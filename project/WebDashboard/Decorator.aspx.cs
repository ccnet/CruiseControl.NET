using System;
using System.Collections;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace ThoughtWorks.CruiseControl.webdashboard
{
	public class Decorator : System.Web.UI.Page
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
