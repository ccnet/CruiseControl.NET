
namespace ThoughtWorks.CruiseControl.Remote
{
	/// <summary>
	/// Enumeration of possible states for a project's integrator.
	/// </summary>
	public enum ProjectIntegratorState
	{
		/// <summary>
		/// Integration is running, or will run when appropriate.  This value
		/// indicates that the project integrator is alive, whether currently
		/// integrating or not.
		/// </summary>
		Running,

		/// <summary>
		/// The project integrator will stop when the current integration has
		/// completed.
		/// </summary>
		Stopping,

		/// <summary>
		/// The project integrator has stopped integration.  The project will
		/// not be integrated until the integrator is started again.
		/// </summary>
		Stopped
	}
}
