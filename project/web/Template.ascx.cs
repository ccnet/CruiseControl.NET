namespace tw.ccnet.web
{
	using System;
	using System.Web.UI;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;
	using System.Configuration;
	using System.Text;
	using tw.ccnet.core;

	public abstract class Template : UserControl
	{
		protected HtmlTableCell contentCell;
		protected DataList menu;
		protected HtmlGenericControl buildStats;
		protected HtmlAnchor nextLog;
		protected HtmlAnchor previousLog;
		protected HtmlAnchor testTiming;

		public Control Content
		{
			set 
			{ 
				EnsureChildControls();
				contentCell.Controls.Add(value); 
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			string path = WebUtil.GetLogDirectory(Context).FullName;
			InitBuildStats(path);
			InitLogFileList(path);
			InitAdjacentAnchors(path);
			testTiming.HRef = "TestTiming.aspx";
			string logFile = Request.QueryString[LogFile.LogQueryString];
			if (logFile != null && logFile.Length > 0)
				testTiming.HRef += LogFile.CreateUrl(logFile);
		}

		private void InitBuildStats(string path)
		{
			LogStatistics stats = LogStatistics.Create(path);

			StringBuilder buffer = new StringBuilder();
			buffer.Append("Latest Build Status: ");
			buffer.Append(stats.IsLatestBuildSuccessful() ? "Successful" : "Failed");
			buffer.Append("<br/>\n");

			buffer.Append("Time Since Latest Build: ");
			TimeSpan interval = stats.GetTimeSinceLatestBuild();
			if (interval.TotalHours >= 1)
			{
				buffer.Append((int)interval.TotalHours).Append(" hours");
			}
			else
			{
				buffer.Append((int)interval.TotalMinutes).Append(" min");
			}
			buildStats.InnerHtml = buffer.ToString();

			if (! stats.IsLatestBuildSuccessful())
			{
				buildStats.Attributes["class"] = "buildresults-data-failed";
			}
		}

		private void InitAdjacentAnchors(string path)
		{			
			string currentFile = Request.QueryString[LogFile.LogQueryString];
			LogFileLister.InitAdjacentAnchors(previousLog, nextLog, path, currentFile);			
		}

		private void InitLogFileList(string path)
		{
			menu.DataSource = LogFileLister.GetLinks(path);
			menu.DataBind();
		}

		private void DataList_BindItem(object sender, DataListItemEventArgs e)
		{
			if (e.Item.DataItem != null)
				e.Item.Controls.Add((Control)e.Item.DataItem);
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
			menu.ItemDataBound += new DataListItemEventHandler(this.DataList_BindItem);
		}
		#endregion
	}
}
