using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace ThoughtWorks.CruiseControl.Web
{
	public class FxCop : Page
	{
		protected HtmlGenericControl BodyArea;

		private void Page_Load(object sender, EventArgs e)
		{
			string xslFilename = WebUtil.GetXslFilename("FxCopReport.xsl", Request);
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
			this.Load += new EventHandler(this.Page_Load);

		}
		#endregion
	}
}
