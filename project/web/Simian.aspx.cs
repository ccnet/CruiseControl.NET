using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace ThoughtWorks.CruiseControl.Web
{
	public class Simian : Page
	{
		protected ThoughtWorks.CruiseControl.Web.PluginLinks PluginLinks;
		protected HtmlGenericControl BodyArea;

		private void Page_Load(object sender, EventArgs e)
		{
			string xslFilename = WebUtil.GetXslFilename("SimianReport.xsl", Request);
			BodyArea.InnerHtml = new PageTransformer(WebUtil.ResolveLogFile(Context),xslFilename).LoadPageContent();
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
