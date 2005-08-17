using System;
using System.IO;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	public class IntegrationRunner : IIntegratable
	{
		public IIntegrationRunnerTarget target;
		private readonly IIntegrationResultManager resultManager;
		private readonly IQuietPeriod quietPeriod;

		public IntegrationRunner(IIntegrationResultManager resultManager, IIntegrationRunnerTarget target, IQuietPeriod quietPeriod)
		{
			this.target = target;
			this.quietPeriod = quietPeriod;
			this.resultManager = resultManager;
		}

		// ToDo - this is all still a bit messy - could do with cleaning up
		public IIntegrationResult RunIntegration(BuildCondition buildCondition)
		{
			IIntegrationResult result = resultManager.StartNewIntegration(buildCondition);
			IIntegrationResult lastResult = resultManager.LastIntegrationResult;

			CreateDirectoryIfItDoesntExist(result.WorkingDirectory);
			CreateDirectoryIfItDoesntExist(result.ArtifactDirectory);
			result.MarkStartTime();
			try
			{
				result.Modifications = GetModifications(lastResult, result);
				if (result.ShouldRunBuild())
				{
					target.Activity = ProjectActivity.Building;
					target.SourceControl.GetSource(result);
					RunBuild(result);
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex);
				result.ExceptionResult = ex;
			}
			result.MarkEndTime();

			PostBuild(result, resultManager);

			return result;
		}

		private Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			target.Activity = ProjectActivity.CheckingModifications;
			return quietPeriod.GetModifications(target.SourceControl, from, to);
		}

		// ToDo - MR - this is temporary until we know for certain that 'Project.Initialize' will have been called at some point
		private void CreateDirectoryIfItDoesntExist(string directory)
		{
			if (! Directory.Exists(directory))
				Directory.CreateDirectory(directory);
		}

		private void RunBuild(IIntegrationResult result)
		{
			if (result.BuildCondition == BuildCondition.ForceBuild)
				Log.Info("Build forced");

			Log.Info("Building");
			target.Run(result);
			Log.Info("Build complete: " + result.Status);
		}

		private void PostBuild(IIntegrationResult result, IIntegrationResultManager resultManager)
		{
			if (ShouldPublishResult(result))
			{
				HandleProjectLabelling(result);

				// raise event (publishers do their thing in response)
				target.OnIntegrationCompleted(result);

				resultManager.FinishIntegration();
			}
			Log.Info("Integration complete: " + result.EndTime);

			target.Activity = ProjectActivity.Sleeping;
		}

		private bool ShouldPublishResult(IIntegrationResult result)
		{
			IntegrationStatus integrationStatus = result.Status;
			if (integrationStatus == IntegrationStatus.Exception)
			{
				return target.PublishExceptions;
			}
			else
			{
				return integrationStatus != IntegrationStatus.Unknown;
			}
		}

		private void HandleProjectLabelling(IIntegrationResult result)
		{
			target.SourceControl.LabelSourceControl(result);
		}
	}
}