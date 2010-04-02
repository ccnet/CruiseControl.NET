namespace ThoughtWorks.CruiseControl.Core
{
    using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// Generates a status snapshot of the item.
    /// </summary>
    public interface IStatusSnapshotGenerator
    {
        #region GenerateSnapshot()
        /// <summary>
        /// Generates a snapshot of the current status.
        /// </summary>
        /// <returns></returns>
        ItemStatus GenerateSnapshot();
        #endregion
    }
}
