namespace ThoughtWorks.CruiseControl.Core.Tasks.Conditions
{
    using System.Globalization;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Config;

    /// <title>Or Condition</title>
    /// <version>1.6</version>
    /// <summary>
    /// Checks that at least one child condition matches.
    /// </summary>
    /// <example>
    /// <code title="Basic example">
    /// <![CDATA[
    /// <orCondition>
    /// <conditions>
    /// <!-- Conditions to check -->
    /// </conditions>
    /// </orCondition>
    /// ]]>
    /// </code>
    /// <code title="Example in context">
    /// <![CDATA[
    /// <conditional>
    /// <conditions>
    /// <orCondition>
    /// <conditions>
    /// <!-- Conditions to check -->
    /// </conditions>
    /// </orCondition>
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
    [ReflectorType("orCondition")]
    public class OrTaskCondition
        : ConditionBase, IConfigurationValidation
    {
        #region Public properties
        #region Conditions
        /// <summary>
        /// The conditions to check.
        /// </summary>
        /// <version>1.6</version>
        /// <default>n/a</default>
        [ReflectorProperty("conditions", Required = true)]
        public ITaskCondition[] Conditions { get; set; }
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
            this.LogDescriptionOrMessage("Performing OR check - " + 
                this.Conditions.Length.ToString(CultureInfo.InvariantCulture) + 
                " conditions to check");
            var evaluationResult = false;
            foreach (var condition in this.Conditions)
            {
                evaluationResult = condition.Eval(result);
                if (evaluationResult)
                {
                    break;
                }
            }

            return evaluationResult;
        }
        #endregion

        #region Validate()
        /// <summary>
        /// Checks the internal validation of the item.
        /// </summary>
        /// <param name="configuration">The entire configuration.</param>
        /// <param name="parent">The parent item for the item being validated.</param>
        /// <param name="errorProcesser">The error processer to use.</param>
        public void Validate(IConfiguration configuration, 
            ConfigurationTrace parent, 
            IConfigurationErrorProcesser errorProcesser)
        {
            if (this.Conditions.Length == 0)
            {
                errorProcesser
                    .ProcessError("Validation failed for orCondition - at least one child condition must be supplied");
            }
        }
        #endregion
        #endregion
    }
}
