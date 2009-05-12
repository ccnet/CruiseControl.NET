using System;
using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public sealed class ExecutionEnvironment : IExecutionEnvironment
	{
		private static bool? isRunningOnWindows;

		public char DirectorySeparator
		{
			get { return Path.DirectorySeparatorChar; }
		}

		/// <summary>
		/// Returns true if CruiseControl is running on a windows platform
		/// </summary>
		/// <remarks>
		/// If this method returns false, expect any DllImport not to work!
		/// </remarks>
		public bool IsRunningOnWindows
		{
			get
			{
				if (isRunningOnWindows.HasValue)
					return isRunningOnWindows.Value;

				// mono returns 4, 128 when running on linux, .NET 2.0 returns 4
				// and 6 for MacOSX
				// see http://www.mono-project.com/FAQ:_Technical
				int platform = (int)Environment.OSVersion.Platform;
				isRunningOnWindows = ((platform != 4) // PlatformID.Unix
					&& (platform != 6) // PlatformID.MacOSX
					&& (platform != 128)); // Mono compability value for PlatformID.Unix on .NET 1.x profile

				return isRunningOnWindows.Value;
			}
		}
	}

	public interface IExecutionEnvironment
	{
		char DirectorySeparator { get; }
		bool IsRunningOnWindows { get; }
	}
}