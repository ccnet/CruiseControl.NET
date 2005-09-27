using System;

namespace ThoughtWorks.CruiseControl.Core.Configuration
{
	public class DefaultServerConfiguration : IServerConfiguration
	{
		public string CcnetConfigFile
		{
			get { throw new NotImplementedException(); }
		}

		public string CcnetServiceName
		{
			get { throw new NotImplementedException(); }
		}

		public bool UseRemoting
		{
			get { throw new NotImplementedException(); }
		}

		public bool WatchConfigFile
		{
			get { throw new NotImplementedException(); }
		}

		public string RemotingSettingsFile
		{
			get { throw new NotImplementedException(); }
		}

		public string ProjectToBuild
		{
			get { throw new NotImplementedException(); }
		}

		public string ServerLogFilePath
		{
			get { throw new NotImplementedException(); }
		}

		public string ServerLogFileLines
		{
			get { throw new NotImplementedException(); }
		}
	}
}
