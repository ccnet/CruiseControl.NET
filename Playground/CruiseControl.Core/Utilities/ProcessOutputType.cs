namespace CruiseControl.Core.Utilities
{
    /// <summary>
    /// The type of output from a process.
    /// </summary>
    public enum ProcessOutputType
    {
        /// <summary>
        /// The output type is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The output was from standard out.
        /// </summary>
        StandardOutput = 1,

        /// <summary>
        /// The output was from error.
        /// </summary>
        ErrorOutput = 2
    }
}
