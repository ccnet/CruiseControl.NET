namespace ThoughtWorks.CruiseControl.Remote
{
	/// <summary>
	/// Enumeration of the possible activities of a project under continuous
	/// integration by CruiseControl.NET.
	/// </summary>
	public enum ProjectActivity 
	{
		/// <summary>
		/// CruiseControl.NET is checking for modifications in this project's
		/// source control system.
		/// </summary>
		CheckingModifications,

		/// <summary>
		/// CruiseControl.NET is running the build phase of the project's
		/// integration.
		/// </summary>
		Building,

		/// <summary>
		/// CruiseControl.NET is sleeping, and no activity is being performed
		/// for this project.
		/// </summary>
		Sleeping,
	}
}
