using System;
using System.Collections;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	public interface IIntegrationResult
	{
		string ProjectName { get; }
		string ProjectUrl { get; set;}

		BuildCondition BuildCondition { get; }
		string WorkingDirectory { get; set; }
		string Label { get; set; }
		IntegrationStatus Status { get; set; }
		DateTime StartTime { get; set; }
		DateTime EndTime { get; }
		TimeSpan TotalIntegrationTime { get; }
		string ArtifactDirectory { get; set;}
		bool Failed { get; }
		bool Fixed { get; }
		bool Succeeded { get; }
		void MarkStartTime();
		void MarkEndTime();
		
		string LastSuccessfulIntegrationLabel { get; }
		IntegrationStatus LastIntegrationStatus { get; }
		DateTime LastModificationDate { get; }
		int LastChangeNumber { get; }

		IList TaskResults { get; }
		Modification[] Modifications { get; set; }
		Exception ExceptionResult { get; set; }
		string TaskOutput { get; }

		void AddTaskResult(string result);
		void AddTaskResult(ITaskResult result);
		bool IsInitial();
		bool HasModifications();
		bool ShouldRunBuild();
		string BaseFromArtifactsDirectory(string pathToBase);
		string BaseFromWorkingDirectory(string pathToBase);
		IDictionary IntegrationProperties { get; }

		IntegrationState LastIntegration { get; }

		IntegrationState Integration { get; }
	}
}