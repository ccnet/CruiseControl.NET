using System;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Builder;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol.Perforce;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class AddProjectModelGenerator
	{
		private readonly ICruiseManagerWrapper cruiseManagerWrapper;

		public AddProjectModelGenerator(ICruiseManagerWrapper cruiseManagerWrapper)
		{
			this.cruiseManagerWrapper = cruiseManagerWrapper;
		}

		public AddProjectModel GenerateModel(IRequest request)
		{
			string selectedServerName = request.GetText("ServersDropDown");
			string[] serverNames = cruiseManagerWrapper.GetServerNames();

			Project project = new Project();
			project.Name = request.GetText("Project.Name");
			project.WebURL = request.GetText("Project.WebURL");

			P4 p4 = new P4();
			p4.View = request.GetText("Project.SourceControl.View");
			p4.Executable = request.GetText("Project.SourceControl.Executable");
			p4.Client = request.GetText("Project.SourceControl.Client");
			p4.User = request.GetText("Project.SourceControl.User");
			p4.Port = request.GetText("Project.SourceControl.Port");
			p4.ApplyLabel = request.GetChecked("Project.SourceControl.ApplyLabel");
			p4.AutoGetSource = request.GetChecked("Project.SourceControl.AutoGetSource");
			project.SourceControl = p4;

			project.Builder = GenerateBuilder(request);

			MergeFilesTask mergeFilesTask = new MergeFilesTask();
			mergeFilesTask.MergeFilesForPresentation = request.GetText("Project.Tasks.0.MergeFilesForPresentation");
			project.Tasks = new ITask[] {mergeFilesTask};

			XmlLogPublisher logPublisher = new XmlLogPublisher();
			logPublisher.LogDir = request.GetText("Project.Publishers.0.LogDir");
			project.Publishers = new IIntegrationCompletedEventHandler[] { logPublisher };

			return new AddProjectModel(project, selectedServerName, serverNames);
		}

		private IBuilder GenerateBuilder(IRequest request)
		{
			string builderType = request.GetText("Project.Builder");
			if (builderType == null || builderType == "NAntBuilder")
			{
				return GenerateNAntBuilder(request);
			}
			else
			{
				return GenerateCommandLineBuilder(request);
			}
		}

		private NAntBuilder GenerateNAntBuilder(IRequest request)
		{
			NAntBuilder builder = new NAntBuilder();
			builder.Executable = request.GetText("Project.Builder.Executable");
			builder.ConfiguredBaseDirectory = request.GetText("Project.Builder.BaseDirectory");
			builder.BuildFile = request.GetText("Project.Builder.BuildFile");
			builder.BuildArgs = request.GetText("Project.Builder.BuildArgs");
			builder.TargetsForPresentation = request.GetText("Project.Builder.TargetsForPresentation");
			builder.BuildTimeoutSeconds = request.GetInt("Project.Builder.BuildTimeoutSeconds", builder.BuildTimeoutSeconds);
			return builder;
		}

		private CommandLineBuilder GenerateCommandLineBuilder(IRequest request)
		{
			CommandLineBuilder builder = new CommandLineBuilder();
			builder.Executable = request.GetText("Project.Builder.Executable");
			builder.ConfiguredBaseDirectory = request.GetText("Project.Builder.BaseDirectory");
			builder.BuildArgs = request.GetText("Project.Builder.BuildArgs");
			builder.BuildTimeoutSeconds = request.GetInt("Project.Builder.BuildTimeoutSeconds", builder.BuildTimeoutSeconds);
			return builder;
		}
	}
}
