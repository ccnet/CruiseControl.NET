using System.Web.UI;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.DeleteProject
{
	public interface IDeleteProjectViewBuilder
	{
		Control BuildView(DeleteProjectModel model);
	}
}
