using System;
using System.Collections;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Builder;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol.Perforce;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.AddProject
{
//	public class AddProjectViewBuilder
//	{
//		private readonly Type[] SourceControlTypesAvailable = new Type[] { typeof(P4), typeof(Cvs), typeof(FileSourceControl)};
//		private readonly Type[] BuilderTypesAvailable = new Type[] { typeof(NAntBuilder), typeof(CommandLineBuilder)};
//
//		private readonly IVelocityViewGenerator velocityViewGenerator;
//
//		public AddProjectViewBuilder(IVelocityViewGenerator velocityViewGenerator)
//		{
//			this.velocityViewGenerator = velocityViewGenerator;
//		}
//
//		public IView BuildView(AddEditProjectModel model)
//		{
//			Hashtable velocityContext = new Hashtable();
//			velocityContext["model"] = model;
//			velocityContext["MergeFiles"] = GetMergeFiles(model.Project);
//			velocityContext["SourceControlTypes"] = GetPickableTypes(SourceControlTypesAvailable, model.Project.SourceControl);
//			velocityContext["SourceControlDetails"] = GetChildView(model.Project.SourceControl).HtmlFragment;
//			velocityContext["BuilderTypes"] = GetPickableTypes(BuilderTypesAvailable, model.Project.Builder);
//			velocityContext["BuilderDetails"] = GetChildView(model.Project.Builder).HtmlFragment;
//			
//			return velocityViewGenerator.GenerateView("AddProject.vm", velocityContext);
//		}
//
//		private string GetMergeFiles(Project project)
//		{
//			string mergefiles = "";
//			foreach (ITask task in project.Tasks)
//			{
//				if (task is MergeFilesTask)
//				{
//					mergefiles = ((MergeFilesTask) task).MergeFilesForPresentation;
//				}
//			}
//			return mergefiles;
//		}
//
//		private IList GetPickableTypes(Type[] availableTypes, object currentSelection)
//		{
//			ArrayList types = new ArrayList();
//			foreach(Type type in availableTypes)
//			{
//				NameAndSelected nameAndSelected = new NameAndSelected(type.Name);
//				if (currentSelection.GetType() == type)
//				{
//					nameAndSelected.Selected = true;
//				}
//				types.Add(nameAndSelected);
//			}
//			return types;
//		}
//
//		private IView GetChildView(object child)
//		{
//			Hashtable context = new Hashtable();
//			context["model"] = child;
//			return velocityViewGenerator.GenerateView(string.Format("EditProject{0}.vm", child.GetType().Name), context);
//		}
//	}
}
