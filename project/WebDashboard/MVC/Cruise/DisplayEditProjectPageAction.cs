using System.Web.UI;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class DisplayEditProjectPageAction : ICruiseAction
	{
		private readonly ICruiseManagerWrapper cruiseManagerWrapper;
		private readonly IProjectSerializer serializer;
		private readonly AddProjectViewBuilder viewBuilder;
		private readonly AddProjectModelGenerator projectModelGenerator;

		public DisplayEditProjectPageAction(AddProjectModelGenerator projectModelGenerator, AddProjectViewBuilder viewBuilder,
		                                    ICruiseManagerWrapper cruiseManagerWrapper, IProjectSerializer serializer)
		{
			this.projectModelGenerator = projectModelGenerator;
			this.viewBuilder = viewBuilder;
			this.serializer = serializer;
			this.cruiseManagerWrapper = cruiseManagerWrapper;
		}

		public Control Execute(ICruiseRequest request)
		{
			AddEditProjectModel model = null;
			if (request.Request.GetText("Project.SourceControl") == null || request.Request.GetText("Project.SourceControl") == string.Empty)
			{
				Project project = serializer.Deserialize(cruiseManagerWrapper.GetProject(request.ServerName, request.ProjectName));
				model = new AddEditProjectModel(project, request.ServerName, new string[0]);
			}
			else
			{
				model = projectModelGenerator.GenerateModel(request.Request);
			}
			model.SaveActionName = CruiseActionFactory.EDIT_PROJECT_SAVE_ACTION_NAME;
			model.IsAdd = false;
			model.Status = "";
			return viewBuilder.BuildView(model);
		}
	}
}
