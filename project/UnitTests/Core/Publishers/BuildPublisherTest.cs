using System.IO;
using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
	[TestFixture]
	public class BuildPublisherTest : CustomAssertion
	{
		private SystemPath srcRoot;
		private SystemPath pubRoot;
		private BuildPublisher publisher;
		private IntegrationResult result;
		private SystemPath labelPubDir;
		private const string fileName = "foo.txt";
		private const string fileContents = "I'm the contents of foo.txt";

		[SetUp]
		public void SetUp()
		{
			srcRoot = SystemPath.UniqueTempPath();
			pubRoot = SystemPath.UniqueTempPath();

			publisher = new BuildPublisher();
			publisher.PublishDir = pubRoot.ToString();
			publisher.SourceDir = srcRoot.ToString();
			result = IntegrationResultMother.CreateSuccessful("99");
			labelPubDir = pubRoot.Combine("99");
		}

		[Test]
		public void CopyFiles()
		{
			SystemPath subRoot = srcRoot.CreateSubDirectory("SubDir");
			SystemPath subSubRoot = subRoot.CreateSubDirectory("SubSubDir");
			srcRoot.CreateTextFile(fileName, fileContents);
			subRoot.CreateTextFile(fileName, fileContents);
			subSubRoot.CreateTextFile(fileName, fileContents);

			publisher.Run(result);

			Assert.IsTrue(labelPubDir.Combine(fileName).Exists(), "File not found in build number directory");
			SystemPath subPubDir = labelPubDir.Combine("SubDir");
			Assert.IsTrue(subPubDir.Combine(fileName).Exists(), "File not found in sub directory");
			Assert.IsTrue(subPubDir.Combine("SubSubDir").Combine(fileName).Exists(), "File not found in sub sub directory");
		}

		[Test]
		public void SourceRootShouldBeRelativeToIntegrationWorkingDirectory()
		{
			srcRoot.CreateSubDirectory("foo").CreateTextFile(fileName, fileContents);

			result.WorkingDirectory = srcRoot.ToString();

			publisher.SourceDir = "foo";
			publisher.Run(result);

			Assert.IsTrue(labelPubDir.Combine(fileName).Exists(), "File not found in build number directory");
		}

		[Test]
		public void PublishDirShouldBeRelativeToIntegrationArtifactDirectory()
		{
			srcRoot.CreateSubDirectory("foo").CreateTextFile(fileName, fileContents);
			result.ArtifactDirectory = pubRoot.ToString();
			
			publisher.PublishDir = "bar";
			publisher.Run(result);

			labelPubDir = pubRoot.Combine("bar").Combine("99").Combine("foo");
			Assert.IsTrue(labelPubDir.Combine(fileName).Exists(), "File not found in build number directory");
		}

		[Test]
		public void DoNotUseLabelSubdirectoryAndCreatePublishDirIfItDoesntExist()
		{
			srcRoot.CreateDirectory().CreateTextFile(fileName, fileContents);
			publisher.UseLabelSubDirectory = false;
			publisher.Run(result);

			Assert.IsTrue(pubRoot.Combine(fileName).Exists(), "File not found in pubRoot directory");
		}

		[Test]
		public void OverwriteReadOnlyFileAtDestination()
		{
			srcRoot.CreateDirectory().CreateTextFile(fileName, fileContents);
			pubRoot.CreateDirectory();
			FileInfo readOnlyDestFile = new FileInfo(pubRoot.CreateEmptyFile(fileName).ToString());
			readOnlyDestFile.Attributes = FileAttributes.ReadOnly;
			publisher.UseLabelSubDirectory = false;
			publisher.Run(result);			
		}

		[Test]
		public void LoadFromXml()
		{
			string xml = @"<buildpublisher useLabelSubDirectory=""false"">
	<sourceDir>c:\source</sourceDir>
	<publishDir>\\file\share\build</publishDir>
</buildpublisher>";

			publisher = (BuildPublisher) NetReflector.Read(xml);
			Assert.AreEqual(@"c:\source", publisher.SourceDir);
			Assert.AreEqual(@"\\file\share\build", publisher.PublishDir);
			Assert.AreEqual(false, publisher.UseLabelSubDirectory);
		}

		[Test]
		public void LoadMinimalXml()
		{
			string xml = @"<buildpublisher />";

			publisher = (BuildPublisher) NetReflector.Read(xml);
			Assert.IsNull(publisher.SourceDir);
			Assert.IsNull(publisher.PublishDir);
			Assert.IsTrue(publisher.UseLabelSubDirectory);			
		}
		
		[Test]
		public void PublishWorksIfNoPropertiesAreSpecified()
		{
			srcRoot.CreateDirectory().CreateTextFile(fileName, fileContents);
			result.WorkingDirectory = srcRoot.ToString();
			result.ArtifactDirectory = pubRoot.ToString();
			
			publisher = new BuildPublisher();
			publisher.Run(result);

			Assert.IsTrue(labelPubDir.Combine(fileName).Exists(), "File not found in pubRoot directory");
		}
		
		[TearDown]
		public void TearDown()
		{
			srcRoot.DeleteDirectory();
			pubRoot.DeleteDirectory();
		}
	}
}