using System;
using System.Collections;
using System.Configuration;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SiteMesh.DecoratorControls;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.WebDashboard.Config;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard
{
	public class Decorator : Page
	{
		protected DataList menu;
		protected HtmlGenericControl buildStats;
		protected HtmlGenericControl ProjectPluginLinks;
		protected HtmlAnchor latestLog;
		protected HtmlAnchor nextLog;
		protected HtmlAnchor previousLog;
		protected Title Title1;
		protected Body Body1;
		protected GetProperty prop1;
		protected HtmlTableCell Td2;
		protected Title Title3;
		protected HtmlTableCell contentCell;
		protected WebUtil webUtil;

		private void Page_Load(object sender, EventArgs e)
		{
			webUtil = WebUtil.Create(Request, Context);
			string path = webUtil.GetLogDirectory().FullName;
			InitBuildStats(path);
			InitLogFileList(path);
			InitAdjacentAnchors(path);
			InitProjectPluginLinks();
		}

		private void InitProjectPluginLinks()
		{
			if (ConfigurationSettings.GetConfig("CCNet/projectPlugins") == null)
			{
				return;
			}

			string pluginLinksHtml = "";
			foreach (PluginSpecification spec in (IEnumerable) ConfigurationSettings.GetConfig("CCNet/projectPlugins"))
			{
				pluginLinksHtml += String.Format(@"|&nbsp; <a class=""link"" href=""{0}"">{1}</a> ", DecoratePluginLinkWithProjectName(spec.LinkUrl), spec.LinkText);
			}
			ProjectPluginLinks.InnerHtml = pluginLinksHtml;
		}

		private string DecoratePluginLinkWithProjectName(string url)
		{
			return string.Format("{0}?{1}={2}", url, LogFileUtil.ProjectQueryString, webUtil.GetCurrentlyViewedProjectName());
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
			string currentFile = Request.QueryString[LogFileUtil.LogQueryString];
			LogFileLister.InitAdjacentAnchors(latestLog, previousLog, nextLog, path, currentFile, webUtil.GetCurrentlyViewedProjectName());			
		}

		private void InitLogFileList(string path)
		{
			menu.DataSource = LogFileLister.GetLinks(path, webUtil.GetCurrentlyViewedProjectName());
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
			this.menu.ItemDataBound += new DataListItemEventHandler(this.DataList_BindItem);
			this.Load += new EventHandler(this.Page_Load);

		}
		#endregion
	}
}
