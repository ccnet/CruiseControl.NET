using System;
using System.Collections;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using tw.ccnet.core;

namespace tw.ccnet.web
{
	public class Statistics : System.Web.UI.Page
	{
		protected HtmlGenericControl statistics;

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				string path = ConfigurationSettings.AppSettings["logDir"];
				InitStatistics(path);
			}
			catch(CruiseControlException ex)
			{
				statistics.InnerHtml += ex.Message;
			}
		}

		private void InitStatistics(string path)
		{
			LogStatistics logStats = LogStatistics.Create(path);
			statistics.InnerHtml += String.Format("<p>Total Successful Builds: {0}</p>", logStats.GetTotalSuccessfulBuilds());
			statistics.InnerHtml += String.Format("<p>Total Failed Builds: {0}</p>", logStats.GetTotalFailedBuilds());
			statistics.InnerHtml += String.Format("<p>Success Ratio: {0}</p>", logStats.GetSuccessRatio());
			statistics.InnerHtml += String.Format("<p>Latest Build Status: {0}</p>", 
				(logStats.IsLatestBuildSuccessful() ? "Successful" : "Failed") );
			statistics.InnerHtml += String.Format("<p>Time Since Latest Build: {0}</p>", logStats.GetTimeSinceLatestBuildString());
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
