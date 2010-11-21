namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// 	
    /// </summary>
	public interface IStarTeamRegExProvider
	{
        /// <summary>
        /// Gets the folder reg ex.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		string FolderRegEx { get; }
        /// <summary>
        /// Gets the file reg ex.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		string FileRegEx { get; }
        /// <summary>
        /// Gets the file history reg ex.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		string FileHistoryRegEx { get; }
	}
}
