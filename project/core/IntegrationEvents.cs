using System;

namespace ThoughtWorks.CruiseControl.Core
{
	public delegate void IntegrationCompletedEventHandler(object sender, IntegrationCompletedEventArgs e);

	public interface IIntegrationCompletedEventHandler
	{
		/// <summary>
		/// Gets a delegate for the IntegrationCompleted event.
		/// </summary>
		IntegrationCompletedEventHandler IntegrationCompletedEventHandler
		{
			get;
		}
	}

	/// <summary>
	/// Event arguments for completed integrations.
	/// </summary>
	public class IntegrationCompletedEventArgs : EventArgs
	{
		/// <summary>
		/// The result of the completed integration.
		/// </summary>
		public IntegrationResult IntegrationResult;

		public IntegrationCompletedEventArgs(IntegrationResult result)
		{
			IntegrationResult = result;
		}
	}

}
