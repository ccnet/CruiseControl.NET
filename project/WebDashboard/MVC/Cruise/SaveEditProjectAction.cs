using System.Web.UI;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class SaveEditProjectAction : ICruiseAction
	{
		private readonly IUrlBuilder urlBuilder;
		private readonly IProjectSerializer serializer;
		private readonly ICruiseManagerWrapper cruiseManagerWrapper;
		private readonly AddProjectViewBuilder viewBuilder;
		private readonly AddProjectModelGenerator projectModelGenerator;

		public SaveEditProjectAction(AddProjectModelGenerator projectModelGenerator, AddProjectViewBuilder viewBuilder, 
			ICruiseManagerWrapper cruiseManagerWrapper, IProjectSerializer serializer, IUrlBuilder urlBuilder)
		{
			this.projectModelGenerator = projectModelGenerator;
			this.viewBuilder = viewBuilder;
			this.cruiseManagerWrapper = cruiseManagerWrapper;
			this.serializer = serializer;
			this.urlBuilder = urlBuilder;
		}

		// Todo - move both add and edit action to server page and just use server in URL (no dropdown)
		public Control Execute(ICruiseRequest request)
		{
			AddEditProjectModel model = projectModelGenerator.GenerateModel(request.Request);
			model.Project.Name = request.ProjectName;
			model.SelectedServerName = request.ServerName;
			SetProjectUrlIfOneNotSet(model);
			try
			{
				cruiseManagerWrapper.UpdateProject(request.ServerName, request.ProjectName, serializer.Serialize(model.Project));
				model.Status = "Project saved successfully";
				model.IsAdd = false;
				model.SaveActionName = "";
			}
			catch (CruiseControlException e)
			{
				model.Status = "Failed to update project. Reason given was: " + e.Message;
				model.SaveActionName = CruiseActionFactory.EDIT_PROJECT_SAVE_ACTION_NAME;
				model.IsAdd = false;
			}
			
			return viewBuilder.BuildView(model);
		}

		private void SetProjectUrlIfOneNotSet(AddEditProjectModel model)
		{
			if (model.Project.WebURL == null || model.Project.WebURL == string.Empty)
			{
				model.Project.WebURL = urlBuilder.BuildProjectUrl(new ActionSpecifierWithName(CruiseActionFactory.VIEW_PROJECT_REPORT_ACTION_NAME), model.SelectedServerName, model.Project.Name);
			}
		}
	}
}
