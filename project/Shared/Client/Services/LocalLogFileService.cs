using System;
using System.IO;

using ThoughtWorks.CruiseControl.Shared.Services;
using ThoughtWorks.CruiseControl.Shared.Services.Commands.Reporting;
using ThoughtWorks.CruiseControl.Shared.Entities.Logging;

namespace ThoughtWorks.CruiseControl.Shared.Client.Services
{
	public class LocalLogFileService : ISpecializedCruiseService
	{
		protected static readonly Type[] SUPPORTED_TYPES = new Type[] { typeof(GetProjectLogCommand) };

		private LocalLogFileServiceConfig _config;

		public LocalLogFileService(LocalLogFileServiceConfig config)
		{
			_config = config;
		}

		public ICruiseResult Run(ICruiseCommand command)
		{
			if (!(command is GetProjectLogCommand))
			{
				throw new InvalidCommandException(command);
			}
			GetProjectLogCommand cmd = command as GetProjectLogCommand;

			string logfilename = LogFileUtil.GetLatestLogFileName(_config.GetDefaultProjectLogDirectory());

			string log;
			using (StreamReader sr = new StreamReader(Path.Combine(_config.GetDefaultProjectLogDirectory(), logfilename)))
			{
				log = sr.ReadToEnd();
			}

			return new GetProjectLogResult(log);
		}

		public Type[] SupportedCommandTypes
		{
			get
			{
				return SUPPORTED_TYPES;
			}
		}
	}
}
