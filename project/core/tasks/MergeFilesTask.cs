using System;
using System.Collections;
using ThoughtWorks.CruiseControl.Core.Publishers;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    [ReflectorType("merge")]
    public class MergeFilesTask : ITask
    {
        public void Run(IntegrationResult result)
        {
            foreach (string mergeFile in MergeFiles)
            {
                WildCardPath path = new WildCardPath(mergeFile);
                FileInfo[] files = path.GetFiles();
                foreach (FileInfo fileInfo in files)
                {
                    Log.Info("Merging file: " + fileInfo);
                    if (fileInfo.Exists)
                    {
                        result.TaskResults.Add((new DefaultTaskResult(fileInfo)));
                    }
                    else
                    {
                        Log.Warning("File not Found: " + fileInfo);
                    }
                }
            }
        }

        public bool ShouldRun(IntegrationResult result)
        {
            return true;
        }

        [ReflectorArray("files")] 
		public string[] MergeFiles = new string[0];
    }
}