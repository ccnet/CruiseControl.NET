namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// Defines the build status of an item.
    /// </summary>
    public enum ItemBuildStatus
    {
        /// <summary>
        /// The item is pending.
        /// </summary>
        Pending,
        /// <summary>
        /// The item is currently running.
        /// </summary>
        Running,
        /// <summary>
        /// The item has completed running with a success status.
        /// </summary>
        CompletedSuccess,
        /// <summary>
        /// The item has completed running with a failed status.
        /// </summary>
        CompletedFailed,
        /// <summary>
        /// The item has been cancelled before running.
        /// </summary>
        Cancelled,
        /// <summary>
        /// The status of the item is unknown.
        /// </summary>
        Unknown,
    }
}
