using System;

namespace ThoughtWorks.CruiseControl.Remote
{
	/// <summary>
	/// Enumeration containing possible conditions for the execution of the
	/// build phase of project integration.
	/// </summary>
	public enum BuildCondition
	{
		/// <summary>
		/// A build should not occur.
		/// </summary>
		NoBuild,

		/// <summary>
		/// A build should occur only if modifications exist.
		/// </summary>
		IfModificationExists,

		/// <summary>
		/// A build should be forced, regardless of whether
		/// modifications exist or not.
		/// </summary>
		ForceBuild
	}
}
