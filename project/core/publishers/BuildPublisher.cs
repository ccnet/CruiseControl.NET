using System.IO;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    [ReflectorType("buildpublisher")]
    public class BuildPublisher : ITask
    {
        public enum CleanupPolicy
        {
            NoCleaning,
            KeepLastXBuilds,
            DeleteBuildsOlderThanXDays
        }



        [ReflectorProperty("publishDir", Required = false)]
        public string PublishDir;

        [ReflectorProperty("sourceDir", Required = false)]
        public string SourceDir;

        [ReflectorProperty("useLabelSubDirectory", Required = false)]
        public bool UseLabelSubDirectory = true;

        [ReflectorProperty("alwaysPublish", Required = false)]
        public bool AlwaysPublish = false;

        /// <summary>
        /// Description used for the visualisation of the buildstage, if left empty the process name will be shown
        /// </summary>
        [ReflectorProperty("description", Required = false)]
        public string Description = string.Empty;


        [ReflectorProperty("cleanUpMethod", Required = false)]
        public CleanupPolicy CleanUpMethod = CleanupPolicy.NoCleaning;

        /// <summary>
        /// Defines the value for the cleanup procedure
        /// </summary>
        [ReflectorProperty("cleanUpValue", Required = false)]
        public int cleanUpValue = 5;


        public void Run(IIntegrationResult result)
        {

            result.BuildProgressInformation.SignalStartRunTask(Description != string.Empty ? Description : "Publishing build results");

            if (result.Succeeded || AlwaysPublish)
            {
                DirectoryInfo srcDir = new DirectoryInfo(result.BaseFromWorkingDirectory(SourceDir));
                DirectoryInfo pubDir = new DirectoryInfo(result.BaseFromArtifactsDirectory(PublishDir));
                if (!pubDir.Exists) pubDir.Create();

                if (UseLabelSubDirectory)
                    pubDir = pubDir.CreateSubdirectory(result.Label);

                RecurseSubDirectories(srcDir, pubDir);

                switch (CleanUpMethod)
                {
                    case CleanupPolicy.NoCleaning:
                        break;

                    case CleanupPolicy.DeleteBuildsOlderThanXDays:
                        DeleteSubDirsOlderThanXDays(new DirectoryInfo(result.BaseFromWorkingDirectory(SourceDir)).FullName,
                                                    cleanUpValue, result.BuildLogDirectory);
                        break;

                    case CleanupPolicy.KeepLastXBuilds:
                        KeepLastXSubDirs(new DirectoryInfo(result.BaseFromWorkingDirectory(SourceDir)).FullName,
                                                    cleanUpValue, result.BuildLogDirectory);
                        break;

                    default:
                        throw new System.Exception(string.Format("unmapped cleaning method choosen {0}", CleanUpMethod));
                }
            }
        }

        private void RecurseSubDirectories(DirectoryInfo srcDir, DirectoryInfo pubDir)
        {
            FileInfo[] files = srcDir.GetFiles();
            foreach (FileInfo file in files)
            {
                FileInfo destFile = new FileInfo(Path.Combine(pubDir.FullName, file.Name));
                if (destFile.Exists) destFile.Attributes = FileAttributes.Normal;

                file.CopyTo(destFile.ToString(), true);
            }
            DirectoryInfo[] subDirectories = srcDir.GetDirectories();
            foreach (DirectoryInfo subDir in subDirectories)
            {
                DirectoryInfo subDestination = pubDir.CreateSubdirectory(subDir.Name);

                RecurseSubDirectories(subDir, subDestination);
            }
        }


        private void KeepLastXSubDirs(string targetFolder, int amountToKeep, string buildLogDirectory)
        {
            System.Collections.Generic.List<string> sortNames = new System.Collections.Generic.List<string>();
            const string dateFormat = "yyyyMMddHHmmssffffff";

            foreach (string folder in Directory.GetDirectories(targetFolder))
            {
                if (folder != buildLogDirectory)
                    sortNames.Add(Directory.GetCreationTime(folder).ToString(dateFormat) + folder);
            }

            sortNames.Sort();

            int amountToDelete = sortNames.Count - amountToKeep;
            for (int i = 0; i < amountToDelete; i++)
            {
                DeleteFolder(sortNames[0].Substring(dateFormat.Length));
                sortNames.RemoveAt(0);
            }
        }

        private void DeleteSubDirsOlderThanXDays(string targetFolder, int daysToKeep, string buildLogDirectory)
        {
            System.DateTime cutoffDate = System.DateTime.Now.Date.AddDays(-daysToKeep);

            foreach (string folder in Directory.GetDirectories(targetFolder))
            {
                if ((Directory.GetCreationTime(folder).Date < cutoffDate) &&
                    (folder != buildLogDirectory))
                    DeleteFolder(folder);
            }
        }

        private void DeleteFolder(string folderName)
        {
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



    }
}