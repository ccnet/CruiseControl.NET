
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
				object value = null;
				if (key != null)
				{
					value = key.GetValue(name);
				}
				return (value == null) ? null : value.ToString();
			}
		}

		public string GetExpectedLocalMachineSubKeyValue(string path, string name)
		{
			string value = GetLocalMachineSubKeyValue(path, name);
			if (value == null)
			{
				throw new CruiseControlException(string.Format(@"Registry key or value name not found: HKEY_LOCAL_MACHINE\{0}\{1}", path, name)); 
			}
			return value;
		}
	}
}
