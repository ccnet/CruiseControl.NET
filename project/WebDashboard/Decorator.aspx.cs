using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard
{
	public class Decorator : Page
	{
		protected System.Web.UI.HtmlControls.HtmlGenericControl TopControlsLocation;
		protected HtmlGenericControl SideBarLocation;

		private void Page_Load(object sender, EventArgs e)
		{
			DashboardComponentFactory dcFactory = new DashboardComponentFactory(Request, Context, this);

			SideBarViewBuilder sideBarViewBuilder = new SideBarViewBuilder(
				new DefaultUserRequestSpecificSideBarViewBuilder(
					dcFactory.DefaultHtmlBuilder, 
					dcFactory.DefaultUrlBuilder, 
					dcFactory.CruiseManagerBuildNameRetriever,
				new DecoratingRecentBuildsPanelBuilder(
					dcFactory.DefaultHtmlBuilder,
					dcFactory.DefaultUrlBuilder, 
					new RecentBuildLister(
						dcFactory.DefaultHtmlBuilder, 
						dcFactory.DefaultUrlBuilder, 
						dcFactory.ServerAggregatingCruiseManagerWrapper,
						dcFactory.DefaultBuildNameFormatter))));

			TopControlsViewBuilder topControlsViewBuilder = 
				new TopControlsViewBuilder(
					dcFactory.DefaultHtmlBuilder, 
					dcFactory.DefaultUrlBuilder,
					dcFactory.DefaultBuildNameFormatter);

			SideBarLocation.Controls.Add(sideBarViewBuilder.Execute(dcFactory.QueryStringRequestWrapper));
			TopControlsLocation.Controls.Add(topControlsViewBuilder.Execute(dcFactory.QueryStringRequestWrapper));
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
