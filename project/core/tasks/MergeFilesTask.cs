using System;
using System.Collections;
using ThoughtWorks.CruiseControl.Core.Publishers;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	[ReflectorType("mergefiles")]
    public class MergeFilesTask : ITask
    {
        private IList _mergeFiles = new ArrayList();

        public void Run(IntegrationResult result)
        {
			foreach (string mergeFile in _mergeFiles)
			{
				WildCardPath path=new WildCardPath(mergeFile);
				FileInfo[] files = path.GetFiles();
				foreach (FileInfo fileInfo in files)
				{
					result.TaskResults.Add((new DefaultTaskResult(fileInfo)));
				}
			}
        }

        public bool ShouldRun(IntegrationResult result)
        {
            return true;
        }

		[ReflectorCollection("files", InstanceType=typeof(ArrayList), Required = true)] 
        public IList MergeFiles
        {
            get { return _mergeFiles; }
			set { _mergeFiles = value; }
        }
    }
}