using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
	[TestFixture]
	public class BuildPublisherTest : CustomAssertion
	{
		private string pubDir = "BuildPublisherTest.PubDir";
		private string srcDir = "BuildPublisherTest.SrcDir";
		private string subDir = "SubDir";
		private string subSubDir = "SubSubDir";
		private const string fileName = "foo.txt";
		private const string fileContents = "I'm the contents of foo.txt";

		[SetUp]
		public void SetUp() 
		{
			DirectoryInfo src = new DirectoryInfo(srcDir);
			if (!src.Exists)
				src.Create();

			DirectoryInfo sub = new DirectoryInfo(string.Format("{0}\\{1}", srcDir, subDir));
			if (!sub.Exists)
				sub.Create();
			DirectoryInfo subSub = new DirectoryInfo(string.Format("{0}\\{1}", sub.FullName, subSubDir));
			if (!subSub.Exists)
				subSub.Create();

			FileInfo srcFile = new FileInfo(srcDir + @"\" + fileName);
			StreamWriter writer = srcFile.CreateText();
			writer.WriteLine(fileContents);
			writer.Close();

			
			FileInfo subFile = new FileInfo(sub.FullName + @"\" + fileName);
			writer = subFile.CreateText();
			writer.WriteLine(fileContents);
			writer.Close();

			FileInfo subSubFile = new FileInfo(subSub.FullName + @"\" + fileName);
			writer = subSubFile.CreateText();
			writer.WriteLine(fileContents);
			writer.Close();
		}

		[Test]
		public void TestCopyFiles() 
		{
			BuildPublisher publisher = new BuildPublisher();
			publisher.PublishDir = pubDir;
			publisher.SourceDir = srcDir;
			IntegrationResult result = new IntegrationResult();
			result.Status = IntegrationStatus.Success;
			result.Label = "99";

			publisher.PublishIntegrationResults(result);

			FileInfo resultFile = new FileInfo(pubDir + @"\99\" + fileName);
			Assert.IsTrue(resultFile.Exists, "File not found in build number directory");

			resultFile = new FileInfo(pubDir + @"\99\" + subDir + "\\" + fileName);
			Assert.IsTrue(resultFile.Exists, "File not found in sub directory");

			resultFile = new FileInfo(pubDir + @"\99\" + subDir + "\\" + subSubDir + "\\" + fileName);
			Assert.IsTrue(resultFile.Exists, "File not found in sub sub directory");
		}

		[TearDown]
		public void TearDown() 
		{
			DirectoryInfo pub = new DirectoryInfo(pubDir);
			if (pub.Exists)
				pub.Delete(true);

			DirectoryInfo src = new DirectoryInfo(srcDir);
			if (src.Exists)
				src.Delete(true);
		}
	}
}
