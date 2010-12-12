namespace CruiseControl.Core
{
    /// <summary>
    /// An item for the server.
    /// </summary>
    public abstract class ServerItem
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerItem"/> class.
        /// </summary>
        protected ServerItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerItem"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected ServerItem(string name)
        {
            this.Name = name;
        }
        #endregion

        #region Public properties
        #region Name
        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region AskToIntegrate()
        /// <summary>
        /// Asks if an item can integrate.
        /// </summary>
        /// <param name="context">The context to use.</param>
        public abstract void AskToIntegrate(IntegrationContext context);
        #endregion
        #endregion
    }
}
