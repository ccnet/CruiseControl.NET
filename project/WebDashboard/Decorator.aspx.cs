using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;

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

			TopControlsViewBuilder topControlsViewBuilder = new TopControlsViewBuilder(dcFactory.DefaultHtmlBuilder, dcFactory.DefaultUrlBuilder);

			SideBarLocation.Controls.Add(sideBarViewBuilder.Execute(dcFactory.QueryStringRequestWrapper));
			TopControlsLocation.Controls.Add(topControlsViewBuilder.Execute(dcFactory.QueryStringRequestWrapper));

//			SiteTemplateResults results = new PluginPageRendererFactory(new DashboardComponentFactory(Request, Context, this)).SiteTemplate.Do();

//			FarmActions.Controls.Add(results.FarmControl);

			/*
			if (results.ProjectMode)
			{
				buildStats.InnerHtml = results.BuildStatsHtml;
				buildStats.Attributes["class"] = results.BuildStatsClass;

				ServerPluginsList.DataSource = results.ServerPluginsList;
				ServerPluginsList.DataBind();

				menu.DataSource = results.BuildLinkList;
				menu.DataBind();

				BuildPluginsList.DataSource = results.BuildPluginsList;
				BuildPluginsList.DataBind();
			}

			ProjectPanel.Visible = results.ProjectMode;
			*/
		}

		// This binds the HRef control that is each data item into the Controls container of the list
		/*
		private void DataList_BindItem(object sender, DataListItemEventArgs e)
		{
			if (e.Item.DataItem != null)
				e.Item.Controls.Add((Control)e.Item.DataItem);
		}
		*/

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
