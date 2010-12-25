using System;
using System.Runtime.Serialization;

namespace CruiseControl.Core.Exceptions
{
    /// <summary>
    /// There is an error detected in the configuration.
    /// </summary>
    [Serializable]
    public class ConfigurationException : CruiseControlException
    {
        private const string configurationProblemData = "Ay_Caramba";
        private readonly string configurationProblem;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException" /> class.	
        /// </summary>
        /// <remarks></remarks>
        public ConfigurationException() : base(string.Empty) { }

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
            : base(string.Format(configurationProblem,args))
        {
            this.configurationProblem = string.Format(configurationProblem, args);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException" /> class.	
        /// </summary>
        /// <param name="requestedProject">The requested project.</param>
        /// <param name="e">The e.</param>
        /// <remarks></remarks>
        public ConfigurationException(string configurationProblem, Exception e)
            : base(configurationProblem, e)
        {
            this.configurationProblem = configurationProblem;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException" /> class.	
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        /// <remarks></remarks>
        public ConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.configurationProblem = info.GetString(configurationProblemData);
        }

        /// <summary>
        /// Gets the requested project.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string ConfigurationProblem
        {
            get { return configurationProblem; }
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
            info.AddValue(configurationProblemData, configurationProblem);
        }
    }
}