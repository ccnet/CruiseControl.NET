namespace CruiseControl.Core.Structure
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Schedules integration requests in a round-robin manner.
    /// </summary>
    public class RoundRobin
        : ServerItemContainerBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RoundRobin"/> class.
        /// </summary>
        public RoundRobin()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoundRobin"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="children">The children.</param>
        public RoundRobin(string name, params ServerItem[] children)
            : base(name, children)
        {
        }
        #endregion

        #region Public properties
        #region AllowedActive
        /// <summary>
        /// Gets or sets the number of allowed active children.
        /// </summary>
        /// <value>The allowed active.</value>
        [DefaultValue(null)]
        public int? AllowedActive { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region AskToIntegrate()
        /// <summary>
        /// Asks if an item can integrate.
        /// </summary>
        /// <param name="context">The context to use.</param>
        public override void AskToIntegrate(IntegrationContext context)
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion
    }
}
