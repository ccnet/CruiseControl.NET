namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// Defines an item that has a controlable status.
    /// </summary>
    public interface IStatusItem
        : IStatusSnapshotGenerator
    {
        #region Public methods
        #region InitialiseStatus()
        /// <summary>
        /// Initialises the status.
        /// </summary>
        void InitialiseStatus();
        #endregion
        
        #region CancelStatus()
        /// <summary>
        /// Cancels the status.
        /// </summary>
        void CancelStatus();
        #endregion
        #endregion
    }
}
