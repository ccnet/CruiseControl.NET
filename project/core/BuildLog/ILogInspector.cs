using System;

namespace ThoughtWorks.CruiseControl.Core.BuildLog
{
	public interface ILogInspector
	{
		string GetLogFileName(string log);
	}
}
