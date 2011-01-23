namespace ThoughtWorks.CruiseControl.Remote
{
	/// <remarks>
	/// Enumeration of possible states for the CruiseControl.NET server.
	/// </remarks>
	public enum CruiseControlStatus
	{
		/// <summary>
		/// The server is not running.
		/// </summary>
		Stopped,

		/// <summary>
		/// The server is running.
		/// </summary>
		Running,

		/// <summary>
		/// The server is scheduled to be stopped.
		/// </summary>
		WillBeStopped,

		/// <summary>
		/// The server's state is unknown.
		/// </summary>
		Unknown
	}
}
