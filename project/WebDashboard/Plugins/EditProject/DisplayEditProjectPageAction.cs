using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.AddProject;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.EditProject
{
	//		Commented by Mike Roberts - this is in development - please contact me if you change it
//	public class DisplayEditProjectPageAction : ICruiseAction
//	{
//		private readonly ICruiseManagerWrapper cruiseManagerWrapper;
//		private readonly IProjectSerializer serializer;
//		private readonly AddProjectViewBuilder viewBuilder;
//		private readonly AddProjectModelGenerator projectModelGenerator;
//
//		public DisplayEditProjectPageAction(AddProjectModelGenerator projectModelGenerator, AddProjectViewBuilder viewBuilder,
//		                                    ICruiseManagerWrapper cruiseManagerWrapper, IProjectSerializer serializer)
//		{
//			this.projectModelGenerator = projectModelGenerator;
//			this.viewBuilder = viewBuilder;
//			this.serializer = serializer;
//			this.cruiseManagerWrapper = cruiseManagerWrapper;
//		}
//
//		public IView Execute(ICruiseRequest request)
//		{
//			AddEditProjectModel model = null;
//			if (request.Request.GetText("Project.SourceControl") == null || request.Request.GetText("Project.SourceControl") == string.Empty)
//			{
//				Project project = serializer.Deserialize(cruiseManagerWrapper.GetProject(request.ProjectSpecifier));
//				model = new AddEditProjectModel(project);
//			}
//			else
//			{
//				model = projectModelGenerator.GenerateModelFromRequest(request);
//			}
//			model.SaveActionName = SaveEditProjectAction.ACTION_NAME;
//			model.IsAdd = false;
//			model.Status = "";
//			return viewBuilder.BuildView(model);
//		}
//	}
}
