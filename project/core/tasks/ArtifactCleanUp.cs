using System.Globalization;
namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Publishers;
    using ThoughtWorks.CruiseControl.Core.Util;

    /// <summary>
    /// The artifact CleanUp publisher allows for automatic removal of the buildlogs according to the choosen
    /// setting. It relies on the build log folder, so the XML publisher must be specified before this
    /// publisher can run. For technical reasons this publisher MUST reside in the publisher section, it will
    /// not work in the tasks section. Be sure to specify the <link>Xml Log Publisher</link> before this one.
    /// </summary>
    /// <title> Artifact Cleanup Publisher </title>
    /// <version>1.5</version>
    /// <remarks>
    /// <para>
    /// Supported cleaning up methods :
    /// </para>
    /// <list type="1">
    /// <item>
    /// KeepLastXBuilds : keeps the last specified amount of builds
    /// </item>
    /// <item>
    /// DeleteBuildsOlderThanXDays  : Deletes the builds older than the specifed amount of days
    /// </item>
    /// <item>
    /// KeepMaximumXHistoryDataEntries : Clears the History Data file (for the ModificationHistory), keeping
    /// maximum the specified amount of builds.
    /// </item>
    /// <item>
    /// DeleteSubDirsOlderThanXDays : Deletes subfolders of the artifact folder if they are older than the
    /// specified amount of days. (Buildlogfolder excluded)
    /// </item>
    /// <item>
    /// KeepLastXSubDirs : Keeps the last specified amount of subfolders in the artifacts folder, sorting is
    /// done on creation time of the folder (Buildlogfolder excluded) 
    /// </item>
    /// </list>
    /// <para type="warning">
    /// <b>DeleteSubDirsOlderThanXDays</b> and <b>KeepLastXSubDirs</b> are mainly meant for cleaning up
    /// published builds (done via the <link>Build Publisher</link>).
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// &lt;artifactcleanup cleanUpMethod="KeepLastXBuilds" cleanUpValue="50" /&gt;
    /// </code>
    /// </example>
    [ReflectorType("artifactcleanup")]
    public class ArtifactCleanUpTask
        : TaskBase
    {
        /// <summary>
        /// Supported cleaning up methods
        /// </summary>
        public enum CleanUpMethod
        {
            /// <summary>
            /// keeps the last specified amount of builds
            /// </summary>
            KeepLastXBuilds,
            /// <summary>
            /// Deletes the builds older than the specifed amount of days
            /// </summary>
            DeleteBuildsOlderThanXDays,
            /// <summary>
            /// Clears the History Data file (for the ModificationHistory), keeping maximum the specified amount of builds.
            /// </summary>
            KeepMaximumXHistoryDataEntries,
            /// <summary>
            /// Deletes subfolders of the artifact folder if they are older than the
            /// specified amount of days. (Buildlogfolder excluded)
            /// </summary>
            DeleteSubDirsOlderThanXDays,
            /// <summary>
            /// Keeps the last specified amount of subfolders in the artifacts folder, sorting is
            /// done on creation time of the folder (Buildlogfolder excluded) 
            /// </summary>
            KeepLastXSubDirs
        }

        private CleanUpMethod cleanUpMethod;
        private int cleanUpValue;

        /// <summary>
        /// Defines the procedure to use for cleaning up the artifact folder.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("cleanUpMethod", Required = true)]
        public CleanUpMethod CleaningUpMethod
        {
            get { return cleanUpMethod; }
            set { cleanUpMethod = value; }
        }

        /// <summary>
        /// Defines the value for the cleanup procedure.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("cleanUpValue", Required = true)]
        public int CleaningUpValue
        {
            get { return cleanUpValue; }
            set { cleanUpValue = value; }
        }

        /// <summary>
        /// Executes the specified result.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected override bool Execute(IIntegrationResult result)
        {
            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description) ? Description : "Cleaning up");

            switch (cleanUpMethod)
            {
                case CleanUpMethod.KeepLastXBuilds:
                    if (BuildLogFolderSet(result))
                        KeepLastXBuilds(result.BuildLogDirectory, CleaningUpValue);
                    break;

                case CleanUpMethod.DeleteBuildsOlderThanXDays:
                    if (BuildLogFolderSet(result))
                        DeleteBuildsOlderThanXDays(result.BuildLogDirectory, CleaningUpValue);
                    break;

                case CleanUpMethod.KeepMaximumXHistoryDataEntries:
                    KeepMaximumXHistoryDataEntries(result, cleanUpValue);
                    break;

                case CleanUpMethod.DeleteSubDirsOlderThanXDays:
                    DeleteSubDirsOlderThanXDays(result, cleanUpValue);
                    break;

                case CleanUpMethod.KeepLastXSubDirs:
                    KeepLastXSubDirs(result, cleanUpValue);
                    break;

                default:
                    throw new NotImplementedException("Unmapped cleaning up method used");
            }

            return true;
        }

        private void KeepLastXSubDirs(IIntegrationResult result, int amountToKeep)
        {
            List<string> sortNames = new List<string>();
            const string dateFormat = "yyyyMMddHHmmssffffff";

            Util.Log.Debug("Deleting Subdirs of {0}", result.ArtifactDirectory);

            foreach (string folder in Directory.GetDirectories(result.ArtifactDirectory))
            {
                if (folder != result.BuildLogDirectory)
                    sortNames.Add(Directory.GetCreationTime(folder).ToString(dateFormat, CultureInfo.CurrentCulture) + folder);
            }

            sortNames.Sort();

            int amountToDelete = sortNames.Count - amountToKeep;
            for (int i = 0; i < amountToDelete; i++)
            {
                DeleteFolder(sortNames[0].Substring(dateFormat.Length));
                sortNames.RemoveAt(0);
            }
        }

        private void DeleteSubDirsOlderThanXDays(IIntegrationResult result, int daysToKeep)
        {
            Util.Log.Debug("Deleting Subdirs of {0}", result.ArtifactDirectory);

            DateTime cutoffDate = DateTime.Now.Date.AddDays(-daysToKeep);


            foreach (string folder in Directory.GetDirectories(result.ArtifactDirectory))
            {
                if ((Directory.GetCreationTime(folder).Date < cutoffDate) &&
                    (folder != result.BuildLogDirectory))
                    DeleteFolder(folder);
            }
        }

        private void DeleteFolder(string folderName)
        {
            Util.Log.Debug("Deleting {0}", folderName);

            SetFilesToNormalAttributeAndDelete(folderName);
            Directory.Delete(folderName);
        }

        private void SetFilesToNormalAttributeAndDelete(string folderName)
        {
            foreach (string file in Directory.GetFiles(folderName))
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string subFolder in Directory.GetDirectories(folderName))
            {
                DeleteFolder(subFolder);
            }
        }

        private void SetFilesToNormalAttribute(string folderName)
        {
            foreach (string file in Directory.GetFiles(folderName))
            {
                File.SetAttributes(file, FileAttributes.Normal);
            }
        }

        private bool BuildLogFolderSet(IIntegrationResult result)
        {
            string buildLogFolder = result.BuildLogDirectory;

            if (string.IsNullOrEmpty(buildLogFolder))
            {
                Log.Debug(
                    "Cleaning up the artifact folder not possible because the buildlog folder is NULL. \n Check that the XML Log publisher is before the Artifacts Cleanup publisher in the config.");
                return false;
            }

            return true;
        }

        private void DeleteBuildsOlderThanXDays(string buildLogFolder, int daysToKeep)
        {
            SetFilesToNormalAttribute(buildLogFolder);

            foreach (string filename in Directory.GetFiles(buildLogFolder))
            {
                if (File.GetCreationTime(filename).Date < DateTime.Now.Date.AddDays(-daysToKeep))
                    File.Delete(filename);
            }
        }

        private void KeepLastXBuilds(string buildLogFolder, int buildToKeep)
		{

            SetFilesToNormalAttribute(buildLogFolder);

            // gets the buildlogs themselves, skip the summary files from this list
			List<string> buildLogs = new List<string>(Directory.GetFiles(buildLogFolder,"*.xml"));
			buildLogs.Sort();

			while (buildLogs.Count > buildToKeep)
			{
                File.Delete(buildLogs[0]);
				buildLogs.RemoveAt(0);
			}

            // remove all summary files which do not have a corresponding build file
            if (buildLogs.Count > 0)
            {                
                List<string> summaryFiles = new List<string>(Directory.GetFiles(buildLogFolder, "*.summary"));
                
                foreach (string summary in summaryFiles)
                {
                    string correspondingBuildFile = summary.Replace(".summary", ".xml");

                    if (!buildLogs.Contains(correspondingBuildFile)) File.Delete(summary);
                }

            }

		}

        private void KeepMaximumXHistoryDataEntries(IIntegrationResult result, int entriesToKeep)
        {
            string historyXml = ModificationHistoryPublisher.LoadHistory(result.ArtifactDirectory);

            if (historyXml.Length == 0)
                return;

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(historyXml);

            //if (doc.FirstChild.ChildNodes.Count == 0)
            int nodeCount = doc.FirstChild.ChildNodes.Count;
            if (nodeCount <= entriesToKeep)
                return;


            StringWriter cleanedHistory = new StringWriter();

            for (int i = nodeCount - entriesToKeep; i < nodeCount; i++)
            {
                cleanedHistory.WriteLine(doc.FirstChild.ChildNodes[i].OuterXml);
            }

            StreamWriter historyWriter = new StreamWriter(
                Path.Combine(result.ArtifactDirectory,
                             ModificationHistoryPublisher.DataHistoryFileName));

            historyWriter.WriteLine(cleanedHistory.ToString());
            historyWriter.Close();
        }
    }
}
