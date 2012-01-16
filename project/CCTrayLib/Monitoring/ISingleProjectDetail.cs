using System;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public interface ISingleProjectDetail 
	{
        string ProjectName { get; }
        string ProjectDescription { get; }

        /// <summary>
        /// Retrieve the configuration for this project.
        /// </summary>
        /// <remarks>
        /// This is part of the fix for CCNET-1179.
        /// </remarks>
        CCTrayProject Configuration { get; }
        ProjectState ProjectState { get; }

		bool IsConnected { get; }
        bool ShowForceBuildButton { get; }
        bool ShowStartStopButton { get; }

        string QueueName { get; }
        int QueuePriority { get; }

        string ServerName { get; }	

        string Category { get; }
		ProjectActivity Activity { get; }
		string LastBuildLabel { get; }
		DateTime LastBuildTime { get; }
		DateTime NextBuildTime { get; }
		string ProjectIntegratorState { get; }
		string WebURL { get; }
		string CurrentMessage { get; }
        string CurrentBuildStage { get; }
        Message[] Messages { get; }
		

		/// <summary>
		/// Returns TimeSpan.MaxValue if unknown
		/// </summary>
		TimeSpan EstimatedTimeRemainingOnCurrentBuild { get; }
		
		Exception ConnectException { get; }
	}
}
