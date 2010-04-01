namespace ThoughtWorks.CruiseControl.Core.Tasks.Conditions
{
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Remote;

    /// <title>Build Condition Condition</title>
    /// <version>1.6</version>
    /// <summary>
    /// Checks if the current build condition matches a value.
    /// </summary>
    /// <example>
    /// <code title="Basic example">
    /// <![CDATA[
    /// <buildCondition value="ForeceBuild" />
    /// ]]>
    /// </code>
    /// <code title="Example in context">
    /// <![CDATA[
    /// <conditional>
    /// <conditions>
    /// <buildCondition>
    /// <value>ForceBuild</value>
    /// </buildCondition>
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
    /// <remarks>
    /// <para>
    /// This condition has been kindly supplied by Lasse Sjorup. The original project is available from
    /// <link>http://ccnetconditional.codeplex.com/</link>.
    /// </para>
    /// </remarks>
    [ReflectorType("buildCondition")]
    public class BuildConditionTaskCondition
        : ConditionBase
    {
        #region Public properties
        #region BuildCondition
        /// <summary>
        /// The build condition to match.
        /// </summary>
        /// <version>1.6</version>
        /// <default>n/a</default>
        [ReflectorProperty("value", Required = true)]
        public BuildCondition BuildCondition { get; set; }
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
            this.RetrieveLogger()
                .Debug("Checking build condition - matching to " + this.BuildCondition.ToString());
            return this.BuildCondition == result.BuildCondition;
        }
        #endregion
        #endregion
    }
}
