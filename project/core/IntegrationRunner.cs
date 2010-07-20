
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

        /// <summary>
        /// Starts a new integration result.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// The new <see cref="IIntegrationResult"/>.
        /// </returns>
        public IIntegrationResult StartNewIntegration(IntegrationRequest request)
        {
            var result = resultManager.StartNewIntegration(request);

            // Copy any parameters to the result
            if ((request.BuildValues != null) && (request.BuildValues.Count > 0))
            {
                result.Parameters.AddRange(
                    NameValuePair.FromDictionary(request.BuildValues));
            }

            result.MarkStartTime();
            this.GenerateSystemParameterValues(result);
            return result;
        }

        public IIntegrationResult Integrate(IntegrationRequest request)
        {
            Log.Trace();

            

            this.target.InitialiseForBuild(request);
            var result = this.StartNewIntegration(request);

            IIntegrationResult lastResult = resultManager.LastIntegrationResult;
            CreateDirectoryIfItDoesntExist(result.WorkingDirectory);
            CreateDirectoryIfItDoesntExist(result.ArtifactDirectory);

            Log.Trace("Getting Modifications for project {0}", result.ProjectName);
            try
            {
                result.Modifications = GetModifications(lastResult, result);
            }
            catch (Exception error)
            {
                result.SourceControlError = error;
                result.LastBuildStatus = lastResult.HasSourceControlError ? lastResult.LastBuildStatus : lastResult.Status;
                Log.Warning(string.Format("Source control failure (GetModifications): {0}", error.Message));
                if (request.PublishOnSourceControlException)
                {
                    result.ExceptionResult = error;
                    CompleteIntegration(result);
                }
            }

            var runBuild = false;
            try
            {
                // Check whether a build should be performed
                runBuild = (result.SourceControlError == null) && result.ShouldRunBuild();

                if (runBuild)
                {
                    Log.Info("Building: " + request);

                    target.ClearNotNeededMessages();


                    // hack : otherwise all labellers(CCnet and custom) should be altered, better do this in 1 place
                    // labelers only increase version if PREVIOUS result was ok
                    // they should also increase version if previous was exception, and the new
                    // build got past the getmodifications

                    Log.Trace("Creating Label for project {0}", result.ProjectName);

                    if (result.LastIntegrationStatus == IntegrationStatus.Exception)
                    {
                        IntegrationSummary isExceptionFix = new IntegrationSummary(IntegrationStatus.Success, result.LastIntegration.Label, result.LastIntegration.LastSuccessfulIntegrationLabel, result.LastIntegration.StartTime);
                        IIntegrationResult irExceptionFix = new IntegrationResult(result.ProjectName, result.WorkingDirectory, result.ArtifactDirectory, result.IntegrationRequest, isExceptionFix);
                        irExceptionFix.Modifications = result.Modifications;

                        target.CreateLabel(irExceptionFix);
                        result.Label = irExceptionFix.Label;
                    }
                    else
                    {
                        target.CreateLabel(result);
                    }

                    Log.Trace("Running tasks of project {0}", result.ProjectName);
                    this.GenerateSystemParameterValues(result); 
                    Build(result);
                }
                else if (lastResult.HasSourceControlError)
                {
                    // Reset to the last valid status
                    result.Status = lastResult.LastBuildStatus;
                    resultManager.FinishIntegration();
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Exception caught: " + ex.Message);
                result.ExceptionResult = ex;
            }
            finally
            {
                if (runBuild)
                {
                    CompleteIntegration(result);
                }
            }

            this.target.Activity = ProjectActivity.Sleeping;
            return result;
        }

        #region GenerateSystemParameterValues()
        /// <summary>
        /// Generates parameter values from the incoming request values.
        /// </summary>
        /// <param name="result">The result.</param>
        public void GenerateSystemParameterValues(IIntegrationResult result)
        {
            var props = result.IntegrationProperties;
            foreach (var property in props.Keys)
            {
                // Generate the build value
                var key = string.Format("${0}", property);
                var value = (props[property] ?? string.Empty).ToString();
                result.IntegrationRequest.BuildValues[key] = value;

                // Add to the parameters
                var namedValue = new NameValuePair(key, value);
                if (result.Parameters.Contains(namedValue))
                {
                    // Replace an existing value
                    var index = result.Parameters.IndexOf(namedValue);
                    result.Parameters[index] = namedValue;
                }
                else
                {
                    // Add a new value
                    result.Parameters.Add(namedValue);
                }
            }
        }
        #endregion

        /// <summary>
        /// Completes an integration.
        /// </summary>
        /// <param name="result">The integration result.</param>
        private void CompleteIntegration(IIntegrationResult result)
        {
            result.MarkEndTime();
            PostBuild(result);
            Log.Info(string.Format("Integration complete: {0} - {1}", result.Status, result.EndTime));
        }

        private Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
        {
            target.Activity = ProjectActivity.CheckingModifications;
            to.BuildProgressInformation.SignalStartRunTask("Getting source ... ");

            target.RecordSourceControlOperation(SourceControlOperation.CheckForModifications, ItemBuildStatus.Running);

            bool success = false;

            try
            {
                Modification[] modifications = quietPeriod.GetModifications(target.SourceControl, from, to);
                success = true;
                return modifications;
            }
            finally
            {
                target.RecordSourceControlOperation(SourceControlOperation.CheckForModifications,
                    success ? ItemBuildStatus.CompletedSuccess : ItemBuildStatus.CompletedFailed);
            }
        }

        private void Build(IIntegrationResult result)
        {
            target.Activity = ProjectActivity.Building;
            target.Prebuild(result);
            if (!result.Failed)
            {
                bool success = false;

                target.RecordSourceControlOperation(SourceControlOperation.GetSource, ItemBuildStatus.Running);

                try
                {
                    target.SourceControl.GetSource(result);
                    success = true;
                }
                finally
                {
                    target.RecordSourceControlOperation(SourceControlOperation.GetSource,
                        success ? ItemBuildStatus.CompletedSuccess : ItemBuildStatus.CompletedFailed);
                }

                target.Run(result);

                target.SourceControl.LabelSourceControl(result);
            }
        }

        public virtual void PostBuild(IIntegrationResult result)
        {
            resultManager.FinishIntegration();

            Log.Trace("Running publishers");
            target.PublishResults(result);
        }

        private static void CreateDirectoryIfItDoesntExist(string directory)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }
    }
}
