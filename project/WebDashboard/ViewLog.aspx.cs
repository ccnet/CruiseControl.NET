using System;
using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.Core.BuildLog;
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
			LogViewer logViewer = new LogViewer(new QueryStringRequestWrapper(Request.QueryString), 
				new ServerAggregatingCruiseManagerWrapper() , 
				new DefaultLogInspector(), 
				new LocalFileCacheManager(new HttpPathMapper(Context, this), new ConfigurationSettingsConfigGetter()));

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
