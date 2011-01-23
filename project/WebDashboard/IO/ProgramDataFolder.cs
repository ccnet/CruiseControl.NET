namespace ThoughtWorks.CruiseControl.WebDashboard.IO
{
    using System.Configuration;
    using System.IO;
    using ThoughtWorks.CruiseControl.Core.Util;

    /// <summary>
    /// A file system location to store data in.
    /// </summary>
    public static class ProgramDataFolder
    {
        #region private fields
        /// <summary>
        /// The <see cref="IFileSystem"/> to use.
        /// </summary>
        private static readonly IFileSystem fileSystem = new SystemIoFileSystem();

        /// <summary>
        /// The <see cref="IExecutionEnvironment"/> to use.
        /// </summary>
        private static readonly IExecutionEnvironment executionEnvironment = new ExecutionEnvironment();

        /// <summary>
        /// The default location to use.
        /// </summary>
        private static string location;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="ProgramDataFolder"/> class.
        /// </summary>
        static ProgramDataFolder()
        {
            location = ConfigurationManager.AppSettings["CruiseControlDataLocation"];
        }
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
                    Log.Debug(string.Concat("Initialising data folder: '", location, "'."));
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
