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
					dcFactory.DefaultUrlBuilderWithHttpPathMapper, 
					dcFactory.CruiseManagerBuildNameRetriever,
				new DecoratingRecentBuildsPanelBuilder(
					dcFactory.DefaultHtmlBuilder,
					dcFactory.DefaultUrlBuilderWithHttpPathMapper, 
					new RecentBuildLister(
						dcFactory.DefaultHtmlBuilder, 
						dcFactory.DefaultUrlBuilderWithHttpPathMapper, 
						dcFactory.ServerAggregatingCruiseManagerWrapper,
						dcFactory.DefaultBuildNameFormatter))));

			TopControlsViewBuilder topControlsViewBuilder = 
				new TopControlsViewBuilder(
					dcFactory.DefaultHtmlBuilder, 
					dcFactory.DefaultUrlBuilderWithHttpPathMapper,
					dcFactory.DefaultBuildNameFormatter);

			SideBarLocation.Controls.Add(sideBarViewBuilder.Execute(dcFactory.RequestWrappingCruiseRequest));
			TopControlsLocation.Controls.Add(topControlsViewBuilder.Execute(dcFactory.RequestWrappingCruiseRequest));
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
