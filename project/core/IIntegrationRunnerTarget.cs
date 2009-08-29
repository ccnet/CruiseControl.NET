using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.Core
{
	public interface IIntegrationRunnerTarget : ITask
	{
		ISourceControl SourceControl { get; }

        void Prebuild(IIntegrationResult result);

		void PublishResults(IIntegrationResult result);

		// Would like to have this somewhere else really
        ProjectActivity Activity { set; get;}

        void CreateLabel(IIntegrationResult result);

        /// <summary>
        /// Records a source control operation.
        /// </summary>
        /// <param name="operation">The operation to record.</param>
        /// <param name="status">The status of the operation.</param>
        void RecordSourceControlOperation(SourceControlOperation operation, ItemBuildStatus status);
    }
}
