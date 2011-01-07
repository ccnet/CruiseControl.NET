namespace CruiseControl.Core.Tasks.Conditions
{
    using System.Linq;

    /// <summary>
    /// A condition to check if there are any modifications.
    /// </summary>
    public class HasModifications
        : TaskCondition
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
        public override bool Evaluate(TaskExecutionContext context)
        {
            var modificationCount = context.ModificationSets.Count();
            return modificationCount > 0;
        }
        #endregion
        #endregion
    }
}
