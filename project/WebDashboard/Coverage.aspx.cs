using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard
{
	public class Coverage : Page
	{
		protected Label results;
		protected HtmlGenericControl BodyArea;

		private void Page_Load(object sender, EventArgs e)
		{
			WebUtil webUtil = WebUtil.Create(Request, Context, this);
			string xslFilename = webUtil.GetXslFilename("Ncover.xsl");
			BodyArea.InnerHtml = new PageTransformer(webUtil.GetLogFileAndCheckItExists(), xslFilename).LoadPageContent();
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
