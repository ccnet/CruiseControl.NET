using System.IO;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	[ReflectorType("buildpublisher")]
	public class BuildPublisher : ITask
	{
		[ReflectorProperty("publishDir", Required=false)]
		public string PublishDir;

		[ReflectorProperty("sourceDir", Required=false)]
		public string SourceDir;

		[ReflectorProperty("useLabelSubDirectory", Required=false)]
		public bool UseLabelSubDirectory = true;

        [ReflectorProperty("alwaysPublish", Required = false)]
        public bool AlwaysPublish = false;


		public void Run(IIntegrationResult result)
		{
            if (result.Succeeded || AlwaysPublish)
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