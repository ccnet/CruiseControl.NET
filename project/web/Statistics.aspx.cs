using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.Web
{
	public class Statistics : Page
	{
		protected Label BodyLabel;
	
		private void Page_Load(object sender, EventArgs e)
		{
			try
			{
				InitStatistics(WebUtil.GetLogDirectory(Context).FullName);
			}
			catch(CruiseControlException ex)
			{
				Response.Write(ex.Message);
			}
		}

		private void InitStatistics(string path)
		{
			LogStatistics logStats = LogStatistics.Create(path);
			BodyLabel.Text += (String.Format("<p>Total Successful Builds: {0}</p>", logStats.GetTotalSuccessfulBuilds()));
			BodyLabel.Text += (String.Format("<p>Total Failed Builds: {0}</p>", logStats.GetTotalFailedBuilds()));
			BodyLabel.Text += (String.Format("<p>Success Ratio: {0}</p>", logStats.GetSuccessRatio()));
			BodyLabel.Text += (String.Format("<p>Latest Build Status: {0}</p>", 
				(logStats.IsLatestBuildSuccessful() ? "Successful" : "Failed") ));
			BodyLabel.Text += (String.Format("<p>Time Since Latest Build: {0}</p>", logStats.GetTimeSinceLatestBuildString()));
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
