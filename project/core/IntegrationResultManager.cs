using System;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	public class IntegrationResultManager : IIntegrationResultManager
	{
		private readonly Project project;
		private IIntegrationResult lastResult;
		private IntegrationResult currentResult;

		public IntegrationResultManager(Project project)
		{
			this.project = project;
		}

		public IIntegrationResult LastIntegrationResult
		{
			get
			{
				if (lastResult == null)
				{
					if (project.StateManager.HasPreviousState(project.Name))
						lastResult = project.StateManager.LoadState(project.Name);
					else
						lastResult = IntegrationResult.CreateInitialIntegrationResult(project.Name, project.WorkingDirectory);
				}
				return lastResult;
			}
		}

		public IIntegrationResult StartNewIntegration(IntegrationRequest request)
		{
			currentResult = new IntegrationResult(project.Name, project.WorkingDirectory, project.StatisticsFile, request);

			currentResult.LastIntegrationStatus = LastIntegrationResult.Status;
			currentResult.LastSuccessfulIntegrationLabel = LastIntegrationResult.LastSuccessfulIntegrationLabel;

			currentResult.BuildCondition = DetermineBuildCondition(request.BuildCondition);
			currentResult.ArtifactDirectory = project.ArtifactDirectory;
			currentResult.ProjectUrl = project.WebURL;
			return currentResult;
		}

		private BuildCondition DetermineBuildCondition(BuildCondition buildCondition)
		{
			if (LastIntegrationResult.IsInitial())
			{
				return BuildCondition.ForceBuild;
			}
			return buildCondition;
		}

		public void FinishIntegration()
		{
			try
			{
				project.StateManager.SaveState(currentResult);
			}
			catch (Exception ex)
			{
				// swallow exception???
				Log.Error("Unable to save integration result: " + ex.ToString());
			}
			lastResult = currentResult;
		}
	}
}