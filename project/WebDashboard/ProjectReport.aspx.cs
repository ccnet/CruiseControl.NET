using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Util;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReporterPlugin;

namespace ThoughtWorks.CruiseControl.WebDashboard
{
	public class ProjectReport : Page
	{
		protected HtmlTableCell HeaderCell;
		protected HtmlTableCell DetailsCell;
		protected HtmlGenericControl PluginLinks;
		protected HyperLink TestDetailsLink;
		protected HyperLink LogLink;
		protected HtmlGenericControl BodyLabel;

		private void Page_Load(object sender, EventArgs e)
		{
			try
			{
				ProjectReportResults results = new PluginFactory(new DashboardComponentFactory(Request, Context, this)).ProjectReporter.Do();
				HeaderCell.InnerHtml = results.HeaderCellHtml;
				DetailsCell.InnerHtml = results.DetailsCellHtml;
				PluginLinks.InnerHtml = results.PluginLinksHtml;
			}
			// ToDo - Generic Error page?
			catch (CruiseControlException ex)
			{
				// This fixes a problem where the BodyLabel control isn't initialised, causing
				// a NullReferenceException.  The original exception (ex) was being lost.
				// Why is BodyLabel null?  (drewnoakes: I saw this problem while working with
				// invalid Xsl in file modifications.xsl)
				if (BodyLabel==null)
					throw new CruiseControlException("Unable to render page.", ex);

				if (BodyLabel.InnerText==null)
					BodyLabel.InnerText = string.Empty;

				BodyLabel.InnerText += new HtmlExceptionFormatter(ex).ToString();
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
			this.Load += new EventHandler(this.Page_Load);

		}
		#endregion
	}
}
