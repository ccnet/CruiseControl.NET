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
using System.Xml;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Util;

namespace ThoughtWorks.CruiseControl.Web
{
	public class FxCop : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label results;
		protected HtmlGenericControl BodyArea;

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				string logfile = WebUtil.ResolveLogFile(Context);
				BodyArea.InnerHtml = LogFileLister.Transform(logfile, WebUtil.GetXslFilename("FxCopReport.xsl", Request));
			}
			catch(CruiseControlException ex)
			{
				BodyArea.InnerHtml += new HtmlExceptionFormatter(ex).ToString();
			}
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
