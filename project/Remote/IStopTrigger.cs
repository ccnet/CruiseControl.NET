namespace ThoughtWorks.CruiseControl.Remote
{
	public interface IStopProjectTrigger
	{
		/// <summary>
		/// Whether we should stop integrating
		/// </summary>
		/// <returns>True if integration should be stopped, else false.</returns>
		bool ShouldStopProject();

		/// <summary>
		/// Notifies the schedule that an integration has completed.
		/// </summary>
		void IntegrationCompleted();
	}
}
