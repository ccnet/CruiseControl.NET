using System;
using System.IO;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	public class IntegrationRunner : IIntegratable
	{
		public IIntegrationRunnerTarget target;
		private readonly IIntegrationResultManager resultManager;

		public IntegrationRunner(IIntegrationResultManager resultManager, IIntegrationRunnerTarget target)
		{
			this.target = target;
			this.resultManager = resultManager;
		}

		// ToDo - this is all still a bit messy - could do with cleaning up
		public IIntegrationResult RunIntegration(BuildCondition buildCondition)
		{
			IIntegrationResult result = resultManager.StartNewIntegration(buildCondition);
			IIntegrationResult lastResult = resultManager.LastIntegrationResult;

			ISourceControl sourceControl = target.SourceControl;

			result.MarkStartTime();
			try
			{
				result.Modifications = GetSourceModifications(sourceControl, result, lastResult);
				if (result.ShouldRunBuild(target.ModificationDelaySeconds))
				{
					CreateWorkingDirectoryIfItDoesntExist();
					CreateTemporaryLabelIfNeeded(sourceControl);
					sourceControl.GetSource(result);
					RunBuild(result);
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex);
				result.ExceptionResult = ex;
				result.Status = IntegrationStatus.Exception;
			}
			result.MarkEndTime();

			PostBuild(result,resultManager);

			return result;
		}

		private Modification[] GetSourceModifications(ISourceControl sourceControl, IIntegrationResult result, IIntegrationResult lastResult)
		{
			target.Activity = ProjectActivity.CheckingModifications;
			Modification[] modifications = sourceControl.GetModifications(lastResult.StartTime, result.StartTime);
			Log.Info(GetModificationsDetectedMessage(modifications));
			return modifications;
		}

		private string GetModificationsDetectedMessage(Modification[] modifications)
		{
			switch (modifications.Length)
			{
				case 0:
					return "No modifications detected.";
				case 1:
					return "1 modification detected.";
				default:
					return string.Format("{0} modifications detected.", modifications.Length);
			}
		}

		// ToDo - MR - this is temporary until we know for certain that 'Project.Initialize' will have been called at some point
		private void CreateWorkingDirectoryIfItDoesntExist()
		{
			if (! Directory.Exists(target.WorkingDirectory))
				Directory.CreateDirectory(target.WorkingDirectory);
		}

		internal void CreateTemporaryLabelIfNeeded(ISourceControl sourceControl)
		{
			if (sourceControl is ITemporaryLabeller)
			{
				((ITemporaryLabeller) sourceControl).CreateTemporaryLabel();
			}
		}

		private void RunBuild(IIntegrationResult result)
		{
			target.Activity = ProjectActivity.Building;
			if (result.BuildCondition == BuildCondition.ForceBuild)
				Log.Info("Build forced");

			Log.Info("Building");
			target.Run(result);
			Log.Info("Build complete: " + result.Status);
		}

		private void PostBuild(IIntegrationResult result, IIntegrationResultManager resultManager)
		{
			if (ShouldPublishException(result))
			{
				// Shouldn't this be outside the if?
				HandleProjectLabelling(result);

				// raise event (publishers do their thing in response)
				target.OnIntegrationCompleted(result);

				resultManager.FinishIntegration();
			}
			Log.Info("Integration complete: " + result.EndTime);

			target.Activity = ProjectActivity.Sleeping;
		}

		private bool ShouldPublishException(IIntegrationResult result)
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
			if (result.Succeeded)
			{
				// This call to result.Label unnecessary
				target.SourceControl.LabelSourceControl(result.Label, result);
			}
			else
			{
				if (target.SourceControl is ITemporaryLabeller)
				{
					((ITemporaryLabeller) target.SourceControl).DeleteTemporaryLabel();
				}
			}
		}
	}
}
