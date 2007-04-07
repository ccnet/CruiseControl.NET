namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public interface IPollable
	{
		void Poll();
		/// <summary>
		/// Invoked every time a poll is started (first time and each time after being stopped).
		/// Provides an opportunity for the IPollable instance to reset if required.
		/// </summary>
		void OnPollStarting();
	}
}
