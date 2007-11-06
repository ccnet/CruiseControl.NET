using System;
using System.Collections;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	public interface IIntegrationResult
	{
		// Project configuration properties
		string ProjectName { get; }
		string ProjectUrl { get; set;}
		string WorkingDirectory { get; set; }
		string ArtifactDirectory { get; set;}
		string BaseFromArtifactsDirectory(string pathToBase);
		string BaseFromWorkingDirectory(string pathToBase);


        string BuildLogDirectory { get; set;}   //rw

		// Current integration state
		BuildCondition BuildCondition { get; }
		string Label { get; set; }
		IntegrationStatus Status { get; set; }
		DateTime StartTime { get; set; }
		DateTime EndTime { get; }
		TimeSpan TotalIntegrationTime { get; }
		bool Failed { get; }
		bool Fixed { get; }
		bool Succeeded { get; }
		void MarkStartTime();
		void MarkEndTime();
		bool IsInitial();
		IntegrationRequest IntegrationRequest { get; }
        string ListenerFile { get; }

		// Last integration state
		IntegrationStatus LastIntegrationStatus { get; }
		DateTime LastModificationDate { get; }
		int LastChangeNumber { get; }
		IntegrationSummary LastIntegration { get; }

		// Last successful integration state
		string LastSuccessfulIntegrationLabel { get; }

		// Current integration artifacts
		IList TaskResults { get; }
		Modification[] Modifications { get; set; }
		Exception ExceptionResult { get; set; }
		string TaskOutput { get; }
		void AddTaskResult(string result);
		void AddTaskResult(ITaskResult result);
		bool HasModifications();
		bool ShouldRunBuild();

		IDictionary IntegrationProperties { get; }
	}
}