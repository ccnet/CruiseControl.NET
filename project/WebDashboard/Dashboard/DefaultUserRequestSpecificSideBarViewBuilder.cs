using System.Web.UI;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class DefaultUserRequestSpecificSideBarViewBuilder : HtmlBuilderViewBuilder, IUserRequestSpecificSideBarViewBuilder
	{
		private readonly IUrlBuilder urlBuilder;

		public DefaultUserRequestSpecificSideBarViewBuilder(IHtmlBuilder htmlBuilder, IUrlBuilder urlBuilder) : base (htmlBuilder)
		{
			this.urlBuilder = urlBuilder;
		}

		public Control GetFarmSideBar()
		{
			return Table(
				TR( TD( A("Add Project", urlBuilder.BuildUrl("controller.aspx")))));
		}

		public Control GetServerSideBar(string serverName)
		{
			return Table(
				TR( TD( A("View Server Log", urlBuilder.BuildUrl("ViewServerLog.aspx", "server=" + serverName)))),
				TR( TD( A("Add Project", urlBuilder.BuildUrl("controller.aspx", "server=" + serverName)))));
		}

		public Control GetProjectSideBar(string serverName, string projectName)
		{
			return Table(
				TR( TD( "Project Side Bar Not Implemented" )));
		}

		public Control GetBuildSideBar(string serverName, string projectName, string buildName)
		{
			return Table(
				TR( TD( "Build Side Bar Not Implemented" )));
		}
	}
}
