using System;

namespace ThoughtWorks.CruiseControl.Core.Config
{
	/// <summary>
	/// Typed exception for use within CruiseControl configuration.
	/// </summary>
	public class ConfigurationException : CruiseControlException
	{
		public ConfigurationException(string s) : base(s) {}
		public ConfigurationException(string s, Exception e) : base(s, e) {}
	}
}
