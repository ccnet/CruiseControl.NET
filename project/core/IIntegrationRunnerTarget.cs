using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// 	
    /// </summary>
	public interface IIntegrationRunnerTarget : ITask
	{
        /// <summary>
        /// Gets the source control.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		ISourceControl SourceControl { get; }

        /// <summary>
        /// Prebuilds the specified result.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
        void Prebuild(IIntegrationResult result);

        /// <summary>
        /// Publishes the results.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
		void PublishResults(IIntegrationResult result);

		// Would like to have this somewhere else really
        /// <summary>
        /// Gets or sets the activity.	
        /// </summary>
        /// <value>The activity.</value>
        /// <remarks></remarks>
        ProjectActivity Activity { set; get;}

        /// <summary>
        /// Creates the label.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
        void CreateLabel(IIntegrationResult result);

        /// <summary>
        /// Records a source control operation.
        /// </summary>
        /// <param name="operation">The operation to record.</param>
        /// <param name="status">The status of the operation.</param>
        void RecordSourceControlOperation(SourceControlOperation operation, ItemBuildStatus status);

        /// <summary>
        /// Clears messages that are build dependant. Example failing tasks, ...
        /// </summary>
        void ClearNotNeededMessages();

        /// <summary>
        /// Initialises the target for a build.
        /// </summary>
        /// <param name="request">The request.</param>
        void InitialiseForBuild(IntegrationRequest request);
    }
}
