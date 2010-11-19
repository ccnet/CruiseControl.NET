namespace CruiseControl.Web.Configuration
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.IO;

    /// <summary>
    /// Exposes the various settings from the configuration store.
    /// </summary>
    public static class Folders
    {
        #region Private fields
        private static NameValueCollection Settings = ConfigurationManager.AppSettings;
        #endregion

        #region Public properties
        #region AppSettings
        /// <summary>
        /// Gets or sets the app settings.
        /// </summary>
        /// <value>The app settings.</value>
        public static NameValueCollection AppSettings
        {
            get { return Settings; }
            set { Settings = value; }
        }
        #endregion

        #region DataDirectory
        /// <summary>
        /// Gets the data directory.
        /// </summary>
        /// <value>The data directory.</value>
        public static string DataDirectory
        {
            get
            {
                var directory = AppSettings["CCNetDataDirectory"];
                if (string.IsNullOrEmpty(directory))
                {
                    directory = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                        "CCNet",
                        "Web");
                }

                return directory;
            }
        }
        #endregion

        #region PluginDirectory
        /// <summary>
        /// Gets the plug-in directory.
        /// </summary>
        /// <value>The plug-in directory.</value>
        public static string PluginDirectory
        {
            get
            {
                var directory = AppSettings["CCNetPluginDirectory"];
                if (string.IsNullOrEmpty(directory))
                {
                    directory = Path.Combine(DataDirectory, "Plugins");
                }

                return directory;
            }
        }
        #endregion
        #endregion

        #region Public methods
        #region Reset()
        /// <summary>
        /// Resets this instance.
        /// </summary>
        public static void Reset()
        {
            Settings = ConfigurationManager.AppSettings;
        }
        #endregion
        #endregion
    }
}