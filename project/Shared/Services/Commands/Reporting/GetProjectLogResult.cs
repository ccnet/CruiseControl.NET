using System;

using ThoughtWorks.CruiseControl.Shared.Services;

namespace ThoughtWorks.CruiseControl.Shared.Services.Commands.Reporting
{
	public class GetProjectLogResult : ICruiseResult
	{
		private readonly string _log;

		public GetProjectLogResult(string log)
		{
			_log = log;
		}

		public string Log
		{
			get
			{
				return _log;
			}
		}
	}
}
