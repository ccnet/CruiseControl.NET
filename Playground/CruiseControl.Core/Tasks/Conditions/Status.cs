namespace CruiseControl.Core.Tasks.Conditions
{
    using System.Windows.Markup;

    /// <summary>
    /// Checks the current status of the build.
    /// </summary>
    [ContentProperty("Value")]
    public class Status
        : TaskCondition
    {
        #region Public properties
        #region Value
        public IntegrationStatus Value { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region Evaluate()
        /// <summary>
        /// Evaluates whether this condition is valid.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///   <c>true</c> if this instance is valid; <c>false</c> otherwise.
        /// </returns>
        public override bool Evaluate(TaskExecutionContext context)
        {
            var isValid = context.CurrentStatus == this.Value;
            return isValid;
        }
        #endregion
        #endregion
    }
}
