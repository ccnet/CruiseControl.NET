using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	public interface IIntegrationRunnerTarget : ITask
	{
		ISourceControl SourceControl { get; }
		int ModificationDelaySeconds { get; }

		bool PublishExceptions { get; }

		string WorkingDirectory { get; }
		void OnIntegrationCompleted(IIntegrationResult result);

		// Would like to have this somewhere else really
		ProjectActivity Activity { set; }
	}
}
