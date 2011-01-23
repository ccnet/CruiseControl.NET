namespace ThoughtWorks.CruiseControl.Core.Tasks.Conditions
{
    using System.Globalization;
    using System.Linq;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Config;

    /// <title>And Condition</title>
    /// <version>1.6</version>
    /// <summary>
    /// Checks that all the child condition pass.
    /// </summary>
    /// <example>
    /// <code title="Basic example">
    /// <![CDATA[
    /// <andCondition>
    /// <conditions>
    /// <!-- Conditions to check -->
    /// </conditions>
    /// </andCondition>
    /// ]]>
    /// </code>
    /// <code title="Example in context">
    /// <![CDATA[
    /// <conditional>
    /// <conditions>
    /// <andCondition>
    /// <conditions>
    /// <!-- Conditions to check -->
    /// </conditions>
    /// </andCondition>
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
    /// This condition has been kindly supplied by Lasse Sjørup. The original project is available from
    /// <link>http://ccnetconditional.codeplex.com/</link>.
    /// </para>
    /// </remarks>
    [ReflectorType("andCondition")]
    public class AndTaskCondition
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
            this.LogDescriptionOrMessage("Performing AND check - " +
                this.Conditions.Length.ToString(CultureInfo.InvariantCulture) +
                " conditions to check");
            var evaluationResult = true;
            foreach (var condition in this.Conditions)
            {
                evaluationResult = condition.Eval(result);
                if (!evaluationResult)
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
            if ((this.Conditions == null) || (this.Conditions.Length == 0))
            {
                errorProcesser
                    .ProcessError("Validation failed for andCondition - at least one child condition must be supplied");
            }
            else
            {
                foreach (var child in this.Conditions.OfType<IConfigurationValidation>())
                {
                    child.Validate(
                        configuration,
                        parent.Wrap(this),
                        errorProcesser);
                }
            }
        }
        #endregion
        #endregion
    }
}
