using System;
using System.Collections;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	public interface IIntegrationResult
	{
		string ProjectName { get; }
		BuildCondition BuildCondition { get; }
		string WorkingDirectory { get; }
		string Label { get; set; }
		IntegrationStatus Status { get; set; }
		IntegrationStatus LastIntegrationStatus { get; }
		DateTime StartTime { get; }
		DateTime EndTime { get; }
		TimeSpan TotalIntegrationTime { get; }
		IList TaskResults { get; }
		string Output { get; set; }
		DateTime LastModificationDate { get; }
		int LastChangeNumber { get; }
		Modification[] Modifications { get; set; }
		Exception ExceptionResult { get; set; }
		bool IsInitial();
		bool HasModifications();
		bool Working { get; }
		bool Failed { get; }
		bool Fixed { get; }
		bool Succeeded { get; }
		void MarkStartTime();
		void MarkEndTime();
	}
}