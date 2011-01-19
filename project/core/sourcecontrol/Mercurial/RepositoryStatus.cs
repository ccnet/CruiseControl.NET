namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Mercurial
{
	/// <summary>
	/// Enumeration to indicate the status of the repository after HgCreateLocalRepository was called.
	/// </summary>
	/// <remarks>Unknown is not currently used.</remarks>
	public enum RepositoryStatus
	{
		Unknown = 0,
		Created = 1,
		AlreadyExists = 2
	}
}
