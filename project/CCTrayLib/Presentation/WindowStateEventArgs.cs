using System;
using Microsoft.Win32;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class WindowStateEventArgs : EventArgs
	{
		public readonly RegistryKey Key;

		public WindowStateEventArgs( RegistryKey key )
		{
			Key = key;
		}
	}
}
