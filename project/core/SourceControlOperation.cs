namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// A type of source control operation.
    /// </summary>
    public enum SourceControlOperation
    {
        /// <summary>
        /// Checking for modifications.
        /// </summary>
        CheckForModifications,
        /// <summary>
        /// Getting the source code.
        /// </summary>
        GetSource,
    }
}
