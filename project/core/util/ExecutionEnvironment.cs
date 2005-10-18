using System;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public sealed class ExecutionEnvironment
	{
		private ExecutionEnvironment()
		{
			// static members only
		}

		/// <summary>
		/// Returns true if CruiseControl is running on a windows platform
		/// </summary>
		/// <remarks>
		/// If this method returns false, expect any DllImport not to work!
		/// </remarks>
		public static bool IsRunningOnWindows
		{
			get
			{
				// mono returns 128 when running on linux, .NET 2.0 returns 4
				// see http://www.mono-project.com/FAQ:_Technical
				int platform = (int) Environment.OSVersion.Platform;
				return ((platform != 4) && (platform != 128));
			}
		}

	}
}
