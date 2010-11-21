using System;
using System.Runtime.Serialization;

namespace ThoughtWorks.CruiseControl.Core.Config
{
	/// <summary>
	/// Typed exception for use within CruiseControl configuration.
	/// </summary>
	[Serializable]
	public class ConfigurationException : CruiseControlException
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException" /> class.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks></remarks>
		public ConfigurationException(string message) : base(message) {}
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException" /> class.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <remarks></remarks>
		public ConfigurationException(string message, Exception exception) : base(message, exception) {}
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException" /> class.	
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        /// <remarks></remarks>
		protected ConfigurationException(SerializationInfo info, StreamingContext context)
			:base (info, context) {}
	}

	/// <summary>
	/// Exception thrown if configuration file (ccnet.config) could not be found.
	/// </summary>
	[Serializable]
	public class ConfigurationFileMissingException : ConfigurationException
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationFileMissingException" /> class.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks></remarks>
		public ConfigurationFileMissingException(string message) : base(message) {}
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationFileMissingException" /> class.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <remarks></remarks>
		public ConfigurationFileMissingException(string message, Exception exception) : base(message, exception) {}
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationFileMissingException" /> class.	
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        /// <remarks></remarks>
		protected ConfigurationFileMissingException(SerializationInfo info, StreamingContext context)
			: base(info, context) {}
	}
}
