namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// Defines that an item stores a hash.
    /// </summary>
    public interface IHashStore
    {
        #region Public properies
        #region Hash
        /// <summary>
        /// Gets or sets the hash.
        /// </summary>
        /// <value>
        /// The hash.
        /// </value>
        byte[] Hash { get; set; }
        #endregion
        #endregion
    }
}
