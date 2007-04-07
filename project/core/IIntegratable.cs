using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	public interface IIntegratable
	{
		/// <summary>
		/// Runs an integration of this project.
		/// </summary>
		/// <param name="request"></param>
		/// <returns>The result of the integration, or null if no integration took place.</returns>
		IIntegrationResult Integrate(IntegrationRequest request);

		/// <summary>
		/// Notification that project should enter a pending state due to being queued.
		/// </summary>
		void NotifyPendingState();

		/// <summary>
		/// Notification of last project exiting the integration queue and hence can return to sleeping state.
		/// </summary>
		void NotifySleepingState();
	}
}
