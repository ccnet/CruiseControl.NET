using System;
using System.IO;
using System.Xml;
using Exortech.NetReflector;

namespace tw.ccnet.core.publishers
{
	[ReflectorType("buildpublisher")]
	public class BuildPublisher : PublisherBase
	{
		private string publishDir;
		private string sourceDir;
		private string additionalDir;

		public BuildPublisher() : base()
		{
		}

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

		[ReflectorProperty("additionalDir", Required=false)]
		public string AdditionalDir
		{
			get { return additionalDir; }
			set { additionalDir = value; }
		}

		public override void Publish(object source, IntegrationResult result)
		{
			if (result.Succeeded) 
			{
				DirectoryInfo pubDir = new DirectoryInfo(PublishDir);
				DirectoryInfo srcDir = new DirectoryInfo(SourceDir);
				DirectoryInfo destination = pubDir.CreateSubdirectory(result.Label);
				DirectoryInfo additionalDestination = null;

				recurseSubDirectories(srcDir, destination);

				if (AdditionalDir != null) 
				{
					additionalDestination = pubDir.CreateSubdirectory(AdditionalDir);
					recurseSubDirectories(srcDir, additionalDestination);
				}
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
