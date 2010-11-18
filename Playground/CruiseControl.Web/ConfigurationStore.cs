namespace CruiseControl.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Configuration;
    using System.IO;
using System.Collections.Specialized;

    /// <summary>
    /// Exposes the various settings from the configuration store.
    /// </summary>
    public static class ConfigurationStore
    {
        #region Private fields
        private static NameValueCollection sSettings = ConfigurationManager.AppSettings;
        #endregion

        #region Public properties
        #region AppSettings
        /// <summary>
        /// Gets or sets the app settings.
        /// </summary>
        /// <value>The app settings.</value>
        public static NameValueCollection AppSettings
        {
            get { return sSettings; }
            set { sSettings = value; }
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
            sSettings = ConfigurationManager.AppSettings;
        }
        #endregion
        #endregion
    }
}