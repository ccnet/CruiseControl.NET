namespace ThoughtWorks.CruiseControl.Core.Tasks.Conditions
{
    using System;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Util;

    /// <title>Last Build Time Condition</title>
    /// <version>1.6</version>
    /// <summary>
    /// Checks if the last build started at least a certain time period ago.
    /// </summary>
    /// <example>
    /// <code title="Basic example">
    /// <![CDATA[
    /// <lastBuildTimeCondition time="120" />
    /// ]]>
    /// </code>
    /// <code title="Example in context">
    /// <![CDATA[
    /// <conditional>
    /// <conditions>
    /// <lastBuildTimeCondition>
    /// <time unit="minutes">5</time>
    /// </lastBuildTimeCondition>
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
    [ReflectorType("lastBuildTimeCondition")]
    public class LastBuildTimeTaskCondition
        : ConditionBase
    {
        #region Public properties
        #region Status
        /// <summary>
        /// The time period to use.
        /// </summary>
        /// <version>1.6</version>
        /// <default>n/a</default>
        [ReflectorProperty("time", typeof(TimeoutSerializerFactory), Required = true)]
        public Timeout Time { get; set; }
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
            this.LogDescriptionOrMessage(
                "Checking last build time - checking build was at least " +
                (this.Time.Millis/1000) + "s ago");
            if (result.IsInitial())
            {
                // There is no previous build - assume that the condition passes
                return true;
            }

            var checkTime = DateTime.Now.AddMilliseconds(-this.Time.Millis);
            var isTrue = checkTime > result.LastIntegration.StartTime;
            return isTrue;
        }
        #endregion
        #endregion
    }
}
