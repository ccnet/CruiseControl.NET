namespace CruiseControl.Core
{
    using System;
    using CruiseControl.Core.Interfaces;

    /// <summary>
    /// Defines a trigger for an integration.
    /// </summary>
    public abstract class Trigger
        : ProjectItem
    {
        #region Private fields
        private IntegrationRequest current;
        #endregion

        #region Public properties
        #region NextTime
        /// <summary>
        /// Gets the time the trigger will next be tripped or checked to see if tripped.
        /// </summary>
        public virtual DateTime? NextTime
        {
            get { return null; }
        }
        #endregion
        #endregion

        #region Public methods
        #region Validate()
        /// <summary>
        /// Validates this trigger.
        /// </summary>
        /// <param name="validationLog">The validation log.</param>
        public virtual void Validate(IValidationLog validationLog)
        {
        }
        #endregion

        #region Initialise()
        /// <summary>
        /// Initialises this when the project starts.
        /// </summary>
        public virtual void Initialise()
        {
            this.Reset();
        }
        #endregion

        #region Check()
        /// <summary>
        /// Checks if this trigger has been tripped.
        /// </summary>
        /// <returns>
        /// An <see cref="IntegrationRequest"/> if tripped; <c>null</c> otherwise.
        /// </returns>
        public IntegrationRequest Check()
        {
            return this.current ?? (this.current = this.OnCheck());
        }
        #endregion

        #region Reset()
        /// <summary>
        /// Resets this trigger after an integration.
        /// </summary>
        public void Reset()
        {
            this.current = null;
            this.OnReset();
        }
        #endregion

        #region CleanUp()
        /// <summary>
        /// Cleans up when the project stops.
        /// </summary>
        public virtual void CleanUp()
        {
        }
        #endregion
        #endregion

        #region Protected methods
        #region OnCheck()
        /// <summary>
        /// Called when the trigger needs to be checked.
        /// </summary>
        /// <returns>
        /// An <see cref="IntegrationRequest"/> if tripped; <c>null</c> otherwise.
        /// </returns>
        protected abstract IntegrationRequest OnCheck();
        #endregion

        #region OnReset()
        /// <summary>
        /// Called when a reset needs to be performed.
        /// </summary>
        protected virtual void OnReset()
        {
        }
        #endregion
        #endregion
    }
}
