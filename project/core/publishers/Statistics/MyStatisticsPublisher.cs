using System.IO;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
	/// <summary>
	/// Generates an xml file containing build statistics which 
	/// can be used to display graphs and charts on the project page.
	/// </summary>
	[ReflectorType("buildstatistics")]
	public class MyStatisticsPublisher : ITask
	{
		private string fileName;
		private BuildStatisticsProcessor processor = new BuildStatisticsProcessor();

		public BuildStatisticsProcessor Processor
		{
			get { return processor; }
			set { processor = value; }
		}

		public void Run(IIntegrationResult result)
		{
			IntegrationStatistics results = processor.ProcessBuildResults(result);

			Publish(results, GetFilePath(result));
		}

		private void Publish(IntegrationStatistics integrationStatistics, string filePath)
		{
			FileInfo fileInfo = new FileInfo(filePath);
			FileStream fileStream = fileInfo.Open(FileMode.OpenOrCreate);
			fileStream.Close();
		}

		[ReflectorProperty("file")]
		public string FileName
		{
			get { return fileName; }
			set { fileName = value;}
		}

		private string GetFilePath(IIntegrationResult result)
		{
			return Path.Combine(result.ArtifactDirectory, FileName);
		}
	}

}
