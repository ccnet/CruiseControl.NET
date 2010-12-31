namespace CruiseControl.Core.Interfaces
{
    public class GetSourceParameters
    {
        #region Public properties
        #region Revert
        /// <summary>
        /// Gets or sets a value indicating whether any changes should be reverted prior to getting
        /// the source.
        /// </summary>
        /// <value>
        ///   <c>true</c> if revert; otherwise, <c>false</c>.
        /// </value>
        public bool Revert { get; set; }
        #endregion
        #endregion
    }
}
