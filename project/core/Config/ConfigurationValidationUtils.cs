namespace ThoughtWorks.CruiseControl.Core.Config
{
    /// <summary>
    /// Helper methods for validating configuration settings.
    /// </summary>
    public static class ConfigurationValidationUtils
    {
        #region Public methods
        #region GenerateResultForProject()
        /// <summary>
        /// Generates a mock result for a project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>The mock <see cref="IIntegrationResult"/>.</returns>
        /// <remarks>
        /// This will only set the basic properties for the result, including project name, working directory and artifect directory.
        /// </remarks>
        public static IIntegrationResult GenerateResultForProject(Project project)
        {
            var result = new IntegrationResult
            {
                ProjectName = project.Name,
                WorkingDirectory = project.WorkingDirectory,
                ArtifactDirectory = project.ArtifactDirectory
            };
            return result;
        }
        #endregion
        #endregion
    }
}
