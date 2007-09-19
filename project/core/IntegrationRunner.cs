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

        public IIntegrationResult Integrate(IntegrationRequest request)
        {
            IIntegrationResult result = resultManager.StartNewIntegration(request);
            IIntegrationResult lastResult = resultManager.LastIntegrationResult;

            CreateDirectoryIfItDoesntExist(result.WorkingDirectory);
            CreateDirectoryIfItDoesntExist(result.ArtifactDirectory);
            result.MarkStartTime();
            result.Modifications = GetModifications(lastResult, result);
            if (result.ShouldRunBuild())
            {
                Log.Info("Building: " + request);
                Build(result);
                PostBuild(result);
                Log.Info(string.Format("Integration complete: {0} - {1}", result.Status, result.EndTime));
            }
            target.Activity = ProjectActivity.Sleeping;
            return result;
        }

        private Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
        {
            target.Activity = ProjectActivity.CheckingModifications;
            return quietPeriod.GetModifications(target.SourceControl, from, to);
        }

        private void Build(IIntegrationResult result)
        {
            target.Activity = ProjectActivity.Building;
            try
            {
                target.Prebuild(result);
                if (!result.Failed) 
                {                                    
                    target.SourceControl.GetSource(result);
                    target.Run(result);
                    target.SourceControl.LabelSourceControl(result);
                }
            }
            catch (Exception ex)
            {
                result.ExceptionResult = ex;
            }
            result.MarkEndTime();
        }

        private void PostBuild(IIntegrationResult result)
        {
            target.PublishResults(result);
            resultManager.FinishIntegration();
        }

        private static void CreateDirectoryIfItDoesntExist(string directory)
        {
            if (! Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }
    }
}