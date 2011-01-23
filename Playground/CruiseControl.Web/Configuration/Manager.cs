namespace CruiseControl.Web.Configuration
{
    using System;
    using System.IO;
    using System.Xaml;
    using System.Xml;
    using Utilities;

    /// <summary>
    /// Manages the configuration settings.
    /// </summary>
    public class Manager
    {
        #region Private fields
        private static Settings CurrentSettings;
        private static readonly object LoadLock = new object();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes the <see cref="Manager"/> class.
        /// </summary>
        static Manager()
        {
            FileSystem = new FileSystem();
        }
        #endregion

        #region Public properties
        #region Current
        /// <summary>
        /// Gets or sets the current settings.
        /// </summary>
        /// <value>The current settings.</value>
        public static Settings Current
        {
            get
            {
                if (CurrentSettings == null)
                {
                    Reload(false);
                }

                return CurrentSettings;
            }

            set { CurrentSettings = value; }
        }
        #endregion

        #region FileSystem
        /// <summary>
        /// Gets or sets the file system.
        /// </summary>
        /// <value>The file system.</value>
        public static FileSystem FileSystem { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region Reset()
        /// <summary>
        /// Resets the current settings.
        /// </summary>
        public static void Reset()
        {
            FileSystem = new FileSystem();
            lock (LoadLock)
            {
                CurrentSettings = null;
            }
        }
        #endregion

        #region Reload()
        /// <summary>
        /// Reloads the current settings.
        /// </summary>
        /// <param name="forceReload">If set to <c>true</c> then the current settings will be overwritten.</param>
        public static void Reload(bool forceReload)
        {
            lock (LoadLock)
            {
                if (!(forceReload || (CurrentSettings == null)))
                {
                    return;
                }

                var filePath = Path.Combine(Folders.DataDirectory, "web.settings");
                if (FileSystem.CheckFileExists(filePath))
                {
                    using (var reader = FileSystem.ReadFromFile(filePath))
                    {
                        Current = XamlServices.Load(reader) as Settings;
                    }
                }
                else
                {
                    // Define the default settings
                    Current = new Settings();
                    Current.ReportLevels.Add(new ReportLevel {Target = ActionHandlerTargets.Root});
                    Current.ReportLevels.Add(new ReportLevel {Target = ActionHandlerTargets.Server});
                    Current.ReportLevels.Add(new ReportLevel {Target = ActionHandlerTargets.Project});
                    Current.ReportLevels.Add(new ReportLevel {Target = ActionHandlerTargets.Build});
                }
            }
        }
        #endregion

        #region Update()
        /// <summary>
        /// Updates the settings.
        /// </summary>
        /// <param name="newSettings">The new settings.</param>
        public static void Update(Settings newSettings)
        {
            // Save to a file
            var filePath = Path.Combine(Folders.DataDirectory, "web.settings");
            using (var writer = FileSystem.WriteToFile(filePath))
            {
                var settings = new XmlWriterSettings
                                   {
                                       CheckCharacters = true,
                                       Indent = true,
                                       NamespaceHandling = NamespaceHandling.OmitDuplicates,
                                       NewLineHandling = NewLineHandling.Entitize,
                                       NewLineOnAttributes = false,
                                       OmitXmlDeclaration = true
                                   };
                using (var xmlWriter = XmlWriter.Create(writer, settings))
                {
                    XamlServices.Save(xmlWriter, newSettings);
                }
            }

            // Update the current settings
            lock (LoadLock)
            {
                CurrentSettings = newSettings;
            }
        }
        #endregion
        #endregion
    }
}