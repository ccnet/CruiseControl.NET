using System;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public interface IRegistry
	{
		string GetLocalMachineSubKeyValue(string path, string name);
		string GetExpectedLocalMachineSubKeyValue(string path, string name);
	}
}
