namespace ThoughtWorks.CruiseControl.Core
{
    using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// Defines a mechanism for storing data from a project.
    /// </summary>
    public interface IDataStore
    {
        #region Public methods
        #region StoreProjectSnapshot()
        /// <summary>
        /// Stores a snapshot of a project build.
        /// </summary>
        /// <param name="result">The result that the snapshot is for.</param>
        /// <param name="snapshot">The project snapshot.</param>
        void StoreProjectSnapshot(IIntegrationResult result, ItemStatus snapshot);
        #endregion
        #endregion
    }
}
