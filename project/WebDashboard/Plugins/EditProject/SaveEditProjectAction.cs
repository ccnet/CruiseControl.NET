using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.AddProject;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.EditProject
{
	//		Commented by Mike Roberts - this is in development - please contact me if you change it
//	public class SaveEditProjectAction : ICruiseAction
//	{
//		public static readonly string ACTION_NAME = "EditProjectSave";
//
//		private readonly IUrlBuilder urlBuilder;
//		private readonly IProjectSerializer serializer;
//		private readonly ICruiseManagerWrapper cruiseManagerWrapper;
//		private readonly AddProjectViewBuilder viewBuilder;
//		private readonly AddProjectModelGenerator projectModelGenerator;
//
//		public SaveEditProjectAction(AddProjectModelGenerator projectModelGenerator, AddProjectViewBuilder viewBuilder, 
//			ICruiseManagerWrapper cruiseManagerWrapper, IProjectSerializer serializer, IUrlBuilder urlBuilder)
//		{
//			this.projectModelGenerator = projectModelGenerator;
//			this.viewBuilder = viewBuilder;
//			this.cruiseManagerWrapper = cruiseManagerWrapper;
//			this.serializer = serializer;
//			this.urlBuilder = urlBuilder;
//		}
//
//		// Todo - move both add and edit action to server page and just use server in URL (no dropdown)
//		public IView Execute(ICruiseRequest request)
//		{
//			AddEditProjectModel model = projectModelGenerator.GenerateModelFromRequest(request);
//			model.Project.Name = request.ProjectName;
//			SetProjectUrlIfOneNotSet(model, request.ProjectSpecifier);
//			try
//			{
//				cruiseManagerWrapper.UpdateProject(request.ProjectSpecifier, serializer.Serialize(model.Project));
//				model.Status = "Project saved successfully";
//				model.IsAdd = false;
//				model.SaveActionName = "";
//			}
//			catch (CruiseControlException e)
//			{
//				model.Status = "Failed to update project. Reason given was: " + e.Message;
//				model.SaveActionName = ACTION_NAME;
//				model.IsAdd = false;
//			}
//			
//			return viewBuilder.BuildView(model);
//		}
//
//		private void SetProjectUrlIfOneNotSet(AddEditProjectModel model, IProjectSpecifier projectSpecifier)
//		{
//			if (model.Project.WebURL == null || model.Project.WebURL == string.Empty)
//			{
//				model.Project.WebURL = urlBuilder.BuildProjectUrl(new ActionSpecifierWithName(ProjectReportProjectPlugin.ACTION_NAME), projectSpecifier);
//			}
//		}
//	}
}
