namespace ThoughtWorks.CruiseControl.Core.Tasks.Conditions
{
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Config;
    using ThoughtWorks.CruiseControl.Core.Util;

    /// <title>URL Ping Condition</title>
    /// <version>1.6</version>
    /// <summary>
    /// Checks if a URL can be pinged.
    /// </summary>
    /// <example>
    /// <code title="Basic example">
    /// <![CDATA[
    /// <urlPingCondition url="http://somewhere.com" />
    /// ]]>
    /// </code>
    /// <code title="Example in context">
    /// <![CDATA[
    /// <conditional>
    /// <conditions>
    /// <urlPingCondition>
    /// <url>http://somewhere.com</url>
    /// </urlPingCondition>
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
    [ReflectorType("urlPingCondition")]
    public class UrlPingsTaskCondition
        : ConditionBase, IConfigurationValidation
    {
        #region Public properties
        #region Url
        /// <summary>
        /// The URL to ping.
        /// </summary>
        /// <version>1.6</version>
        /// <default>n/a</default>
        [ReflectorProperty("url", Required = true)]
        public string Url { get; set; }
        #endregion

        #region WebFunctions
        /// <summary>
        /// Gets or sets the web functions.
        /// </summary>
        /// <value>The web functions.</value>
        public IWebFunctions WebFunctions { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region Validate()
        /// <summary>
        /// Checks the internal validation of the item.
        /// </summary>
        /// <param name="configuration">The entire configuration.</param>
        /// <param name="parent">The parent item for the item being validated.</param>
        /// <param name="errorProcesser">The error processer to use.</param>
        public void Validate(IConfiguration configuration, ConfigurationTrace parent, IConfigurationErrorProcesser errorProcesser)
        {
            if (string.IsNullOrEmpty(this.Url))
            {
                errorProcesser.ProcessError("URL cannot be empty");
            }
        }
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
            this.LogDescriptionOrMessage("Pinging URL '" + this.Url + "'");
            var functions = this.WebFunctions ?? new DefaultWebFunctions();
            var exists = functions.PingUrl(this.Url);
            return exists;
        }
        #endregion
        #endregion
    }
}
