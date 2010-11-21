namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Perforce
{
    /// <summary>
    /// 	
    /// </summary>
	public interface IP4Initializer
	{
        /// <summary>
        /// Initializes the specified p4.	
        /// </summary>
        /// <param name="p4">The p4.</param>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="workingDirectory">The working directory.</param>
        /// <remarks></remarks>
		void Initialize(P4 p4, string projectName, string workingDirectory);
	}
}
