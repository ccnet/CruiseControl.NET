using System;
using System.IO;
using NUnit.Framework;
using tw.ccnet.core;
using tw.ccnet.core.publishers;
using tw.ccnet.remote;
using tw.ccnet.core.util;

namespace tw.ccnet.core.publishers.test
{
	[TestFixture]
	public class BuildPublisherTest : CustomAssertion
	{
		private string pubDir = "BuildPublisherTest.PubDir";
		private string srcDir = "BuildPublisherTest.SrcDir";
		private string subDir = "SubDir";
		private string subSubDir = "SubSubDir";
		private string additionalDir = "BuildPublisherTest.AdditionalDir";
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
			publisher.AdditionalDir = additionalDir;
			IntegrationResult result = new IntegrationResult();
			result.Status = IntegrationStatus.Success;
			result.Label = "99";

			publisher.PublishIntegrationResults(null, result);

			FileInfo resultFile = new FileInfo(pubDir + @"\99\" + fileName);
			Assert("File not found in build number directory", resultFile.Exists);

			

			resultFile = new FileInfo(pubDir + @"\99\" + subDir + "\\" + fileName);
			Assert("File not found in sub directory", resultFile.Exists);

			resultFile = new FileInfo(pubDir + @"\99\" + subDir + "\\" + subSubDir + "\\" + fileName);
			Assert("File not found in sub sub directory", resultFile.Exists);


			resultFile = new FileInfo(pubDir + @"\" + additionalDir + @"\" + fileName);
			Assert("File not found in additional directory", resultFile.Exists);

			resultFile = new FileInfo(pubDir + @"\" + additionalDir + @"\" + subDir + "\\" + fileName);
			Assert("File not found in additional sub directory", resultFile.Exists);

			resultFile = new FileInfo(pubDir + @"\" + additionalDir + @"\" + subDir + "\\" + subSubDir + "\\" + fileName);
			Assert("File not found in additional sub sub directory", resultFile.Exists);

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
