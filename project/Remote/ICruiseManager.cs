using System;

namespace ThoughtWorks.CruiseControl.Remote
{
	/// <remarks>
	/// Remote Interface to CruiseControl.NET.
	/// </remarks>
	public interface ICruiseManager
	{
		/// <summary>
		/// Gets the current status of the CruiseControl.NET server.
		/// </summary>
		CruiseControlStatus GetStatus();

		/// <summary>
		/// Gets information about the last build status, current activity and project name.
		/// for all projects on a cruise server
		/// </summary>
		ProjectStatus [] GetProjectStatus();

		/// <summary>
		/// Forces a build for the named project.
		/// </summary>
		/// <param name="projectName"></param>
		void ForceBuild(string projectName);
	}
}
