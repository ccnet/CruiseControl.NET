using System;

namespace ThoughtWorks.CruiseControl.Remote
{
	/// <remarks>
	/// Remote Interface to CruiseControl.NET.
	/// </remarks>
	public interface ICruiseManager
	{
		/// <summary>
		/// Starts a stopped instance of CruiseControl.NET
		/// </summary>
		void StartCruiseControl();

		/// <summary>
		/// Attempts to stop CruiseControl.NET after it completes its current task.
		/// </summary>
		void StopCruiseControl();

		/// <summary>
		/// Stops CruiseControl.NET immediately
		/// </summary>
		void StopCruiseControlNow();

		/// <summary>
		/// Gets the current status of the CruiseControl.NET server.
		/// </summary>
		CruiseControlStatus GetStatus();

		/// <summary>
		/// Gets information about the last build status, current activity and project name.
		/// </summary>
		ProjectStatus GetProjectStatus();

		/// <summary>
		/// Forces a build for the named project.
		/// </summary>
		/// <param name="projectName"></param>
		void ForceBuild(string projectName);

		/// <summary>
		/// Gets and sets the configuration string for this CruiseControl.NET instance.
		/// </summary>
        string Configuration
		{
			get;
			set;
		}
	}
}
