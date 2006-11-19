using System.IO;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	[ReflectorType("buildpublisher")]
	public class BuildPublisher : ITask
	{
		[ReflectorProperty("publishDir")]
		public string PublishDir;

		[ReflectorProperty("sourceDir")]
		public string SourceDir;

		[ReflectorProperty("useLabelSubDirectory")]
		public bool UseLabelSubDirectory = true;
		
		public void Run(IIntegrationResult result)
		{
			if (result.Succeeded)
			{
				DirectoryInfo srcDir = new DirectoryInfo(result.BaseFromWorkingDirectory(SourceDir));
				DirectoryInfo pubDir = new DirectoryInfo(result.BaseFromArtifactsDirectory(PublishDir));
				if (! pubDir.Exists) pubDir.Create();
				
				if (UseLabelSubDirectory)
					pubDir = pubDir.CreateSubdirectory(result.Label);

				RecurseSubDirectories(srcDir, pubDir);
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
	}
}