namespace CruiseControl.Core
{
    using System;

    /// <summary>
    /// A request to integrate an item
    /// </summary>
    public class IntegrationRequest
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="IntegrationRequest"/> class.
        /// </summary>
        public IntegrationRequest()
        {
            this.RequestTime = DateTime.Now;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegrationRequest"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public IntegrationRequest(IntegrationContext context)
            : this()
        {
            this.Context = context;
        }
        #endregion

        #region Public properties
        #region Context
        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public IntegrationContext Context { get; set; }
        #endregion

        #region RequestTime
        /// <summary>
        /// Gets or sets the request time.
        /// </summary>
        /// <value>
        /// The request time.
        /// </value>
        public DateTime RequestTime { get; set; }
        #endregion
        #endregion
    }
}
