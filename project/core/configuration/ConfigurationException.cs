using System;

namespace ThoughtWorks.CruiseControl.Core.Configuration
{
	public class ConfigurationException : CruiseControlException
	{
		public ConfigurationException(string s) : base(s) {}
		public ConfigurationException(string s, Exception e) : base(s, e) {}
	}
}
