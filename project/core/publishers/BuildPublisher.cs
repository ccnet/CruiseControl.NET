using System.IO;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	[ReflectorType("buildpublisher")]
	public class BuildPublisher : ITask
	{
		private string publishDir;
		private string sourceDir;

		[ReflectorProperty("publishDir")]
		public string PublishDir
		{
			get { return publishDir; }
			set { publishDir = value; }
		}

		[ReflectorProperty("sourceDir")]
		public string SourceDir
		{
			get { return sourceDir; }
			set { sourceDir = value; }
		}

		public void Run(IIntegrationResult result)
		{
			if (result.Succeeded) 
			{
				DirectoryInfo pubDir = new DirectoryInfo(PublishDir);
				DirectoryInfo srcDir = new DirectoryInfo(SourceDir);
				DirectoryInfo destination = pubDir.CreateSubdirectory(result.Label);

				recurseSubDirectories(srcDir, destination);
			}
		}

		private void recurseSubDirectories(DirectoryInfo srcDir, DirectoryInfo destination) 
		{
			FileInfo[] files = srcDir.GetFiles();
			foreach (FileInfo file in files) 
			{
				file.CopyTo(destination.FullName + @"\" + file.Name, true);

			}
			DirectoryInfo[] subDirectories = srcDir.GetDirectories();
			foreach (DirectoryInfo subDir in subDirectories) 
			{
				DirectoryInfo subDestination = destination.CreateSubdirectory(subDir.Name);

				recurseSubDirectories(subDir, subDestination);
			}
		}
	}
}
