using System.IO;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class TopControlsViewBuilder : HtmlBuilderViewBuilder
	{
		private readonly ICruiseRequest request;
		private readonly IBuildNameFormatter buildNameFormatter;
		private readonly IUrlBuilder urlBuilder;

		public TopControlsViewBuilder(IHtmlBuilder htmlBuilder, IUrlBuilder urlBuilder, IBuildNameFormatter buildNameFormatter, ICruiseRequest request) : base (htmlBuilder)
		{
			this.urlBuilder = urlBuilder;
			this.buildNameFormatter = buildNameFormatter;
			this.request = request;
		}

		public Control Execute()
		{
			StringWriter writer = new StringWriter();
			HtmlTextWriter htmlWriter = new HtmlTextWriter(writer);

			HtmlAnchor dashboard = A("Dashboard", urlBuilder.BuildUrl("default.aspx"));
			dashboard.RenderControl(htmlWriter);

			string serverName = request.ServerName;
			if (serverName != "")
			{
				htmlWriter.Write(" &gt; ");
				A(serverName, urlBuilder.BuildServerUrl("default.aspx", request.ServerSpecifier)).RenderControl(htmlWriter);
			}

			string projectName = request.ProjectName;
			if (projectName != "")
			{
				htmlWriter.Write(" &gt; ");
				A(projectName, urlBuilder.BuildProjectUrl(new ActionSpecifierWithName("ViewProjectReport"), request.ProjectSpecifier)).RenderControl(htmlWriter);
			}

			string buildName = request.BuildName;
			if (buildName != "")
			{
				htmlWriter.Write(" &gt; ");

				IBuildSpecifier buildSpecifier = request.BuildSpecifier;
				A(buildNameFormatter.GetPrettyBuildName(buildSpecifier),
					urlBuilder.BuildBuildUrl(new ActionSpecifierWithName(ViewBuildReportAction.ACTION_NAME), buildSpecifier)).RenderControl(htmlWriter);
			}

			HtmlGenericControl locationMenu = new HtmlGenericControl("div");
			locationMenu.InnerHtml = writer.ToString();

			HtmlTable table = Table();
			table.Attributes.Add("class", "breadcrumbs");
			table.Rows.Add(TR( TD( locationMenu )));
			return table;
		}
	}
}
