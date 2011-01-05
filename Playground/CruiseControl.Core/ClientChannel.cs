namespace CruiseControl.Core
{
    using CruiseControl.Core.Interfaces;

    /// <summary>
    /// A communications channel for clients to use when communicating with the server.
    /// </summary>
    public abstract class ClientChannel
        : AttachablePropertyStore
    {
        #region Public properties
        #region Invoker
        /// <summary>
        /// Gets the invoker.
        /// </summary>
        public ActionInvoker Invoker { get; private set; }
        #endregion
        #endregion

        #region Public methods
        #region Validate()
        /// <summary>
        /// Validates this channel.
        /// </summary>
        /// <param name="validationLog">The validation log.</param>
        public virtual void Validate(IValidationLog validationLog)
        {
        }
        #endregion
        #region Initialise()
        /// <summary>
        /// Initialises this client.
        /// </summary>
        /// <param name="invoker">The invoker.</param>
        public void Initialise(ActionInvoker invoker)
        {
            this.Invoker = invoker;
            this.OnInitialise();
        }
        #endregion

        #region CleanUp()
        /// <summary>
        /// Cleans up this channel.
        /// </summary>
        public void CleanUp()
        {
            this.OnCleanUp();
        }
        #endregion
        #endregion

        #region Protected methods
        #region OnInitialise()
        /// <summary>
        /// Called when this channel is initialised.
        /// </summary>
        protected abstract void OnInitialise();
        #endregion

        #region OnCleanUp()
        /// <summary>
        /// Called when this channel is cleaned up.
        /// </summary>
        protected abstract void OnCleanUp();
        #endregion
        #endregion
    }
}
