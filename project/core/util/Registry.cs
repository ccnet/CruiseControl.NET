
using Microsoft.Win32;
using System;
using System.Globalization;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// 	
    /// </summary>
	public class Registry : IRegistry
	{
        /// <summary>
        /// Gets the local machine sub key value.	
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <remarks></remarks>
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

        /// <summary>
        /// Gets the expected local machine sub key value.	
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public string GetExpectedLocalMachineSubKeyValue(string path, string name)
		{
			string value = GetLocalMachineSubKeyValue(path, name);
			if (value == null)
			{
				throw new CruiseControlException(string.Format(CultureInfo.CurrentCulture, @"Registry key or value name not found: HKEY_LOCAL_MACHINE\{0}\{1}", path, name)); 
			}
			return value;
		}
	}
}
