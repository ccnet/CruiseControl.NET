using Microsoft.Win32;
using System;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class Registry : IRegistry
	{
		public string GetLocalMachineSubKeyValue(string path, string name)
		{
			using (RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(path))
			{
				try { return key.GetValue(name).ToString(); }
				catch (NullReferenceException) 
				{ 
					throw new CruiseControlException(string.Format(@"Registry key or value name not found: {0}\{1}", path, name)); 
				}
			}
		}
	}
}
