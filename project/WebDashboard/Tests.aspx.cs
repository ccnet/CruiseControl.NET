using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard
{
	public class Tests : Page
	{
		protected Label BodyLabel;
		protected HtmlGenericControl BodyArea;

		private void Page_Load(object sender, EventArgs e)
		{
			WebUtil webUtil = WebUtil.Create(Request, Context);
			string xslFilename = webUtil.GetXslFilename("tests.xsl");
			BodyArea.InnerHtml = new PageTransformer(webUtil.GetLogFileAndCheckItExists(),xslFilename).LoadPageContent();
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
