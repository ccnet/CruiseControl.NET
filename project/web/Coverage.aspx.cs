using System;
using System.Collections;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Util;

namespace ThoughtWorks.CruiseControl.Web
{
	public class Coverage : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label results;
		protected HtmlGenericControl BodyArea;

		private void Page_Load(object sender, System.EventArgs e)
		{
			BodyArea.InnerHtml = new PageTransformer(WebUtil.ResolveLogFile(Context),"Ncover.xsl").LoadPageContent();
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
