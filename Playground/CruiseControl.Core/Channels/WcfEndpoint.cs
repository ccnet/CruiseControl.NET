namespace CruiseControl.Core.Channels
{
    using System.ServiceModel.Channels;

    /// <summary>
    /// Defines an endpoint for the WCF channel.
    /// </summary>
    public abstract class WcfEndpoint
    {
        #region Public properties
        #region Address
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address { get; set; }
        #endregion

        #region Binding
        /// <summary>
        /// Gets the binding.
        /// </summary>
        public abstract Binding Binding { get; }
        #endregion
        #endregion
    }
}
