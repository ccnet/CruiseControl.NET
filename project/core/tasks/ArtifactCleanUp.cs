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
            DeleteBuildsOlderThanXDays
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
            string BuildLogFolder = result.BuildLogDirectory;
            
            switch (cleanUpMethod)
            {
                case CleanUpMethod.KeepLastXBuilds:
                    KeepLastXBuilds(BuildLogFolder, CleaningUpValue);
                    break;
                    
                case CleanUpMethod.DeleteBuildsOlderThanXDays:
                    DeleteBuildsOlderThanXDays(BuildLogFolder, CleaningUpValue);
                    break;

                default:
                    throw new NotImplementedException("Unmapped cleaning up method used");                    
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

    }
}
