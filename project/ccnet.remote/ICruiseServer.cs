using System;

namespace tw.ccnet.remote
{
	public interface ICruiseServer
	{
		/// <summary>
		/// Launches the CruiseControl.NET server and starts all project schedules it contains
		/// </summary>
		void Start();

		/// <summary>
		/// Terminates the CruiseControl.NET server, stopping all started projects
		/// </summary>
		void Stop();

		/// <summary>
		/// Force the specified project to run a single integration immediately
		/// </summary>
		void ForceBuild(string project);
	}
}
