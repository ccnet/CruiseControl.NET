using System.IO;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class TopControlsViewBuilder : HtmlBuilderViewBuilder
	{
		private readonly IBuildNameFormatter buildNameFormatter;
		private readonly IUrlBuilder urlBuilder;

		public TopControlsViewBuilder(IHtmlBuilder htmlBuilder, IUrlBuilder urlBuilder, IBuildNameFormatter buildNameFormatter) : base (htmlBuilder)
		{
			this.urlBuilder = urlBuilder;
			this.buildNameFormatter = buildNameFormatter;
		}

		public Control Execute(ICruiseRequest request)
		{
			StringWriter writer = new StringWriter();
			HtmlTextWriter htmlWriter = new HtmlTextWriter(writer);

			HtmlAnchor dashboard = A("Dashboard", urlBuilder.BuildUrl("default.aspx"));
			dashboard.RenderControl(htmlWriter);

			string serverName = request.GetServerName();
			if (serverName != "")
			{
				htmlWriter.Write(" &gt; ");
				A(serverName, urlBuilder.BuildServerUrl("default.aspx", serverName)).RenderControl(htmlWriter);
			}

			string projectName = request.GetProjectName();
			if (projectName != "")
			{
				htmlWriter.Write(" &gt; ");
				A(projectName, urlBuilder.BuildProjectUrl("BuildReport.aspx", serverName, projectName)).RenderControl(htmlWriter);
			}

			string buildName = request.GetBuildName();
			if (buildName != "")
			{
				htmlWriter.Write(" &gt; ");
				A(buildNameFormatter.GetPrettyBuildName(buildName),
					urlBuilder.BuildBuildUrl("BuildReport.aspx", serverName, projectName, buildName)).RenderControl(htmlWriter);
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
