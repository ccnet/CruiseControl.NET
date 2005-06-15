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
					lastResult = LoadLastIntegration();
				}
				return lastResult;
			}
		}

		private IIntegrationResult LoadLastIntegration()
		{
			IIntegrationResult result = project.StateManager.LoadState(project.Name);
			result.WorkingDirectory = project.WorkingDirectory;
			return result;
		}

		public IIntegrationResult StartNewIntegration(BuildCondition buildCondition)
		{
			currentResult = new IntegrationResult(project.Name, project.WorkingDirectory);
			currentResult.LastIntegrationStatus = LastIntegrationResult.Status;
			currentResult.BuildCondition = DetermineBuildCondition(buildCondition);
			currentResult.Label = project.Labeller.Generate(LastIntegrationResult);
			currentResult.ArtifactDirectory = project.ArtifactDirectory;
			currentResult.ProjectUrl = project.WebURL;
			currentResult.LastSuccessfulIntegrationLabel = LastIntegrationResult.LastSuccessfulIntegrationLabel;
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
