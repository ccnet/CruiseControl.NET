using System;

namespace tw.ccnet.remote
{
	/// <summary>
	/// Enumeration of possible summations following the integration of a project.
	/// </summary>
	public enum IntegrationStatus
	{
		/// <summary>
		/// Indicates the project's integration was successful.  Compilation succeeded,
		/// and any tests passed.
		/// </summary>
		Success,

		/// <summary>
		/// Indicates the project's integration failed.  Either the compilation or tests failed.
		/// </summary>
		Failure,

		/// <summary>
		/// Indicated CruiseControl.NET experienced exceptional circumstances during the
		/// integration of the project.
		/// </summary>
		Exception,

		/// <summary>
		/// Indicates the state of the most recent integration is unknown.  Perhaps no integration
		/// has yet occurred.
		/// </summary>
		Unknown
	}
}
