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
		/// Requests all started projects within the CruiseControl.NET server to stop
		/// </summary>
		void Stop();

		/// <summary>
		/// Terminates the CruiseControl.NET server immediately, stopping all started projects
		/// </summary>
		void Abort();

		/// <summary>
		/// Force the specified project to run a single integration immediately
		/// </summary>
		void ForceBuild(string project);
	}
}
