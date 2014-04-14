using System;
using System.Collections;
using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Remote;
using System.Xml;

namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// 	
    /// </summary>
    public interface IIntegrationResult
    {
        // Project configuration properties
        /// <summary>
        /// Gets the name of the project.	
        /// </summary>
        /// <value>The name of the project.</value>
        /// <remarks></remarks>
        string ProjectName { get; }
        /// <summary>
        /// Gets or sets the project URL.	
        /// </summary>
        /// <value>The project URL.</value>
        /// <remarks></remarks>
        string ProjectUrl { get; set; }
        /// <summary>
        /// Gets or sets the working directory.	
        /// </summary>
        /// <value>The working directory.</value>
        /// <remarks></remarks>
        string WorkingDirectory { get; set; }
        /// <summary>
        /// Gets or sets the artifact directory.	
        /// </summary>
        /// <value>The artifact directory.</value>
        /// <remarks></remarks>
        string ArtifactDirectory { get; set; }
        /// <summary>
        /// Bases from artifacts directory.	
        /// </summary>
        /// <param name="pathToBase">The path to base.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        string BaseFromArtifactsDirectory(string pathToBase);
        /// <summary>
        /// Bases from working directory.	
        /// </summary>
        /// <param name="pathToBase">The path to base.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        string BaseFromWorkingDirectory(string pathToBase);
        /// <summary>
        /// Gets or sets the build log directory.	
        /// </summary>
        /// <value>The build log directory.</value>
        /// <remarks></remarks>
        string BuildLogDirectory { get; set; }

        /// <summary>
        ///  The parameters used.
        /// </summary>
        List<NameValuePair> Parameters { get; set; }

        // Current integration state
        /// <summary>
        /// Gets the build condition.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        BuildCondition BuildCondition { get; }
        /// <summary>
        /// Gets or sets the label.	
        /// </summary>
        /// <value>The label.</value>
        /// <remarks></remarks>
        string Label { get; set; }
        /// <summary>
        /// Gets or sets the status.	
        /// </summary>
        /// <value>The status.</value>
        /// <remarks></remarks>
        IntegrationStatus Status { get; set; }
        /// <summary>
        /// Gets or sets the start time.	
        /// </summary>
        /// <value>The start time.</value>
        /// <remarks></remarks>
        DateTime StartTime { get; set; }
        /// <summary>
        /// Gets the end time.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        DateTime EndTime { get; }
        /// <summary>
        /// Gets the total integration time.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        TimeSpan TotalIntegrationTime { get; }
        /// <summary>
        /// Gets the failed.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        bool Failed { get; }
        /// <summary>
        /// Gets the fixed.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        bool Fixed { get; }
        /// <summary>
        /// Gets the succeeded.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        bool Succeeded { get; }
        /// <summary>
        /// Marks the start time.	
        /// </summary>
        /// <remarks></remarks>
        void MarkStartTime();
        /// <summary>
        /// Marks the end time.	
        /// </summary>
        /// <remarks></remarks>
        void MarkEndTime();
        /// <summary>
        /// Determines whether this instance is initial.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        bool IsInitial();
        /// <summary>
        /// Gets the integration request.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        IntegrationRequest IntegrationRequest { get; }

        /// <summary>
        /// Gets the last integration status.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        IntegrationStatus LastIntegrationStatus { get; }
        // Users who contributed modifications to a series of failing builds:
        /// <summary>
        /// Gets the failure users.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        ArrayList FailureUsers { get; }             // This should really be a Set but sets are not available in .NET 1.1
        // Name of the tasks which contributed to the current build failure:
        /// <summary>
        /// Gets the failure tasks.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        ArrayList FailureTasks { get; }             // This should really be a Set but sets are not available in .NET 1.1
        /// <summary>
        /// Gets the last modification date.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        DateTime LastModificationDate { get; }
        /// <summary>
        /// Gets the last change number.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        string LastChangeNumber { get; }
        /// <summary>
        /// Gets the last integration.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        IntegrationSummary LastIntegration { get; }

        // Last successful integration state
        /// <summary>
        /// Gets the last successful integration label.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        string LastSuccessfulIntegrationLabel { get; }

        // Current integration artifacts
        /// <summary>
        /// Gets the task results.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        IList TaskResults { get; }
        /// <summary>
        /// Gets or sets the modifications.	
        /// </summary>
        /// <value>The modifications.</value>
        /// <remarks></remarks>
        Modification[] Modifications { get; set; }
        /// <summary>
        /// Gets or sets the exception result.	
        /// </summary>
        /// <value>The exception result.</value>
        /// <remarks></remarks>
        Exception ExceptionResult { get; set; }
        /// <summary>
        /// Gets the task output.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        string TaskOutput { get; }
        /// <summary>
        /// Adds the task result.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
        void AddTaskResult(string result);
        /// <summary>
        /// Adds the task result.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
        void AddTaskResult(ITaskResult result);
        /// <summary>
        /// Determines whether this instance has modifications.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        bool HasModifications();
        /// <summary>
        /// Shoulds the run build.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        bool ShouldRunBuild();

        /// <summary>
        /// Gets the build id, an id that is unique for each build.
        /// </summary>
        Guid BuildId { get; set; }


        /// <summary>
        /// Any error that occurred during the get modifications stage of source control.
        /// </summary>
        /// <remarks>
        /// If there is no error then this property will be null.
        /// </remarks>
        Exception SourceControlError { get; set; }

        #region HasSourceControlError
        /// <summary>
        /// Gets or sets a value indicating whether there was a source control error.
        /// </summary>
        bool HasSourceControlError { get; }
        #endregion

        #region LastBuildStatus
        /// <summary>
        /// The last status from a build that progressed pass any source control checks.
        /// </summary>
        IntegrationStatus LastBuildStatus { get; set; }
        #endregion

        /// <summary>
        /// Gets the integration properties.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        IDictionary IntegrationProperties { get; }

        /// <summary>
        /// Gets or sets the custom integration properties.
        /// CCNet code should NOT use this.
        /// These can be used by custom tasks to pass variables around tasks / publishers ...
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        List<NameValuePair> CustomIntegrationProperties { get; set; }

        /// <summary>
        /// Adds or updates the CustomIntegrationProperties with the passed value
        /// </summary>
        /// <param name="nv"></param>
        void UpsertCustomIntegrationProperty(NameValuePair nv);

        /// <summary>
        /// Retrieves the NameValuePair with the passed name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        NameValuePair GetCustomIntegrationProperty(string name);

        /// <summary>
        /// Gets the build progress information.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        Util.BuildProgressInformation BuildProgressInformation { get; }

        #region Clone()
        /// <summary>
        /// Clones this integration result.
        /// </summary>
        /// <returns>Returns a clone of the result.</returns>
        IIntegrationResult Clone();
        #endregion

        #region Merge()
        /// <summary>
        /// Merges another result.
        /// </summary>
        /// <param name="value">The result to merge.</param>
        void Merge(IIntegrationResult value);
        #endregion

        #region SourceControlData
        /// <summary>
        /// Extended source control data.
        /// </summary>
        /// <remarks>
        /// It is up to the individual source control providers to decide what to store in here.
        /// </remarks>
        List<NameValuePair> SourceControlData { get; }
        #endregion
    }
}
