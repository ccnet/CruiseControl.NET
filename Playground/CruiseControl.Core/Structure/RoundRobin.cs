namespace CruiseControl.Core.Structure
{
    using System.Collections.Generic;
    using System.Windows.Markup;
    using System;

    public class RoundRobin
        : ServerItemContainerBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RoundRobin"/> class.
        /// </summary>
        public RoundRobin()
            : base()
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
