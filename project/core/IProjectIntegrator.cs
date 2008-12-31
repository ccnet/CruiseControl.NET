using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	public interface IProjectIntegrator
	{
		IProject Project { get; }

		string Name { get; }

		/// <summary>
		/// Starts the integration of this project on a separate thread.  If
		/// this integrator has already started, this method causes no action.
		/// </summary>
		void Start();

		/// <summary>
		/// Stops the integration of this project.
		/// </summary>
		void Stop();

		/// <summary>
		/// Waits for the project integrator thread to exit, and joins with it.
		/// </summary>
		void WaitForExit();

		/// <summary>
		/// Aborts the integrator thread immediately.
		/// </summary>
		void Abort();

		/// <summary>
		/// Gets a value indicating whether this project integrator is currently
		/// running.
		/// </summary>
		bool IsRunning { get; }

		/// <summary>
		/// Gets a value indicating the project integrator's current state.
		/// </summary>
		ProjectIntegratorState State { get; }

		IIntegrationRepository IntegrationRepository { get; }

		/// <summary>
		/// For invocation by a force build publisher or having the exe config running a project
		/// when CC.Net first starts.
		/// </summary>
        /// <param name="enforcerName">ID of program/person forcing the build</param>
        void ForceBuild(string enforcerName);
		
		/// <summary>
		/// Aborts the build of the selected project.
		/// </summary>
		void AbortBuild(string enforcerName);

		/// <summary>
		/// For "Force" requests such as by CCTray or the Web GUI.
		/// </summary>
		/// <param name="request">Request contains the source such as the user id.</param>
		void Request(IntegrationRequest request);

		/// <summary>
		/// Cancel a pending project integration request from the integration queue.
		/// </summary>
		void CancelPendingRequest();
	}
}
