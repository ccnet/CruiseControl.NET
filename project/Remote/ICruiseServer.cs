using System;

namespace ThoughtWorks.CruiseControl.Remote
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
		/// Wait for CruiseControl server to finish executing
		/// </summary>
		void WaitForExit();

		/// <summary>
		/// Retrieve CruiseManager interface for the server
		/// </summary>
		ICruiseManager CruiseManager { get; }
	}
}
