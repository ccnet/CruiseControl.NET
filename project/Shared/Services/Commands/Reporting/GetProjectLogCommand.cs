using System;

using ThoughtWorks.CruiseControl.Shared.Services;

namespace ThoughtWorks.CruiseControl.Shared.Services.Commands.Reporting
{
	public enum GetProjectLogCommandLogType
	{
		Latest,
		Dated
	}

	public class GetProjectLogCommand : ICruiseCommand
	{
		private readonly string _projectName;
		private readonly DateTime _date;
		private readonly GetProjectLogCommandLogType _logType;
		private GetProjectLogResult _result;

		public GetProjectLogCommand()
		{
			_projectName = "";
			_logType = GetProjectLogCommandLogType.Latest;
		}

		public GetProjectLogCommand(string projectName) : this()
		{
			_projectName = projectName;
		}

		public GetProjectLogCommand(DateTime date) : this()
		{
			_date = date;
			_logType = GetProjectLogCommandLogType.Dated;
		}

		public GetProjectLogCommand(string projectName, DateTime date) : this(date)
		{
			_projectName = projectName;
		}

		public string ProjectName
		{
			get
			{
				return _projectName;
			}
		}

		public GetProjectLogCommandLogType LogType
		{
			get
			{
				return _logType;
			}
		}

		public DateTime LogDate
		{
			get
			{
				return _date;
			}
		}

		public GetProjectLogResult Result
		{
			get
			{
				return _result;
			}
			set
			{
				_result = value;
			}
		}
	}
}
