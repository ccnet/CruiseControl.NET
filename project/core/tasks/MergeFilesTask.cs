using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	[ReflectorType("merge")]
	public class MergeFilesTask
        : TaskBase, ITask
	{
		[ReflectorArray("files")]
		public string[] MergeFiles = new string[0];

        /// <summary>
        /// Description used for the visualisation of the buildstage, if left empty the process name will be shown
        /// </summary>
        [ReflectorProperty("description", Required = false)]
        public string Description = string.Empty;


		public void Run(IIntegrationResult result)
		{
            result.BuildProgressInformation.SignalStartRunTask(Description != string.Empty ? Description : "Merging Files");                

			foreach (string mergeFile in MergeFiles)
			{
				string fullMergeFile = mergeFile;
				if (!Path.IsPathRooted(mergeFile))
					fullMergeFile = Path.Combine(result.WorkingDirectory, mergeFile);

				WildCardPath path = new WildCardPath(fullMergeFile);
				foreach (FileInfo fileInfo in path.GetFiles())
				{
					Log.Info("Merging file: " + fileInfo);
					if (fileInfo.Exists)
					{
						result.AddTaskResult((new FileTaskResult(fileInfo)));
					}
					else
					{
						Log.Warning("File not Found: " + fileInfo);
					}
				}
			}
		}
	}
}
