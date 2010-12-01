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
            this.Folder = "snapshots";
            this.FileSystem = new SystemIoFileSystem();
        }
        #endregion

        #region Public properties
        #region Folder
        /// <summary>
        /// The folder to store the XML in.
        /// </summary>
        /// <version>1.6</version>
        /// <default>snapshots</default>
        /// <remarks>
        /// If this is not an absolute folder then it will be rooted in the project's
        /// artefacts folder.
        /// </remarks>
        [ReflectorProperty("folder", Required = false)]
        public string Folder { get; set; }
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
            var dirPath = Path.IsPathRooted(this.Folder) ?
                this.Folder :
                result.BaseFromArtifactsDirectory(this.Folder);
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
    }
}
