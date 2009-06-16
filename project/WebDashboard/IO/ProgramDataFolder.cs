using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.IO
{
    /// <summary>
    /// A file system location to store data in.
    /// </summary>
    public static class ProgramDataFolder
    {
        #region private fields
        private static string location = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            Path.Combine("CruiseControl.NET", "WebDashboard"));
        #endregion

        #region Public properties
        #region Location
        /// <summary>
        /// Gets or sets the location of the program data folder.
        /// </summary>
        /// <value>The program data folder path.</value>
        public static string Location
        {
            get { return location; }
            set { location = value; }
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
            var fullPath = new DirectoryInfo(Path.Combine(location, path)).FullName;
            return fullPath;
        }
        #endregion
        #endregion
    }
}
