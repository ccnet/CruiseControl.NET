namespace CruiseControl.Core.Structure
{
    using System;

    /// <summary>
    /// A pipeline of integrations - will trigger additional integrations to build projects.
    /// </summary>
    public class Pipeline
        : ServerItemContainerBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Pipeline"/> class.
        /// </summary>
        public Pipeline()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pipeline"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="children">The children.</param>
        public Pipeline(string name, params ServerItem[] children)
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
