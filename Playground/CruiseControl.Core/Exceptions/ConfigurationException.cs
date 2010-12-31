namespace CruiseControl.Core.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// There is an error detected in the configuration.
    /// </summary>
    [Serializable]
    public class ConfigurationException
        : CruiseControlException
    {
        private const string ConfigurationProblemData = "Ay_Caramba";
        private readonly string configurationProblem;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException" /> class.	
        /// </summary>
        /// <param name="configurationProblem">The problem in the configuration.</param>
        /// <remarks></remarks>
        public ConfigurationException(string configurationProblem)
            : base(configurationProblem)
        {
            this.configurationProblem = configurationProblem;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException" /> class.	
        /// </summary>
        /// <param name="configurationProblem">The problem in the configuration</param>
        /// <param name="args">variables in the error string</param>
        public ConfigurationException(string configurationProblem, params string[] args)
            : base(string.Format(configurationProblem, args))
        {
            this.configurationProblem = string.Format(configurationProblem, args);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException"/> class.
        /// </summary>
        /// <param name="configurationProblem">The configuration problem.</param>
        /// <param name="innerException">The inner exception.</param>
        public ConfigurationException(string configurationProblem, Exception innerException)
            : base(configurationProblem, innerException)
        {
            this.configurationProblem = configurationProblem;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException" /> class.	
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        public ConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.configurationProblem = info.GetString(ConfigurationProblemData);
        }

        /// <summary>
        /// Gets the configuration problem.
        /// </summary>
        public string ConfigurationProblem
        {
            get { return this.configurationProblem; }
        }

        /// <summary>
        /// Gets the object data.	
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        /// <remarks></remarks>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(ConfigurationProblemData, configurationProblem);
        }
    }
}