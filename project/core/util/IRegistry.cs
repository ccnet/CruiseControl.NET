using System;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public interface IRegistry
	{
		string GetLocalMachineSubKeyValue(string path, string name);
	}
}
