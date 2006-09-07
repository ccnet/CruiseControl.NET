using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
	[TestFixture]
	public class BuildPublisherTest : CustomAssertion
	{
		private SystemPath srcRoot;
		private SystemPath pubRoot;
		private SystemPath subRoot;
		private SystemPath subSubRoot;
		private const string fileName = "foo.txt";
		private const string fileContents = "I'm the contents of foo.txt";

		[SetUp]
		public void SetUp()
		{
			srcRoot = SystemPath.UniqueTempPath();
			pubRoot = SystemPath.UniqueTempPath();
			subRoot = srcRoot.CreateSubDirectory("SubDir");
			subSubRoot = subRoot.CreateSubDirectory("SubSubDir");
			srcRoot.CreateTextFile(fileName, fileContents);
			subRoot.CreateTextFile(fileName, fileContents);
			subSubRoot.CreateTextFile(fileName, fileContents);
		}

		[Test]
		public void TestCopyFiles()
		{
			BuildPublisher publisher = new BuildPublisher();
			publisher.PublishDir = pubRoot.ToString();
			publisher.SourceDir = srcRoot.ToString();
			IntegrationResult result = new IntegrationResult();
			result.Status = IntegrationStatus.Success;
			result.Label = "99";

			publisher.Run(result);

			SystemPath labelPubDir = pubRoot.Combine("99");
			Assert.IsTrue(labelPubDir.Combine(fileName).Exists(), "File not found in build number directory");
			SystemPath subPubDir = labelPubDir.Combine("SubDir");
			Assert.IsTrue(subPubDir.Combine(fileName).Exists(), "File not found in sub directory");
			Assert.IsTrue(subPubDir.Combine("SubSubDir").Combine(fileName).Exists(), "File not found in sub sub directory");
		}

		[TearDown]
		public void TearDown()
		{
			srcRoot.DeleteDirectory();
			pubRoot.DeleteDirectory();
		}
	}
}