using System;
using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public sealed class ExecutionEnvironment : IExecutionEnvironment
	{
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
				// mono returns 128 when running on linux, .NET 2.0 returns 4
				// see http://www.mono-project.com/FAQ:_Technical
				int platform = (int) Environment.OSVersion.Platform;
				return ((platform != 4) && (platform != 128));
			}
		}
	}

	public interface IExecutionEnvironment
	{
		char DirectorySeparator { get; }
		bool IsRunningOnWindows { get; }
	}
}