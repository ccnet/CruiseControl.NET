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
    public class ModificationReaderTaskTest
    {
        private ModificationWriterTask writerTask;
        private ModificationReaderTask readerTask;
        private IntegrationResult result;
        private Modification[] modifications;

        [SetUp]
        public void SetUp()
        {
            writerTask = new ModificationWriterTask();
            readerTask = new ModificationReaderTask();
            writerTask.AppendTimeStamp = true;

            ClearExistingModificationFiles();

            modifications = new Modification[]
				{
					ModificationMother.CreateModification("foo.txt", @"c\src"),
					ModificationMother.CreateModification("bar.txt", @"c\src")
				};                                   
        }


        [TearDown]
        public void TearDown()
        {
        }


        [Test]
        public void ShouldReadModificationFile()
        {
            // integration with modifications
            result = IntegrationResultMother.CreateSuccessful(modifications);
            writerTask.Run(result);
            

            // new integration without modifications, 
            // modifications to be read from the saved file
            result = IntegrationResultMother.CreateSuccessful();            
            Assert.AreEqual(0, result.Modifications.Length);

            // read the saved modifications into the current integration result
            readerTask.Run(result);

            Assert.AreEqual(2, result.Modifications.Length);

        }



        [Test]
        public void ShouldReadMultipleModificationFile()
        {

            // 1st integration with modifications
            result = IntegrationResultMother.CreateSuccessful(modifications);
            writerTask.Run(result);

            System.Threading.Thread.Sleep(3);

            // 2nd integration with modifications
            result = IntegrationResultMother.CreateSuccessful(modifications);
            writerTask.Run(result);

            
            // new integrationresult that should get the saved mods
            result = IntegrationResultMother.CreateSuccessful();

            Assert.AreEqual(0, result.Modifications.Length);

            readerTask.Run(result);

            Assert.AreEqual(4, result.Modifications.Length);
        }



        private void ClearExistingModificationFiles()
        {
            result = IntegrationResultMother.CreateSuccessful();

            System.IO.FileInfo fi = new System.IO.FileInfo(System.IO.Path.Combine(result.BaseFromArtifactsDirectory(writerTask.OutputPath), writerTask.Filename));
            string FileSearchPattern = fi.Name.Remove(fi.Name.Length - fi.Extension.Length) + "*" + fi.Extension;

            foreach (string file in System.IO.Directory.GetFiles(fi.DirectoryName, FileSearchPattern))
            {
                System.IO.File.Delete(file);
            }

        }
    }
}
