namespace ThoughtWorks.CruiseControl.Core.Storage
{
    using System.IO;
    using System.Xml.Serialization;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// Base class for providing common functionality in folde-based data stores.
    /// </summary>
    public abstract class BaseFolderDataStore
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFolderDataStore"/> class.
        /// </summary>
        public BaseFolderDataStore()
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

        #region Protected methods
        #region RootFolder()
        /// <summary>
        /// Roots a folder.
        /// </summary>
        /// <param name="artefactsFolder">The artefacts folder.</param>
        /// <param name="folder">The folder to root.</param>
        /// <returns>The rooted folder.</returns>
        protected string RootFolder(string artefactsFolder, string folder)
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

                return Path.Combine(
                    artefactsFolder,
                    Path.Combine(this.BaseFolder, folder));
            }

            return Path.Combine(artefactsFolder, folder);
        }
        #endregion
        #endregion
    }
}
