using System;
using System.Web.UI;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class SaveNewProjectAction : IAction
	{
		private readonly IProjectSerializer serializer;
		private readonly ICruiseManagerWrapper cruiseManagerWrapper;
		private readonly AddProjectViewBuilder viewBuilder;
		private readonly AddProjectModelGenerator projectModelGenerator;

		public SaveNewProjectAction(AddProjectModelGenerator projectModelGenerator, AddProjectViewBuilder viewBuilder, 
			ICruiseManagerWrapper cruiseManagerWrapper, IProjectSerializer serializer)
		{
			this.projectModelGenerator = projectModelGenerator;
			this.viewBuilder = viewBuilder;
			this.cruiseManagerWrapper = cruiseManagerWrapper;
			this.serializer = serializer;
		}

		public Control Execute(IRequest request)
		{
			AddProjectModel model = projectModelGenerator.GenerateModel(request);
			try
			{
				cruiseManagerWrapper.AddProject(model.SelectedServerName, serializer.Serialize(model.Project));
				model.Status = "Project saved successfully";
				model.AllowSave = false;
			}
			catch (CruiseControlException e)
			{
				model.Status = "Failed to create project. Reason given was: " + e.Message;
				model.AllowSave = true;
			}
			
			return viewBuilder.BuildView(model);
		}
	}
}
