using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol;

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
            if (System.IO.Directory.Exists(readerTask.OutputPath))
            {
                System.IO.Directory.Delete(readerTask.OutputPath, true);
            }
        }


        [Test]
        public void ShouldReadModificationFile()
        {
            // integration with modifications
            result = CreateSuccessfulWithModifications(DateTime.Now);
            writerTask.Run(result);


            // new integration without modifications, 
            // modifications to be read from the saved file
            result = CreateSuccessful(DateTime.Now.AddHours(1));
            Assert.AreEqual(0, result.Modifications.Length);

            // read the saved modifications into the current integration result
            readerTask.Run(result);

            Assert.AreEqual(2, result.Modifications.Length);

        }



        [Test]
        public void ShouldReadMultipleModificationFile()
        {

            // 1st integration with modifications
            result = CreateSuccessfulWithModifications(DateTime.Now);
            writerTask.Run(result);


            // 2nd integration with modifications
            result = CreateSuccessfulWithModifications(DateTime.Now.AddHours(1));
            writerTask.Run(result);


            // new integrationresult that should get the saved mods
            result = CreateSuccessful(DateTime.Now.AddHours(2));

            Assert.AreEqual(0, result.Modifications.Length);

            readerTask.Run(result);

            Assert.AreEqual(4, result.Modifications.Length);
        }


        [Test]
        public void ShouldAddReadModificationsToExistingOnes()
        {
            // integration with modifications
            result = CreateSuccessfulWithModifications(DateTime.Now);
            writerTask.Run(result);


            // new integration with modifications, 
            // modifications toread from the saved file should be added
            result = CreateSuccessfulWithModifications(DateTime.Now.AddHours(1));
            Assert.AreEqual(2, result.Modifications.Length);

            // read the saved modifications into the current integration result
            readerTask.Run(result);

            Assert.AreEqual(4, result.Modifications.Length);

        }



        private void ClearExistingModificationFiles()
        {
            result = CreateSuccessful(DateTime.Now);
            writerTask.OutputPath = result.BaseFromArtifactsDirectory("ReaderTest");
            readerTask.OutputPath = writerTask.OutputPath;

            if (System.IO.Directory.Exists(readerTask.OutputPath))
            {
                System.IO.Directory.Delete(readerTask.OutputPath, true);
            }
            System.IO.Directory.CreateDirectory(readerTask.OutputPath);

            //System.IO.FileInfo fi = new System.IO.FileInfo(System.IO.Path.Combine(result.BaseFromArtifactsDirectory(writerTask.OutputPath), writerTask.Filename));
            //string FileSearchPattern = fi.Name.Remove(fi.Name.Length - fi.Extension.Length) + "*" + fi.Extension;

            //foreach (string file in System.IO.Directory.GetFiles(fi.DirectoryName, FileSearchPattern))
            //{
            //    System.IO.File.Delete(file);
            //}

        }


        private IntegrationResult CreateSuccessful(DateTime integrationTime)
        {
            return IntegrationResultMother.CreateSuccessful(integrationTime);
        }

        private IntegrationResult CreateSuccessfulWithModifications(DateTime integrationTime)
        {
            IntegrationResult temp = IntegrationResultMother.CreateSuccessful(integrationTime);
            temp.Modifications = modifications;

            return temp;
        }

    }
}
