
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
		public ConfigurationException(string s) : base(s) {}
		public ConfigurationException(string s, Exception e) : base(s, e) {}
		protected ConfigurationException(SerializationInfo info, StreamingContext context)
			:base (info, context) {}
	}

	/// <summary>
	/// Exception thrown if configuration file (ccnet.config) could not be found.
	/// </summary>
	[Serializable]
	public class ConfigurationFileMissingException : ConfigurationException
	{
		public ConfigurationFileMissingException(string s) : base(s) {}
		public ConfigurationFileMissingException(string s, Exception e) : base(s, e) {}
		protected ConfigurationFileMissingException(SerializationInfo info, StreamingContext context)
			: base(info, context) {}
	}
}
