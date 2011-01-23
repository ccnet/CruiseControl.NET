using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.DeleteProject
{
	public interface IDeleteProjectViewBuilder
	{
		IResponse BuildView(DeleteProjectModel model);
	}
}
