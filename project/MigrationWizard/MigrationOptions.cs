using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.MigrationWizard
{
    /// <summary>
    /// The migration options to use.
    /// </summary>
    public class MigrationOptions
    {
        #region Public consts
        public static string[] AllowedVersions = {
                                                    "1.4.4",
                                                    "1.4.3",
                                                    "1.4.2",
                                                    "1.4.1",
                                                    "1.4.0"
                                                };
        #endregion

        #region Public properties
        #region MigrateServer
        /// <summary>
        /// Should the server be migrated.
        /// </summary>
        public bool MigrateServer { get; set; }
        #endregion

        #region CurrentServerLocation
        /// <summary>
        /// The location of the server files.
        /// </summary>
        public string CurrentServerLocation { get; set; }
        #endregion

        #region NewServerLocation
        /// <summary>
        /// The location of the server files.
        /// </summary>
        public string NewServerLocation { get; set; }
        #endregion

        #region MigrateConfiguration
        /// <summary>
        /// Should the configuration be migrated.
        /// </summary>
        public bool MigrateConfiguration { get; set; }
        #endregion

        #region BackupConfiguration
        /// <summary>
        /// Should the configuration be backed-up before migration.
        /// </summary>
        public bool BackupConfiguration { get; set; }
        #endregion

        #region ConfigurationLocation
        /// <summary>
        /// The location of the configuration file.
        /// </summary>
        public string ConfigurationLocation { get; set; }
        #endregion

        #region MigrateWebDashboard
        /// <summary>
        /// Should the web dashboard be migrated.
        /// </summary>
        public bool MigrateWebDashboard { get; set; }
        #endregion

        #region CurrentWebDashboardLocation
        /// <summary>
        /// The location of the web dashboard files.
        /// </summary>
        public string CurrentWebDashboardLocation { get; set; }
        #endregion

        #region NewWebDashboardLocation
        /// <summary>
        /// The location of the web dashboard files.
        /// </summary>
        public string NewWebDashboardLocation { get; set; }
        #endregion

        #region CurrentVersion
        /// <summary>
        /// The current version of CruiseControl.NET.
        /// </summary>
        public string CurrentVersion { get; set; }
        #endregion
        #endregion
    }
}
