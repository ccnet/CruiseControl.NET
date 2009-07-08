using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.WebDashboard.IO
{
    /// <summary>
    /// A file system location to store data in.
    /// </summary>
    public static class ProgramDataFolder
    {
        #region private fields

		private static readonly IFileSystem fileSystem = new SystemIoFileSystem();
    	private static readonly IExecutionEnvironment executionEnvironment = new ExecutionEnvironment();

        private static string location;
        #endregion

        #region Public properties
        #region Location
        /// <summary>
        /// Gets or sets the location of the program data folder.
        /// </summary>
        /// <value>The program data folder path.</value>
        public static string Location
        {
            get
            {
				if (string.IsNullOrEmpty(location))
				{
					location = executionEnvironment.GetDefaultProgramDataFolder(ApplicationType.WebDashboard);
					Log.Debug(string.Concat("Initialising data folder: '", location,"'."));
					fileSystem.EnsureFolderExists(location);
				}

            	return location;
            }
            set
            {
				Log.Debug(string.Concat("Data folder set to: '", value, "'."));
				fileSystem.EnsureFolderExists(value);
            	location = value;
            }
        }
        #endregion
        #endregion

        #region Public methods
        #region MapPath()
        /// <summary>
        /// Maps the path.
        /// </summary>
        /// <param name="path">The path to map.</param>
        /// <returns>The mapped path.</returns>
        public static string MapPath(string path)
        {
			var fullPath = new DirectoryInfo(Path.Combine(Location, path)).FullName;
            return fullPath;
        }
        #endregion
        #endregion
    }
}
