namespace ThoughtWorks.CruiseControl.Core.Tasks.Conditions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Remote;

    /// <title>Status Condition</title>
    /// <version>1.6</version>
    /// <summary>
    /// Checks if the current status matches a status.
    /// </summary>
    /// <example>
    /// <code title="Basic example">
    /// <![CDATA[
    /// <statusCondition value="Success" />
    /// ]]>
    /// </code>
    /// <code title="In context">
    /// <![CDATA[
    /// 
    /// ]]>
    /// </code>
    /// </example>
    [ReflectorType("statusCondition")]
    public class StatusTaskCondition
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
            this.RetrieveLogger()
                .Debug("Checing status - matching to " + this.Status.ToString());
            return this.Status == result.Status;
        }
        #endregion
        #endregion
    }
}
