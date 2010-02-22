using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System.Xml.Serialization;
using System.Xml;
using System.Diagnostics;
using ThoughtWorks.CruiseControl.WebDashboard.Configuration;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.Administration
{
    /// <summary>
    /// Defines a plug-in package.
    /// </summary>
    /// <remarks>
    /// A plug-in package contains all the files and configuration to get a plug-in working
    /// in the web dashboard.
    /// </remarks>
    public class Package
        : IDisposable
    {
        #region Private constants
        private const int blockSize = 16384;
        #endregion

        #region Private fields
        private PackageManifest manifest;
        private List<string> files = new List<string>();
        private string tempFolder;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialises a new package from a stream.
        /// </summary>
        /// <param name="packageStream">The stream that contains the package.</param>
        public Package(Stream packageStream)
        {
            // Extract all the files into a temporary location
            ExtractAllFiles(packageStream);

            // Attempt to load the manifest
            string manifestFile = Path.Combine(tempFolder, "Manifest.xml");
            if (File.Exists(manifestFile))
            {
                XmlSerializer serialiser = new XmlSerializer(typeof(PackageManifest));
                using (Stream inputStream = File.OpenRead(manifestFile))
                {
                    manifest = serialiser.Deserialize(inputStream) as PackageManifest;
                    inputStream.Close();
                }
            }
        }
        #endregion

        #region Public properties
        #region Manifest
        /// <summary>
        /// The manifest of the package.
        /// </summary>
        public PackageManifest Manifest
        {
            get { return manifest; }
            set { manifest = value; }
        }
        #endregion

        #region IsValid
        /// <summary>
        /// Returns true if this is a valid package.
        /// </summary>
        public bool IsValid
        {
            get { return (manifest != null); }
        }
        #endregion
        #endregion

        #region Public methods
        #region Install()
        /// <summary>
        /// Installs the package to the specified location.
        /// </summary>
        public void Install()
        {
            if (!IsValid) throw new ApplicationException("This is not a valid package");

            // Start the stopwatch
            FireMessage(TraceLevel.Info, "Starting installation");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                // Copy all the files
                FireMessage(TraceLevel.Verbose, "Installing files");
                foreach (FileLocation folder in manifest.FileLocations)
                {
                    CopyFiles(folder);
                }

                // Add all the configuration settings
                FireMessage(TraceLevel.Verbose, "Updating configuration");
                UpdateConfigurationFile(true);
            }
            catch (Exception error)
            {
                // Tell the calling app that an unexpected error occurred
                FireMessage(TraceLevel.Error,
                    "An unexpected error occurred during the installation: '{0}'",
                    error.Message);
            }
            finally
            {
                // Display the elapsed time
                stopwatch.Stop();
                double time = stopwatch.ElapsedMilliseconds;
                FireMessage(TraceLevel.Info,
                    "Installation has completed ({0:0.00}s)",
                    time / 1000);
            }
        }
        #endregion

        #region Uninstall()
        /// <summary>
        /// Uninstalls the package from the specified location.
        /// </summary>
        public void Uninstall()
        {
            if (!IsValid) throw new ApplicationException("This is not a valid package");

            // Start the stopwatch
            FireMessage(TraceLevel.Info, "Starting uninstallation");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                // Delete all the files
                FireMessage(TraceLevel.Verbose, "Removing files");
                foreach (FileLocation folder in manifest.FileLocations)
                {
                    RemoveFiles(folder);
                }

                // Remove all the configuration settings
                FireMessage(TraceLevel.Verbose, "Updating configuration");
                UpdateConfigurationFile(false);
            }
            catch (Exception error)
            {
                // Tell the calling app that an unexpected error occurred
                FireMessage(TraceLevel.Error,
                    "An unexpected error occurred during the uninstallation: '{0}'",
                    error.Message);
            }
            finally
            {
                // Display the elapsed time
                stopwatch.Stop();
                double time = stopwatch.ElapsedMilliseconds;
                FireMessage(TraceLevel.Info,
                    "Uninstallation has completed ({0:0.00}s)",
                    time / 1000);
            }
        }
        #endregion

        #region Dispose()
        /// <summary>
        /// Removes all the temporary files.
        /// </summary>
        public void Dispose()
        {
            foreach (string fileName in files)
            {
                if (File.Exists(fileName)) File.Delete(fileName);
            }
        }
        #endregion
        #endregion

        #region Public events
        /// <summary>
        /// A message for the calling application.
        /// </summary>
        public event EventHandler<PackageImportEventArgs> Message;
        #endregion

        #region Private methods
        #region FireMessage()
        /// <summary>
        /// Fires the <see cref="Message"/> event.
        /// </summary>
        /// <param name="message">The message to pass.</param>
        /// <param name="level">The level of the message.</param>
        /// <param name="values">The values for the message.</param>
        private void FireMessage(TraceLevel level, string message, params object[] values)
        {
            if (message != null)
            {
                string fullMessage = string.Format(message, values);
                PackageImportEventArgs args = new PackageImportEventArgs(fullMessage, level);
                Message(this, args);
            }
        }
        #endregion

        #region ExtractAllFiles()
        /// <summary>
        /// Extracts all the files to a temporary location
        /// </summary>
        /// <param name="packageStream">The stream to load the files from.</param>
        private void ExtractAllFiles(Stream packageStream)
        {
            tempFolder = Path.GetTempPath();
            ZipInputStream zipIn = new ZipInputStream(packageStream);
            ZipEntry entry = zipIn.GetNextEntry();
            while (entry != null)
            {
                string fileName = Path.Combine(tempFolder, entry.Name);
                using (FileStream outputWriter = File.Create(fileName))
                {
                    byte[] data = new byte[blockSize];
                    int dataLength = zipIn.Read(data, 0, blockSize);
                    while (dataLength > 0)
                    {
                        outputWriter.Write(data, 0, dataLength);
                        dataLength = zipIn.Read(data, 0, blockSize);
                    }
                    outputWriter.Close();
                }
                files.Add(fileName);
                entry = zipIn.GetNextEntry();
            }
        }
        #endregion

        #region CopyFiles()
        /// <summary>
        /// Copies files to the install location.
        /// </summary>
        /// <param name="folder">The details of the files to copy.</param>
        private void CopyFiles(FileLocation folder)
        {
            // Make sure the target folder exists
            FireMessage(TraceLevel.Verbose, "Copying files to target '{0}'", folder.Location);
            string target = ProgramDataFolder.MapPath(folder.Location);
            if (!Directory.Exists(target))
            {
                FireMessage(TraceLevel.Info, "Adding target folder '{0}'", folder.Location);
                Directory.CreateDirectory(target);
            }
            foreach (string file in folder.Files)
            {
                // Make sure the source file exists
                string source = Path.Combine(tempFolder, file);
                if (File.Exists(source))
                {
                    // Set to overwrite in case there is an older version of the file
                    FireMessage(TraceLevel.Info, "Deploying file '{0}' to target '{1}'", file, folder.Location);
                    File.Copy(source, Path.Combine(target, file), true);
                }
                else
                {
                    FireMessage(TraceLevel.Warning, "Source file '{0}' does not exist in package", file);
                }
            }
        }
        #endregion

        #region RemoveFiles()
        /// <summary>
        /// Removes files to the install location.
        /// </summary>
        /// <param name="folder">The details of the files to remove.</param>
        private void RemoveFiles(FileLocation folder)
        {
            // Make sure the target folder exists
            FireMessage(TraceLevel.Verbose, "Removing files from target '{0}'", folder.Location);
            string target = ProgramDataFolder.MapPath(folder.Location);
            if (Directory.Exists(target))
            {
                foreach (string file in folder.Files)
                {
                    // Make sure the file exists
                    string targetFile = Path.Combine(target, file);
                    if (File.Exists(targetFile))
                    {
                        // Set to overwrite in case there is an older version of the file
                        FireMessage(TraceLevel.Info, "Removing file '{0}' from target '{1}'", file, folder.Location);
                        File.Delete(targetFile);
                    }
                }
            }
        }
        #endregion

        #region UpdateConfigurationSetting()
        /// <summary>
        /// Updates a setting in the config file.
        /// </summary>
        /// <param name="configXml">The XML document contains the configuration.</param>
        /// <param name="setting">The setting to add.</param>
        /// <param name="addSettings"></param>
        private void UpdateConfigurationSetting(XmlDocument configXml, ConfigurationSetting setting, bool addSettings)
        {
            // Attempt to find the parent element
            XmlElement element = configXml.SelectSingleNode(setting.Path) as XmlElement;
            if (element != null)
            {
                FireMessage(TraceLevel.Verbose, "Element found ('{0}'), updating", setting.Path);
                string xpath = string.IsNullOrEmpty(setting.Filter) ? 
                    setting.Name : 
                    string.Format("{0}[{1}]", setting.Name, setting.Filter);
                if (addSettings)
                {
                    // Otherwise see if there is an old element, if there is then overwrite it, otherwise
                    // a new element must be added
                    XmlNode oldElement = element.SelectSingleNode(xpath);
                    if (oldElement != null)
                    {
                        FireMessage(TraceLevel.Info,
                            "Setting element '{0}' on element '{1}' to value '{2}'",
                            setting.Name,
                            setting.Path,
                            setting.Value);
                        oldElement.InnerText = setting.Value;
                        element = oldElement as XmlElement;
                    }
                    else
                    {
                        FireMessage(TraceLevel.Info,
                            "Adding new element '{0}' to element '{1}' with value '{2}'",
                            setting.Name,
                            setting.Path,
                            setting.Value);
                        XmlElement newElement = configXml.CreateElement(setting.Name);
                        newElement.InnerText = setting.Value;
                        element.AppendChild(newElement);
                        element = newElement;
                    }

                    // Set the attributes
                    foreach (ConfigurationAttribute attribute in setting.Attributes ?? new ConfigurationAttribute[0])
                    {
                        FireMessage(TraceLevel.Info,
                            "Setting attribute '{0}' on element '{1}' to value '{2}'",
                            attribute.Name,
                            setting.Name,
                            attribute.Value);
                        element.SetAttribute(attribute.Name, attribute.Value);
                    }
                }
                else
                {
                    // See if the element is there
                    XmlNode oldElement = element.SelectSingleNode(xpath);
                    if (oldElement != null)
                    {
                        FireMessage(TraceLevel.Info,
                            "Removing element '{0}' on element '{1}'",
                            setting.Name,
                            setting.Path);
                        oldElement.ParentNode.RemoveChild(oldElement);
                    }
                }
            }
            else
            {
                if (addSettings) FireMessage(TraceLevel.Warning, "Unable to find element '{0}'", setting.Path);
            }
        }
        #endregion

        #region UpdateConfigurationFile()
        /// <summary>
        /// Updates the configuration file.
        /// </summary>
        /// <param name="addSettings"></param>
        private void UpdateConfigurationFile(bool addSettings)
        {
            // Load the existing configuration file
            string configFile = DashboardConfigurationLoader.CalculateDashboardConfigPath();
            XmlDocument configXml = new XmlDocument();
            if (File.Exists(configFile))
            {
                FireMessage(TraceLevel.Verbose, "Loading configuration file");
                configXml.Load(configFile);

                // Add or update each setting
                bool configChanged = false;
                foreach (ConfigurationSetting setting in manifest.ConfigurationSettings)
                {
                    UpdateConfigurationSetting(configXml, setting, addSettings);
                    configChanged = true;
                }

                if (configChanged)
                {
                    // Save the updated configuration file
                    FireMessage(TraceLevel.Verbose, "Saving configuration file");
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.CloseOutput = true;
                    settings.ConformanceLevel = ConformanceLevel.Document;
                    settings.Indent = true;
                    using (XmlWriter writer = XmlWriter.Create(configFile, settings))
                    {
                        configXml.Save(writer);
                        writer.Close();
                    }
                }
            }
            else
            {
                FireMessage(TraceLevel.Warning, "Configuration file does not exist");
            }
        }
        #endregion
        #endregion
    }
}
