using System;

using ThoughtWorks.CruiseControl.Shared.Services;

namespace ThoughtWorks.CruiseControl.Shared.Client.Services
{
	public class LocalLogFileServiceConfig : ICruiseServiceConfig
	{
		// This Class will get more complicated as and when we setup for multi-project

		private string _logDirectoryName;
		public string LogDirectoryName
		{
			get
			{
				return _logDirectoryName;
			}
			set
			{
				_logDirectoryName = value;
			}
		}

		public Type ServiceType
		{
			get
			{
				return typeof(LocalLogFileService);
			}
		}

		public virtual string GetDefaultProjectLogDirectory()
		{
			return _logDirectoryName;
		}
	}
}
