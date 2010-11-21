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
		public ConfigurationException(string message) : base(message) {}
		public ConfigurationException(string message, Exception exception) : base(message, exception) {}
		protected ConfigurationException(SerializationInfo info, StreamingContext context)
			:base (info, context) {}
	}

	/// <summary>
	/// Exception thrown if configuration file (ccnet.config) could not be found.
	/// </summary>
	[Serializable]
	public class ConfigurationFileMissingException : ConfigurationException
	{
		public ConfigurationFileMissingException(string message) : base(message) {}
		public ConfigurationFileMissingException(string message, Exception exception) : base(message, exception) {}
		protected ConfigurationFileMissingException(SerializationInfo info, StreamingContext context)
			: base(info, context) {}
	}
}
