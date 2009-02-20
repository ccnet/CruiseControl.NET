using System;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public interface ISingleProjectDetail 
	{
        string ProjectName { get; }
        /// <summary>
        /// Retrieve the configuration for this project.
        /// </summary>
        /// <remarks>
        /// This is part of the fix for CCNET-1179.
        /// </remarks>
        CCTrayProject Configuration { get; }
        ProjectState ProjectState { get; }

		bool IsConnected { get; }

        string ServerName { get; }	
		ProjectActivity Activity { get; }
		string LastBuildLabel { get; }
		DateTime LastBuildTime { get; }
		DateTime NextBuildTime { get; }
		string ProjectIntegratorState { get; }
		string WebURL { get; }
		string CurrentMessage { get; }
        string CurrentBuildStage { get; }
		
		/// <summary>
		/// Returns TimeSpan.MaxValue if unknown
		/// </summary>
		TimeSpan EstimatedTimeRemainingOnCurrentBuild { get; }
		
		Exception ConnectException { get; }
	}
}
