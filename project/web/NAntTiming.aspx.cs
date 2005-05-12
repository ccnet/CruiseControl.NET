using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace ThoughtWorks.CruiseControl.Web
{
	public class NAntTiming : Page
	{
		protected PluginLinks PluginLinks;
		protected HtmlGenericControl Content;

		private void Page_Load(object sender, EventArgs e)
		{
			TransformContent("NAntTiming.xsl");
		}

		private void TransformContent(string stylesheet)
		{
			string xslFilename = WebUtil.GetXslFilename(stylesheet, Request);
			Content.InnerHtml = new PageTransformer(WebUtil.ResolveLogFile(Context), xslFilename).LoadPageContent();
		}

		#region Web Form Designer generated code

		protected override void OnInit(EventArgs e)
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