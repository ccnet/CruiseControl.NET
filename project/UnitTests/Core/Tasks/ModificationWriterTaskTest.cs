using Exortech.NetReflector;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Tasks;
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
        public void ShouldWriteOutModificationsToFileAsXmlWithBuildTimeAppended()
        {

            IntegrationResult result = IntegrationResultMother.CreateSuccessful();
            string newFileName = string.Format("artifactDir\\modifications_{0}.xml",result.StartTime.ToString("yyyyMMddHHmmssfff"));

            mockIO.Expect("Save", newFileName , new IsValidXml().And(new HasChildElements(2)));

            result.ArtifactDirectory = "artifactDir";
            result.Modifications = new Modification[]
				{
					ModificationMother.CreateModification("foo.txt", @"c\src"),
					ModificationMother.CreateModification("bar.txt", @"c\src")
				};
            task.AppendTimeStamp = true;
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
        public void ShouldSaveEmptyFileIfNoModificationsSpecifiedWithBuildTimeAppended()
        {

            IntegrationResult result = IntegrationResultMother.CreateSuccessful();
            string newFileName = string.Format("artifactDir\\output_{0}.xml", result.StartTime.ToString("yyyyMMddHHmmssfff"));
            mockIO.Expect("Save", newFileName, new IsValidXml().And(new HasChildElements(0)));

            result.ArtifactDirectory = "artifactDir";
            task.Filename = "output.xml";
            task.AppendTimeStamp = true;


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
        public void ShouldRebaseDirectoryRelativeToArtifactDirWithBuildTimeAppended()
        {
            IntegrationResult result = IntegrationResultMother.CreateSuccessful();
            string newFileName = string.Format("artifactDir\\relativePath\\modifications_{0}.xml", result.StartTime.ToString("yyyyMMddHHmmssfff"));

            mockIO.Expect("Save", newFileName, new IsValidXml().And(new HasChildElements(0)));

            result.ArtifactDirectory = "artifactDir";
            task.OutputPath = "relativePath";
            task.AppendTimeStamp = true;
            
            task.Run(result);
        }


		[Test]
		public void ShouldWriteXmlUsingUTF8Encoding()
		{
			mockIO.Expect("Save", @"artifactDir\modifications.xml", new StartsWith("<?xml version=\"1.0\" encoding=\"utf-8\"?>"));

			IntegrationResult result = IntegrationResultMother.CreateSuccessful();
			result.ArtifactDirectory = "artifactDir";
			task.Run(result);			
		}



        [Test]
        public void ShouldWriteXmlUsingUTF8EncodingWithBuildTimeAppended()
        {
            IntegrationResult result = IntegrationResultMother.CreateSuccessful();
            string newFileName = string.Format("artifactDir\\modifications_{0}.xml", result.StartTime.ToString("yyyyMMddHHmmssfff"));

            mockIO.Expect("Save", newFileName, new StartsWith("<?xml version=\"1.0\" encoding=\"utf-8\"?>"));

            result.ArtifactDirectory = "artifactDir";
            task.AppendTimeStamp = true;


            task.Run(result);
        }



		[Test]
		public void LoadFromConfigurationXml()
		{
			ModificationWriterTask writer = (ModificationWriterTask) NetReflector.Read(@"<modificationWriter filename=""foo.xml"" path=""c:\bar"" />");
			Assert.AreEqual("foo.xml", writer.Filename);
			Assert.AreEqual(@"c:\bar", writer.OutputPath);
            Assert.AreEqual(false, writer.AppendTimeStamp);

		}


        [Test]
        public void LoadFromConfigurationXmlWithBuildTimeSetToTrue()
        {
            ModificationWriterTask writer = (ModificationWriterTask)NetReflector.Read(@"<modificationWriter filename=""foo.xml"" path=""c:\bar"" appendTimeStamp=""true""/>");
            Assert.AreEqual("foo.xml", writer.Filename);
            Assert.AreEqual(@"c:\bar", writer.OutputPath);
            Assert.AreEqual(true, writer.AppendTimeStamp);

        }


		[Test]
		public void LoadFromMinimalConfigurationXml()
		{
			NetReflector.Read(@"<modificationWriter />");
		}




      


	}
}