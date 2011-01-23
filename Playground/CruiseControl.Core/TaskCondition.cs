namespace CruiseControl.Core
{
    using CruiseControl.Core.Interfaces;

    /// <summary>
    /// A condition for a task.
    /// </summary>
    public abstract class TaskCondition
    {
        #region Public methods
        #region Evaluate()
        /// <summary>
        /// Evaluates whether this condition is valid.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///   <c>true</c> if this instance is valid; <c>false</c> otherwise.
        /// </returns>
        public abstract bool Evaluate(TaskExecutionContext context);
        #endregion

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
