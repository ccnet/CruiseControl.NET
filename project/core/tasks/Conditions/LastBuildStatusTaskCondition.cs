using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Tasks.Conditions
{
    /// <title>Last Build Status Condition</title>
    /// <version>1.6</version>
    /// <summary>
    /// Checks if the status of the last build matches a value. If no previous build exists any specified status will return false.
    /// </summary>
    /// <example>
    /// <code title="Basic example">
    /// <![CDATA[
    /// <lastStatusCondition value="Success" />
    /// ]]>
    /// </code>
    /// <code title="Example in context">
    /// <![CDATA[
    /// <conditional>
    /// <conditions>
    /// <lastStatusCondition>
    /// <value>Failure</value>
    /// </lastStatusCondition>
    /// </conditions>
    /// <tasks>
    /// <!-- Tasks to perform if condition passed -->
    /// </tasks>
    /// <elseTasks>
    /// <!-- Tasks to perform if condition failed -->
    /// </elseTasks>
    /// </conditional>
    /// ]]>
    /// </code>
    /// </example>
    [ReflectorType("lastStatusCondition")]
    public class LastBuildStatusTaskCondition
        : ConditionBase
    {
        #region Public properties

        #region Status

        /// <summary>
        /// The status to match.
        /// </summary>
        /// <version>1.6</version>
        /// <default>n/a</default>
        [ReflectorProperty("value", Required = true)]
        public IntegrationStatus Status { get; set; }

        #endregion

        #endregion

        #region Protected methods

        #region Evaluate()

        /// <summary>
        /// Performs the actual evaluation.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>
        /// <c>true</c> if the condition is true; <c>false</c> otherwise.
        /// </returns>
        protected override bool Evaluate(IIntegrationResult result)
        {
            LogDescriptionOrMessage("Checking last build status - matching to " + Status);
            if (result.IsInitial())
            {
                // There is no previous build - assume that the condition fails
                return false;
            }
            else
            {
                return Status == result.LastBuildStatus;
            }
        }

        #endregion

        #endregion
    }
}