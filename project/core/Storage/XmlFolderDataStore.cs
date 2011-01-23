namespace ThoughtWorks.CruiseControl.Core.Storage
{
    using System.IO;
    using System.Xml.Serialization;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// Stores project data in XML files in a folder.
    /// </summary>
    [ReflectorType("xmlFolderData")]
    public class XmlFolderDataStore
        : BaseFolderDataStore, IDataStore
    {
        #region Private fields
        private static readonly XmlSerializer serialiser = new XmlSerializer(typeof(ProjectStatusSnapshot));
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
            var dirPath = this.RootFolder(result.ArtifactDirectory, this.SnapshotsFolder);
            Log.Info("Writing snapshot (XML) to [" + dirPath + "]");

            var logFile = new LogFile(result);
            var filePath = Path.ChangeExtension(
                Path.Combine(dirPath, logFile.Filename),
                "snapshot");
            this.FileSystem.EnsureFolderExists(filePath);
            Log.Debug("Creating new snapshot (XML) [" + filePath + "]");
            using (var stream = this.FileSystem.OpenOutputStream(filePath))
            {
                using (var writer = new StreamWriter(stream))
                {
                    Log.Debug("Writing snapshot (XML)");
                    writer.Write(snapshot.ToString());
                    Log.Debug("Snapshot (XML) written");
                }
            }
        }
        #endregion

        #region LoadProjectSnapshot()
        /// <summary>
        /// Loads the project snapshot for a build.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="buildName">Name of the build.</param>
        /// <returns>The project snapshot.</returns>
        public ItemStatus LoadProjectSnapshot(IProject project, string buildName)
        {
            var dirPath = this.RootFolder(project.ArtifactDirectory, this.SnapshotsFolder);
            Log.Info("Loading snapshot (XML) from [" + dirPath + "]");

            var snapshotPath = Path.Combine(
                this.RootFolder(project.ArtifactDirectory, this.SnapshotsFolder),
                Path.ChangeExtension(buildName, "snapshot"));
            if (!this.FileSystem.FileExists(snapshotPath))
            {
                Log.Debug("Unable to find snapshot (XML) file [" + snapshotPath + "]");
                return null;
            }

            using (var stream = this.FileSystem.OpenInputStream(snapshotPath))
            {
                Log.Debug("Loading snapshot (XML) file [" + snapshotPath + "]");
                var snapshot = serialiser.Deserialize(stream) as ProjectStatusSnapshot;
                return snapshot;
            }
        }
        #endregion
        #endregion
    }
}
