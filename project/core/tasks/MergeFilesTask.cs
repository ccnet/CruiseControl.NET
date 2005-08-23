using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	[ReflectorType("merge")]
	public class MergeFilesTask : ITask
	{
		[ReflectorArray("files")]
		public string[] MergeFiles = new string[0];

		public void Run(IIntegrationResult result)
		{
			foreach (string mergeFile in MergeFiles)
			{
				string fullMergeFile = mergeFile;
				if (!Path.IsPathRooted(mergeFile))
				{
					fullMergeFile = Path.Combine(result.WorkingDirectory, mergeFile);
				}
				WildCardPath path = new WildCardPath(fullMergeFile);
				FileInfo[] files = path.GetFiles();
				foreach (FileInfo fileInfo in files)
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