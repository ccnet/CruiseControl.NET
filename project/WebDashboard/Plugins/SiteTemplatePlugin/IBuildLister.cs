using System.Web.UI.HtmlControls;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.SiteTemplatePlugin
{
	public interface IBuildLister
	{
		HtmlAnchor[] GetBuildLinks(string serverName, string projectName);
	}
}
