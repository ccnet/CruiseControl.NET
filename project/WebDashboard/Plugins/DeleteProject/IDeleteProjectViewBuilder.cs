using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.DeleteProject
{
	public interface IDeleteProjectViewBuilder
	{
		IView BuildView(DeleteProjectModel model);
	}
}
