using System;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Telelogic
{
	public interface ISynergyCommand : IDisposable
	{
		ProcessResult Execute(ProcessInfo processInfo);
		ProcessResult Execute(ProcessInfo processInfo, bool failOnError);
	}
}