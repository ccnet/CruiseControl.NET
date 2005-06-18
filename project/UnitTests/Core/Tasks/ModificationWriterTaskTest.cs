using Exortech.NetReflector;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.tasks;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.UnitTests.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
	[TestFixture]
	public class ModificationWriterTaskTest
	{
		private IMock mockIO;
		private ModificationWriterTask task;

		[SetUp]
		public void SetUp()
		{
			mockIO = new DynamicMock(typeof (IFileSystem));
			task = new ModificationWriterTask(mockIO.MockInstance as IFileSystem);
		}

		[TearDown]
		public void TearDown()
		{
			mockIO.Verify();
		}

		[Test]
		public void ShouldWriteOutModificationsToFileAsXml()
		{
			mockIO.Expect("Save", @"artifactDir\modifications.xml", new IsValidXml().And(new HasChildElements(2)));

			IntegrationResult result = IntegrationResultMother.CreateSuccessful();
			result.ArtifactDirectory = "artifactDir";
			result.Modifications = new Modification[]
				{
					ModificationMother.CreateModification("foo.txt", @"c\src"),
					ModificationMother.CreateModification("bar.txt", @"c\src")
				};
			task.Run(result);
		}

		[Test]
		public void ShouldSaveEmptyFileIfNoModificationsSpecified()
		{
			mockIO.Expect("Save", @"artifactDir\output.xml", new IsValidXml().And(new HasChildElements(0)));

			IntegrationResult result = IntegrationResultMother.CreateSuccessful();
			result.ArtifactDirectory = "artifactDir";
			task.Filename = "output.xml";
			task.Run(result);
		}

		[Test]
		public void ShouldRebaseDirectoryRelativeToArtifactDir()
		{
			mockIO.Expect("Save", @"artifactDir\relativePath\modifications.xml", new IsValidXml().And(new HasChildElements(0)));

			IntegrationResult result = IntegrationResultMother.CreateSuccessful();
			result.ArtifactDirectory = "artifactDir";
			task.OutputPath = "relativePath";
			task.Run(result);
		}

		[Test]
		public void LoadFromConfigurationXml()
		{
			ModificationWriterTask writer = (ModificationWriterTask) NetReflector.Read(@"<modificationWriter filename=""foo.xml"" path=""c:\bar"" />");
			Assert.AreEqual("foo.xml", writer.Filename);
			Assert.AreEqual(@"c:\bar", writer.OutputPath);
		}

		[Test]
		public void LoadFromMinimalConfigurationXml()
		{
			NetReflector.Read(@"<modificationWriter />");
		}
	}
}