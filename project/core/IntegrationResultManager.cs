#pragma warning disable 1591
using System;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
    public class IntegrationResultManager : IIntegrationResultManager
    {
        private readonly Project project;
        private IIntegrationResult lastResult;
        private IIntegrationResult currentIntegration;
        private IntegrationSummary lastIntegration;

        public IntegrationResultManager(Project project)
        {
            this.project = project;
        }

        public IIntegrationResult LastIntegrationResult
        {
            get
            {
                // lazy loads because StateManager needs to be populated from configuration
                if (lastResult == null)
                {
                    lastResult = CurrentIntegration;
                }
                return lastResult;
            }
        }

        public IntegrationSummary LastIntegration
        {
            get
            {
                if (lastIntegration == null)
                {
                    lastIntegration = ConvertResultIntoSummary(LastIntegrationResult);
                }
                return lastIntegration;
            }
        }

        public IIntegrationResult CurrentIntegration
        {
            get
            {
                if (currentIntegration == null)
                {
                    if (project.StateManager.HasPreviousState(project.Name))
                        currentIntegration = project.StateManager.LoadState(project.Name);
                    else
                        currentIntegration = IntegrationResult.CreateInitialIntegrationResult(project.Name, project.WorkingDirectory, project.ArtifactDirectory);
                }
                return currentIntegration;
            }
        }

        public IIntegrationResult StartNewIntegration(IntegrationRequest request)
        {
            IntegrationResult newResult = new IntegrationResult(project.Name, project.WorkingDirectory, project.ArtifactDirectory, request, LastIntegration);
            newResult.ArtifactDirectory = project.ArtifactDirectory;
            newResult.ProjectUrl = project.WebURL;
            NameValuePair.Copy(LastIntegrationResult.SourceControlData, newResult.SourceControlData);

            return currentIntegration = newResult;
        }

        public void FinishIntegration()
        {
            try
            {
                // Save users who may have broken integration so we can email them until it's fixed
                if (currentIntegration.Status == IntegrationStatus.Failure)
                {
                    // Build is broken - add any users who contributed modifications to the existing list of users
                    // who have contributed modifications to failing builds.
                    foreach (Modification modification in currentIntegration.Modifications)
                    {
                        if (!currentIntegration.FailureUsers.Contains(modification.UserName))
                            currentIntegration.FailureUsers.Add(modification.UserName);
                    }
                }
                project.StateManager.SaveState(currentIntegration);
            }
            catch (Exception ex)
            {
                // swallow exception???
                Log.Error("Unable to save integration result: " + ex);
            }
            lastResult = currentIntegration;
            lastIntegration = ConvertResultIntoSummary(currentIntegration);
        }

        private static IntegrationSummary ConvertResultIntoSummary(IIntegrationResult integration)
        {
            string lastSuccessfulIntegrationLabel = (integration.Succeeded) ? integration.Label : integration.LastSuccessfulIntegrationLabel;
            IntegrationSummary newSummary = new IntegrationSummary(integration.Status, integration.Label, lastSuccessfulIntegrationLabel, integration.StartTime);
            newSummary.FailureUsers = integration.FailureUsers;
            return newSummary;
        }
    }
}
