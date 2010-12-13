namespace CruiseControl.Core.Structure
{
    using System;

    /// <summary>
    /// An integration gate - will accumulate integration requests until released.
    /// </summary>
    public class Gate
        : ServerItemContainerBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Gate"/> class.
        /// </summary>
        public Gate()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gate"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="children">The children.</param>
        public Gate(string name, params ServerItem[] children)
            : base(name, children)
        {
        }
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
