namespace CruiseControl.Common.Messages
{
    using System.ComponentModel;

    /// <summary>
    /// An item on the server.
    /// </summary>
    public class ServerItem
    {
        #region Public properties
        #region DisplayName
        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName { get; set; }
        #endregion

        #region Urn
        /// <summary>
        /// Gets or sets the URN.
        /// </summary>
        /// <value>
        /// The URN of the item.
        /// </value>
        public string Urn { get; set; }
        #endregion

        #region Description
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [DefaultValue(null)]
        public string Description { get; set; }
        #endregion
        #endregion
    }
}
