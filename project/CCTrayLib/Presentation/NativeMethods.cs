using System;
using System.Runtime.InteropServices;
using System.Security;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	[SuppressUnmanagedCodeSecurity]
	public sealed class NativeMethods
	{
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetForegroundWindow(IntPtr handle);
	}
}