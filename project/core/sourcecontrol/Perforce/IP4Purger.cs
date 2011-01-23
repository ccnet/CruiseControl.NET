namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Perforce
{
    /// <summary>
    /// 	
    /// </summary>
	public interface IP4Purger
	{
        /// <summary>
        /// Purges the specified p4.	
        /// </summary>
        /// <param name="p4">The p4.</param>
        /// <param name="workingDirectory">The working directory.</param>
        /// <remarks></remarks>
		void Purge(P4 p4, string workingDirectory);
	}
}
