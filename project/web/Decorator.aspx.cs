using System;
using System.Collections;
using System.Configuration;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SiteMesh.DecoratorControls;
using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.Web
{
	public class Decorator : Page
	{
		protected DataList menu;
		protected HtmlGenericControl buildStats;
		protected HtmlGenericControl ProjectPluginLinks;
		protected HtmlAnchor nextLog;
		protected HtmlAnchor previousLog;
		protected Title Title1;
		protected Body Body1;
		protected GetProperty prop1;
		protected HtmlTableCell Td2;
		protected Title Title3;
		protected HtmlTableCell contentCell;
		private LogFileLister logFileLister = new LogFileLister();

		private void Page_Load(object sender, EventArgs e)
		{
			string path = WebUtil.GetLogDirectory(Context).FullName;
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
				pluginLinksHtml += String.Format(@"|&nbsp; <a class=""link"" href=""{0}"">{1}</a> ", spec.LinkUrl, spec.LinkText);
			}
			ProjectPluginLinks.InnerHtml = pluginLinksHtml;
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
			logFileLister.InitAdjacentAnchors(previousLog, nextLog, path, currentFile);			
		}

		private void InitLogFileList(string path)
		{
			menu.DataSource = logFileLister.GetLinks(path);
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
