using System;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol.Perforce;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.AddProject
{
	//		Commented by Mike Roberts - this is in development - please contact me if you change it
//	public class AddProjectModelGenerator
//	{
//		public AddProjectModelGenerator()
//		{
//		}
//
//		public AddEditProjectModel GenerateNewProjectModel()
//		{
//			Project project = new Project();
//			// ToDo - these defaults should be set in the Project itself.
//			project.Name = "[new project]";
//			project.SourceControl = new P4();
//			project.Builder = new NAntBuilder();
//			return new AddEditProjectModel(project);
//		}
//
//		public AddEditProjectModel GenerateModelFromRequestWithNewSourceControl(IRequest request)
//		{
//			AddEditProjectModel model = GenerateModelFromRequest(request);
//
//			string type = request.GetText("Project.SourceControl");
//			if (type == null || type == "P4")
//			{
//				model.Project.SourceControl = new P4();
//			}
//			else if (type == "Cvs")
//			{
//				model.Project.SourceControl = new Cvs();
//			}
//			else
//			{
//				model.Project.SourceControl = new FileSourceControl();
//			}
//			
//			return model;
//		}
//	
//		public AddEditProjectModel GenerateModelFromRequestWithNewBuilder(IRequest request)
//		{
//			AddEditProjectModel model = GenerateModelFromRequest(request);
//
//			string type = request.GetText("Project.Builder");
//			if (type == null || type == "NAntBuilder")
//			{
//				model.Project.Builder = new NAntBuilder();
//			}
//			else 
//			{
//				model.Project.Builder = new CommandLineBuilder();
//			}
//			
//			return model;
//		}
//	
//		public AddEditProjectModel GenerateModelFromRequest(IRequest request)
//		{
//			Project project = new Project();
//
//			project.Name = request.GetText("Project.Name");
//			project.WebURL = request.GetText("Project.WebURL");
//			project.SourceControl = GenerateSourceControl(request);
//			project.Builder = GenerateBuilder(request);
//			project.ConfiguredWorkingDirectory = request.GetText("Project.ConfiguredWorkingDirectory");
//
//			MergeFilesTask mergeFilesTask = new MergeFilesTask();
//			mergeFilesTask.MergeFilesForPresentation = request.GetText("Project.Tasks.0.MergeFilesForPresentation");
//			project.Tasks = new ITask[] {mergeFilesTask};
//			project.Publishers = new IIntegrationCompletedEventHandler[] { new XmlLogPublisher() };
//
//			return new AddEditProjectModel(project);
//		}
//
//		private IBuilder GenerateBuilder(IRequest request)
//		{
//			string type = request.GetText("Project.Builder");
//			if (type == null || type == "NAntBuilder")
//			{
//				return GenerateNAntBuilder(request);
//			}
//			else
//			{
//				return GenerateCommandLineBuilder(request);
//			}
//		}
//
//		private ISourceControl GenerateSourceControl(IRequest request)
//		{
//			string type = request.GetText("Project.SourceControl");
//			if (type == null || type == "P4")
//			{
//				return GenerateP4(request);
//			}
//			else if (type == "Cvs")
//			{
//				return GenerateCvs(request);
//			}
//			else
//			{
//				return GenerateFileSourceControl(request);
//			}
//		}
//
//		private NAntBuilder GenerateNAntBuilder(IRequest request)
//		{
//			NAntBuilder builder = new NAntBuilder();
//			builder.Executable = request.GetText("Project.Builder.Executable");
//			builder.ConfiguredBaseDirectory = request.GetText("Project.Builder.BaseDirectory");
//			builder.BuildFile = request.GetText("Project.Builder.BuildFile");
//			builder.BuildArgs = request.GetText("Project.Builder.BuildArgs");
//			builder.TargetsForPresentation = request.GetText("Project.Builder.TargetsForPresentation");
//			builder.BuildTimeoutSeconds = request.GetInt("Project.Builder.BuildTimeoutSeconds", builder.BuildTimeoutSeconds);
//			return builder;
//		}
//
//		private CommandLineBuilder GenerateCommandLineBuilder(IRequest request)
//		{
//			CommandLineBuilder builder = new CommandLineBuilder();
//			builder.Executable = request.GetText("Project.Builder.Executable");
//			builder.ConfiguredBaseDirectory = request.GetText("Project.Builder.BaseDirectory");
//			builder.BuildArgs = request.GetText("Project.Builder.BuildArgs");
//			builder.BuildTimeoutSeconds = request.GetInt("Project.Builder.BuildTimeoutSeconds", builder.BuildTimeoutSeconds);
//			return builder;
//		}
//
//		private P4 GenerateP4(IRequest request)
//		{
//			P4 p4 = new P4();
//			p4.View = request.GetText("Project.SourceControl.View") == null ? "" : request.GetText("Project.SourceControl.View").Replace(Environment.NewLine, ",");
//			p4.Executable = request.GetText("Project.SourceControl.Executable");
//			p4.Client = request.GetText("Project.SourceControl.Client");
//			p4.User = request.GetText("Project.SourceControl.User");
//			p4.Port = request.GetText("Project.SourceControl.Port");
//			p4.ApplyLabel = request.GetChecked("Project.SourceControl.ApplyLabel");
//			p4.AutoGetSource = request.GetChecked("Project.SourceControl.AutoGetSource");
//			return p4;
//		}
//
//		private Cvs GenerateCvs(IRequest request)
//		{
//			Cvs cvs = new Cvs();
//			cvs.Executable = request.GetText("Project.SourceControl.Executable");
//			cvs.Timeout = request.GetInt("Project.SourceControl.Timeout", cvs.Timeout);
//			cvs.CvsRoot = request.GetText("Project.SourceControl.CvsRoot");
//			cvs.WorkingDirectory = request.GetText("Project.SourceControl.WorkingDirectory");
//			cvs.LabelOnSuccess = request.GetChecked("Project.SourceControl.LabelOnSuccess");
//			cvs.RestrictLogins = request.GetText("Project.SourceControl.RestrictLogins");
//			cvs.Branch = request.GetText("Project.SourceControl.Branch");
//			cvs.AutoGetSource = request.GetChecked("Project.SourceControl.AutoGetSource");
//			return cvs;
//		}
//
//		private FileSourceControl GenerateFileSourceControl(IRequest request)
//		{
//			FileSourceControl fileSourceControl = new FileSourceControl();
//			fileSourceControl.RepositoryRoot = request.GetText("Project.SourceControl.RepositoryRoot");
//			fileSourceControl.IgnoreMissingRoot = request.GetChecked("Project.SourceControl.IgnoreMissingRoot");
//			return fileSourceControl;
//		}
//	}
}
