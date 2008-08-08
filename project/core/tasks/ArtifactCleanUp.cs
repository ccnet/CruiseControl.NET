using System;
using System.Collections.Generic;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;


namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// Purges the artifact folder according to the settings. 
    /// This allows to clean up the artifacts by ccnet itself, which is more neat. 
    /// </summary>
    [ReflectorType("artifactcleanup")]
    class ArtifactCleanUpTask : ITask
    {
        /// <summary>
        /// Supported cleaning up methods
        /// </summary>
        public enum CleanUpMethod
        {
            KeepLastXBuilds,
            DeleteBuildsOlderThanXDays,
            KeepMaximumXHistoryDataEntries,
            DeleteSubDirsOlderThanXDays,
            KeepLastXSubDirs
        }

        private CleanUpMethod cleanUpMethod;
        private Int32 cleanUpValue;

        /// <summary>
        /// Defines the procedure to use for cleaning up the artifact folder
        /// </summary>
        [ReflectorProperty("cleanUpMethod", Required = true)]
        public CleanUpMethod CleaningUpMethod
        {
            get { return cleanUpMethod; }
            set { cleanUpMethod = value; }
        }

        /// <summary>
        /// Defines the value for the cleanup procedure
        /// </summary>
        [ReflectorProperty("cleanUpValue", Required = true)]
        public Int32 CleaningUpValue
        {
            get { return cleanUpValue; }
            set { cleanUpValue = value; }
        }


        public void Run(IIntegrationResult result)
        {

            switch (cleanUpMethod)
            {
                case CleanUpMethod.KeepLastXBuilds:
                    if (BuildLogFolderSet(result))
                    {
                        KeepLastXBuilds(result.BuildLogDirectory, CleaningUpValue);
                    }
                    break;

                case CleanUpMethod.DeleteBuildsOlderThanXDays:
                    if (BuildLogFolderSet(result))
                    {
                        DeleteBuildsOlderThanXDays(result.BuildLogDirectory, CleaningUpValue);
                    }
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
        }


        private void KeepLastXSubDirs(IIntegrationResult result, Int32 amountToKeep)
        {
            string[] OldFolders;
            System.Collections.ArrayList sortingName = new System.Collections.ArrayList();
            
            const string dateFormat = "yyyyMMddHHmmssffffff";

            OldFolders = System.IO.Directory.GetDirectories(result.ArtifactDirectory);

            foreach (string OldFolder in OldFolders)
            {
                if (OldFolder != result.BuildLogDirectory)
                {
                    sortingName.Add(System.IO.Directory.GetCreationTime(OldFolder).ToString(dateFormat) + OldFolder);
                }
            }

            sortingName.Sort();

            int AmountToDelete = sortingName.Count - amountToKeep;

            for (int i = 0; i < AmountToDelete; i++)
            {
                string OldFolder = sortingName[0].ToString().Substring(dateFormat.Length);
                DeleteFolder(OldFolder);
                sortingName.RemoveAt(0); 
            }                        
        }


        private void DeleteSubDirsOlderThanXDays(IIntegrationResult result, Int32 daysToKeep)
        {
            string[] OldFolders;
            
            OldFolders = System.IO.Directory.GetDirectories(result.ArtifactDirectory);

            foreach (string OldFolder in OldFolders)
            {
                if ( (System.IO.Directory.GetCreationTime(OldFolder).Date < DateTime.Now.Date.AddDays(-daysToKeep)) && (OldFolder != result.BuildLogDirectory) )
                {
                    DeleteFolder(OldFolder);
                }
            }
        }






        private void DeleteFolder(string folderName)
        {
            SetFilesToNormalAttribute(folderName);
            System.IO.Directory.Delete(folderName);
        }

        private void SetFilesToNormalAttribute(string folderName)
        {
            foreach (string file in System.IO.Directory.GetFiles(folderName))
            {
                System.IO.File.SetAttributes(file, System.IO.FileAttributes.Normal);
                System.IO.File.Delete(file);
            }

            foreach (string subFolder in System.IO.Directory.GetDirectories(folderName))
            {
                DeleteFolder(subFolder);
            }
        }


        private bool BuildLogFolderSet(IIntegrationResult result)
        {
            string BuildLogFolder = result.BuildLogDirectory;

            if (BuildLogFolder == null || BuildLogFolder.Length == 0)
            {
                Log.Debug("Cleaning up the artifact folder not possible because the buildlog folder is NULL. \n Check that the XML Log publisher is before the Artifacts Cleanup publisher in the config.");
                return false;
            }
            else
            {
                return true;
            }
        }

        private void DeleteBuildsOlderThanXDays(string buildLogFolder, Int32 daysToKeep)
        {
            string[] OldFiles;

            OldFiles = System.IO.Directory.GetFiles(buildLogFolder);

            foreach (string OldFile in OldFiles)
            {
                if (System.IO.File.GetCreationTime(OldFile).Date < DateTime.Now.Date.AddDays(-daysToKeep))
                {
                    System.IO.File.Delete(OldFile);
                }
            }
        }


        private void KeepLastXBuilds(string buildLogFolder, Int32 buildToKeep)
        {
            System.Collections.ArrayList BuildLogFiles =
                new System.Collections.ArrayList(System.IO.Directory.GetFiles(buildLogFolder));

            BuildLogFiles.Sort();

            while (BuildLogFiles.Count > buildToKeep)
            {
                DeleteFile(BuildLogFiles[0].ToString());
                BuildLogFiles.RemoveAt(0);
            }
        }


        private void DeleteFile(string fileName)
        {
            System.IO.File.Delete(fileName);
        }

        private void KeepMaximumXHistoryDataEntries(IIntegrationResult result, Int32 entriesToKeep)
        {
            string HistoryXmlData = Publishers.ModificationHistoryPublisher.LoadHistory(result.ArtifactDirectory);

            if (HistoryXmlData.Length == 0)
            {
                return;
            }
            System.Xml.XmlDocument XmlDoc = new System.Xml.XmlDocument();

            XmlDoc.LoadXml(HistoryXmlData);

            if (XmlDoc.FirstChild.ChildNodes.Count == 0)
            {
                return;
            }


            Int32 CurrentAmountOfNodes = XmlDoc.FirstChild.ChildNodes.Count;
            System.IO.StringWriter CleanedHistoryData = new System.IO.StringWriter();

            for (int i = CurrentAmountOfNodes - entriesToKeep; i <= CurrentAmountOfNodes - 1; i++)
            {
                CleanedHistoryData.WriteLine(XmlDoc.FirstChild.ChildNodes[i].OuterXml);
            }


            System.IO.StreamWriter HistoryWriter = new System.IO.StreamWriter(
                    System.IO.Path.Combine(result.ArtifactDirectory,
                                            Publishers.ModificationHistoryPublisher.DataHistoryFileName));

            HistoryWriter.WriteLine(CleanedHistoryData.ToString());
            HistoryWriter.Close();

        }
    }
}
