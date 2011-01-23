
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// 	
    /// </summary>
	public sealed class ExecutionEnvironment : IExecutionEnvironment
	{
		private static bool? isRunningOnWindows;

        /// <summary>
        /// Gets the directory separator.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
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
				int platform = (int) Environment.OSVersion.Platform;
				isRunningOnWindows = ((platform != 4) // PlatformID.Unix
					&& (platform != 6) // PlatformID.MacOSX
					&& (platform != 128)); // Mono compability value for PlatformID.Unix on .NET 1.x profile

				return isRunningOnWindows.Value;
			}
		}

		/// <summary>
		/// Gets the directory where the common language runtime is installed. 
		/// </summary>
		public string RuntimeDirectory
		{
			get
			{
				return RuntimeEnvironment.GetRuntimeDirectory();
			}
		}

		/// <summary>
		/// Get the directory for the default location of the CruiseControl.NET data files.
		/// </summary>
		/// <param name="application">Type of the application. E.g. Server or WebDashboard.</param>
		/// <returns>The location of the CruiseControl.NET data files.</returns>
		public string GetDefaultProgramDataFolder(ApplicationType application)
		{
            if (application == ApplicationType.Unknown)
            {
                throw new ArgumentOutOfRangeException("application");
            }

            var pgfPath = AppDomain.CurrentDomain.BaseDirectory;
            return pgfPath;
		}

		/// <summary>
		/// Enstures the path is rooted.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public string EnsurePathIsRooted(string path)
		{
			if (!Path.IsPathRooted(path))
			{
				path = Path.Combine(
					GetDefaultProgramDataFolder(ApplicationType.Server),
					path);
			}
			return path;
		}
	}

    /// <summary>
    /// 	
    /// </summary>
	public interface IExecutionEnvironment
	{
        /// <summary>
        /// Gets the directory separator.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		char DirectorySeparator { get; }
        /// <summary>
        /// Gets the is running on windows.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		bool IsRunningOnWindows { get; }
        /// <summary>
        /// Gets the runtime directory.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		string RuntimeDirectory { get; }
        /// <summary>
        /// Gets the default program data folder.	
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		string GetDefaultProgramDataFolder(ApplicationType application);
        /// <summary>
        /// Ensures the path is rooted.	
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		string EnsurePathIsRooted(string path);
	}

    /// <summary>
    /// 	
    /// </summary>
	public enum ApplicationType
	{
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		Unknown = 0,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		Server = 1,
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		WebDashboard = 2
	}
}