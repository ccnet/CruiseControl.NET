namespace CruiseControl.Core
{
    using CruiseControl.Core.Interfaces;

    /// <summary>
    /// An action to perform when a task fails.
    /// </summary>
    public abstract class TaskFailureAction
    {
        #region Public methods
        #region Validate()
        /// <summary>
        /// Validates this condition.
        /// </summary>
        /// <param name="validationLog">The validation log.</param>
        public virtual void Validate(IValidationLog validationLog)
        {
        }
        #endregion
        #endregion
    }
}
