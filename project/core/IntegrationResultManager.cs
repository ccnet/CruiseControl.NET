using System;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	public class IntegrationResultManager
	{
		private readonly Project _project;
		private IIntegrationResult _lastResult;
		private IntegrationResult _currentResult;

		public IntegrationResultManager(Project project)
		{
			_project = project;
		}

		public IIntegrationResult LastIntegrationResult
		{
			get
			{
				if (_lastResult == null)
				{
					_lastResult = LoadLastIntegration();
				}
				return _lastResult;
			}
		}

		private IIntegrationResult LoadLastIntegration()
		{
			if (_project.StateManager.StateFileExists())
			{
				return _project.StateManager.LoadState();
			}
			return IntegrationResult.CreateInitialIntegrationResult(_project.Name, _project.WorkingDirectory);
		}

		public IIntegrationResult StartNewIntegration(BuildCondition buildCondition)
		{
			_currentResult = new IntegrationResult(_project.Name, _project.WorkingDirectory);
			_currentResult.LastIntegrationStatus = LastIntegrationResult.Status;
			_currentResult.BuildCondition = DetermineBuildCondition(buildCondition);
			_currentResult.Label = _project.Labeller.Generate(LastIntegrationResult);
			_currentResult.ArtifactDirectory = _project.ArtifactDirectory;
			_currentResult.ProjectUrl = _project.WebURL;
			return _currentResult;
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
				_project.StateManager.SaveState(_currentResult);
			}
			catch (Exception ex)
			{
				Log.Error("Unable to save integration result: " + ex.ToString());
			}
			_lastResult = _currentResult;
		}
	}
}
