namespace ThoughtWorks.CruiseControl.Core.Storage
{
    using System.IO;
    using System.Xml.Serialization;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Remote;
    using Newtonsoft.Json;

    /// <summary>
    /// Stores project data in JSON files in a folder.
    /// </summary>
    [ReflectorType("jsonFolderData")]
    public class JsonFolderDataStore
        : BaseFolderDataStore, IDataStore
    {
        #region Private fields
        private static readonly JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
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
            Log.Info("Writing snapshot (JSON) to [" + dirPath + "]");

            var logFile = new LogFile(result);
            var filePath = Path.ChangeExtension(
                Path.Combine(dirPath, logFile.Filename),
                "snapshot");
            this.FileSystem.EnsureFolderExists(filePath);
            Log.Debug("Creating new snapshot (JSON) [" + filePath + "]");
            using (var stream = this.FileSystem.OpenOutputStream(filePath))
            {
                using (var writer = new StreamWriter(stream))
                {
                    Log.Debug("Writing snapshot (JSON)");
                    var json = JsonConvert.SerializeObject(snapshot, Formatting.None, settings);
                    writer.Write(json);
                    Log.Debug("Snapshot (JSON) written");
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
            Log.Info("Loading snapshot (JSON) from [" + dirPath + "]");

            var snapshotPath = Path.Combine(
                this.RootFolder(project.ArtifactDirectory, this.SnapshotsFolder),
                Path.ChangeExtension(buildName, "snapshot"));
            if (!this.FileSystem.FileExists(snapshotPath))
            {
                Log.Debug("Unable to find snapshot (JSON) file [" + snapshotPath + "]");
                return null;
            }

            using (var stream = this.FileSystem.OpenInputStream(snapshotPath))
            {
                using (var reader = new StreamReader(stream))
                {
                    Log.Debug("Loading snapshot (JSON) file [" + snapshotPath + "]");
                    var json = reader.ReadToEnd();
                    var snapshot = JsonConvert.DeserializeObject<ProjectStatusSnapshot>(json, settings);
                    return snapshot;
                }
            }
        }
        #endregion
        #endregion
    }
}
