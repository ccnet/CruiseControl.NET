using System.Web.UI;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewProjectReport;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.AddProject
{
	public class SaveNewProjectAction : IAction
	{
		public static readonly string ACTION_NAME = "AddProjectSave";

		private readonly IUrlBuilder urlBuilder;
		private readonly IProjectSerializer serializer;
		private readonly ICruiseManagerWrapper cruiseManagerWrapper;
		private readonly AddProjectViewBuilder viewBuilder;
		private readonly AddProjectModelGenerator projectModelGenerator;

		public SaveNewProjectAction(AddProjectModelGenerator projectModelGenerator, AddProjectViewBuilder viewBuilder, 
			ICruiseManagerWrapper cruiseManagerWrapper, IProjectSerializer serializer, IUrlBuilder urlBuilder)
		{
			this.projectModelGenerator = projectModelGenerator;
			this.viewBuilder = viewBuilder;
			this.cruiseManagerWrapper = cruiseManagerWrapper;
			this.serializer = serializer;
			this.urlBuilder = urlBuilder;
		}

		public Control Execute(IRequest request)
		{
			AddEditProjectModel model = projectModelGenerator.GenerateModel(request);
			SetProjectUrlIfOneNotSet(model);
			try
			{
				cruiseManagerWrapper.AddProject(model.SelectedServerName, serializer.Serialize(model.Project));
				model.Status = "Project saved successfully";
				model.IsAdd = true;
				model.SaveActionName = "";
			}
			catch (CruiseControlException e)
			{
				model.Status = "Failed to create project. Reason given was: " + e.Message;
				model.SaveActionName = SaveNewProjectAction.ACTION_NAME;
				model.IsAdd = true;
			}
			
			return viewBuilder.BuildView(model);
		}

		private void SetProjectUrlIfOneNotSet(AddEditProjectModel model)
		{
			if (model.Project.WebURL == null || model.Project.WebURL == string.Empty)
			{
				model.Project.WebURL = urlBuilder.BuildProjectUrl(new ActionSpecifierWithName(ViewProjectReportAction.ACTION_NAME), model.SelectedServerName, model.Project.Name);
			}
		}
	}
}
