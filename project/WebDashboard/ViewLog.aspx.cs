using System;
using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.WebDashboard.Cache;
using ThoughtWorks.CruiseControl.WebDashboard.Config;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.LogViewerPlugin;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard
{
	public class ViewLog : System.Web.UI.Page
	{
		protected HtmlGenericControl BodyArea;

		private void Page_Load(object sender, System.EventArgs e)
		{
			ConfigurationSettingsConfigGetter configurationGetter = new ConfigurationSettingsConfigGetter();
			LogViewer logViewer = new LogViewer(
				new QueryStringRequestWrapper(Request.QueryString), 
				new ServerAggregatingCruiseManagerWrapper(configurationGetter, new RemoteCruiseManagerFactory()) , 
				new LocalFileCacheManager(new HttpPathMapper(Context, this), configurationGetter)
			);

			LogViewerResults results = logViewer.Do();

			Response.Redirect(results.RedirectURL);
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
