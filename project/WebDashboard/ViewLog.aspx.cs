using System;
using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.WebDashboard.Cache;
using ThoughtWorks.CruiseControl.WebDashboard.Config;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.LogViewerPlugin;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard
{
	public class ViewLog : System.Web.UI.Page
	{
		protected HtmlAnchor LogLink;

		private void Page_Load(object sender, System.EventArgs e)
		{
			ConfigurationSettingsConfigGetter configurationGetter = new ConfigurationSettingsConfigGetter();
			QueryStringRequestWrapper requestWrapper = new QueryStringRequestWrapper(Request.QueryString);
			LogViewer logViewer = new LogViewer(
				new CachingBuildRetriever(
					new ServerAggregatingCruiseManagerWrapper(configurationGetter, new RemoteCruiseManagerFactory()) , 
					new LocalFileCacheManager(new HttpPathMapper(Context, this), configurationGetter),
					requestWrapper)
				);

			LogViewerResults results = logViewer.Do();
			LogLink.HRef = results.RedirectURL;
		}

		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);
		}
		
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
		}
	}
}
