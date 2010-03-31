namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// A condition that can be used within the <see cref="ConditionalTask"/>.
    /// </summary>
    public interface ITaskCondition
    {
        #region Public methods
        #region Eval()
        /// <summary>
        /// Evals the specified result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>
        /// <c>true</c> if the condition is true; <c>false</c> otherwise.
        /// </returns>
        bool Eval(IIntegrationResult result);
        #endregion
        #endregion
    }
}
