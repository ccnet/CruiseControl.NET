namespace ThoughtWorks.CruiseControl.Core.Storage
{
    using System.IO;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Remote;
    using Exortech.NetReflector;

    /// <summary>
    /// Stores project data in XML files in a folder.
    /// </summary>
    [ReflectorType("xmlFolderData")]
    public class XmlFolderDataStore
        : IDataStore
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlFolderDataStore"/> class.
        /// </summary>
        public XmlFolderDataStore()
        {
            this.BaseFolder = string.Empty;
            this.SnapshotsFolder = "snapshots";
            this.FileSystem = new SystemIoFileSystem();
        }
        #endregion

        #region Public properties
        #region BaseFolder
        /// <summary>
        /// The base folder to store the XML in.
        /// </summary>
        /// <version>1.6</version>
        /// <default>snapshots</default>
        /// <remarks>
        /// If this is not an absolute folder then it will be rooted in the project's
        /// artefacts folder.
        /// </remarks>
        [ReflectorProperty("base", Required = false)]
        public string BaseFolder { get; set; }
        #endregion

        #region SnapshotsFolder
        /// <summary>
        /// The folder to store snapshots.
        /// </summary>
        /// <version>1.6</version>
        /// <default>snapshots</default>
        /// <remarks>
        /// If this is not an absolute folder then it will be relative to the base folder.
        /// </remarks>
        [ReflectorProperty("snapshot", Required = false)]
        public string SnapshotsFolder { get; set; }
        #endregion

        #region FileSystem
        /// <summary>
        /// Gets or sets the file system.
        /// </summary>
        /// <value>The file system.</value>
        public IFileSystem FileSystem { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region StoreProjectSnapshot()
        /// <summary>
        /// Stores a snapshot of a project build.
        /// </summary>
        /// <param name="result">The result that the snapshot is for.</param>
        /// <param name="snapshot">The project snapshot.</param>
        public void StoreProjectSnapshot(IIntegrationResult result, ItemStatus snapshot)
        {
            Log.Debug("Initialising folder");
            var dirPath = this.RootFolder(result, this.SnapshotsFolder);
            Log.Info("Writing snapshot to [" + dirPath + "]");

            var logFile = new LogFile(result);
            var filePath = Path.ChangeExtension(
                Path.Combine(dirPath, logFile.Filename),
                "snapshot");
            this.FileSystem.EnsureFolderExists(filePath);
            Log.Debug("Creating new snapshot [" + filePath + "]");
            using (var stream = this.FileSystem.OpenOutputStream(filePath))
            {
                using (var writer = new StreamWriter(stream))
                {
                    Log.Debug("Writing snapshot");
                    writer.Write(snapshot.ToString());
                    Log.Debug("Snapshot written");
                }
            }
        }
        #endregion
        #endregion

        #region Private methods
        #region RootFolder()
        /// <summary>
        /// Roots a folder.
        /// </summary>
        /// <param name="result">The result to use.</param>
        /// <param name="folder">The folder to root.</param>
        /// <returns>The rooted folder.</returns>
        private string RootFolder(IIntegrationResult result, string folder)
        {
            if (Path.IsPathRooted(folder))
            {
                return folder;
            }
            else if (!string.IsNullOrEmpty(this.BaseFolder))
            {
                if (Path.IsPathRooted(this.BaseFolder))
                {
                    return Path.Combine(this.BaseFolder, folder);
                }

                return result.BaseFromArtifactsDirectory(
                        Path.Combine(this.BaseFolder, folder));
            }

            return result.BaseFromArtifactsDirectory(folder);
        }
        #endregion
        #endregion
    }
}
