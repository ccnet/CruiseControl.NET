using System.IO;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class TopControlsViewBuilder : HtmlBuilderViewBuilder
	{
		private readonly IUrlBuilder urlBuilder;

		public TopControlsViewBuilder(IHtmlBuilder htmlBuilder, IUrlBuilder urlBuilder) : base (htmlBuilder)
		{
			this.urlBuilder = urlBuilder;
		}

		public Control Execute(ICruiseRequestWrapper request)
		{
			StringWriter writer = new StringWriter();
			HtmlTextWriter htmlWriter = new HtmlTextWriter(writer);

			HtmlAnchor dashboard = A("Dashboard", urlBuilder.BuildUrl("default.aspx"));
			dashboard.RenderControl(htmlWriter);

			string serverName = request.GetServerName();
			if (serverName != "")
			{
				htmlWriter.Write(" &gt; ");
				A(serverName, urlBuilder.BuildUrl("default.aspx","server=" + serverName)).RenderControl(htmlWriter);
			}

			string projectName = request.GetProjectName();
			if (projectName != "")
			{
				htmlWriter.Write(" &gt; ");
				A(projectName, urlBuilder.BuildUrl("BuildReport.aspx","server=" + serverName + "&project=" + projectName)).RenderControl(htmlWriter);
			}

			HtmlGenericControl locationMenu = new HtmlGenericControl("div");
			locationMenu.InnerHtml = writer.ToString();

			return Table(
				TR( TD( locationMenu )));
		}
	}
}
