using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace ThoughtWorks.CruiseControl.Web
{
	public class Coverage : Page
	{
		protected Label results;
		protected HtmlGenericControl BodyArea;

		private void Page_Load(object sender, EventArgs e)
		{
			string xslFilename = WebUtil.GetXslFilename("Ncover.xsl", Request);
			BodyArea.InnerHtml = new PageTransformer(WebUtil.ResolveLogFile(Context), xslFilename).LoadPageContent();
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
