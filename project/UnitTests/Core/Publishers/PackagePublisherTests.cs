namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using Exortech.NetReflector;
    using NMock;
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Core.Publishers;
    using ThoughtWorks.CruiseControl.Remote;

    [TestFixture]
    public class PackagePublisherTests
    {
        #region Private fields
        private string dataFilePath;
        #endregion

        #region Setup
        [OneTimeSetUp]
        public void Setup()
        {
            // Delete any outputs from the last build
            string projectPackageList = Path.Combine(Path.GetTempPath(), "Test project-packages.xml");
            string buildFolder = Path.Combine(Path.GetTempPath(), "A Label");
            if (File.Exists(projectPackageList)) File.Delete(projectPackageList);
            if (Directory.Exists(buildFolder)) Directory.Delete(buildFolder, true);
            dataFilePath = Path.Combine(Path.GetTempPath(), "datafile.txt");
            if (File.Exists(dataFilePath)) File.Delete(dataFilePath);
            File.WriteAllText(dataFilePath, "This is a test file for the packaging publisher");
        }
        #endregion

        #region CleanUp
        [OneTimeTearDown]
        public void CleanUp()
        {
            if (File.Exists(dataFilePath)) File.Delete(dataFilePath);
        }
        #endregion

        #region Test methods
        #region CheckAllProperties()
        /// <summary>
        /// Make sure all the properties return the same value that they were set with.
        /// </summary>
        [Test]
        public void CheckAllProperties()
        {
            PackagePublisher publisher = new PackagePublisher();
            publisher.AlwaysPackage = true;
            Assert.AreEqual(true, publisher.AlwaysPackage);
            publisher.CompressionLevel = 9;
            Assert.AreEqual(9, publisher.CompressionLevel);
            //publisher.Flatten = true;
            //Assert.AreEqual(true, publisher.Flatten);
            ManifestGenerator generator = new ManifestGenerator();
            publisher.ManifestGenerator = generator;
            Assert.AreSame(generator, publisher.ManifestGenerator);
            publisher.PackageName = "Package name";
            Assert.AreEqual("Package name", publisher.PackageName);
            publisher.PackageList = new IPackageItem[] {
                new PackageFile()
            };
            Assert.AreEqual(1, publisher.PackageList.Length);
        }
        #endregion

        #region CompressionLevelOnlyAllowedBetweenZeroAndNine()
        /// <summary>
        /// Make sure the compression level property is constrained
        /// </summary>
        [Test]
        public void CompressionLevelOnlyAllowedBetweenZeroAndNine()
        {
            PackagePublisher publisher = new PackagePublisher();
            bool exceptionThrown = false;
            try
            {
                publisher.CompressionLevel = -1;
            }
            catch (ArgumentOutOfRangeException)
            {
                exceptionThrown = true;
            }
            Assert.IsTrue(exceptionThrown, "CompressionLevel allowed a number less than zero");
            exceptionThrown = false;
            try
            {
                publisher.CompressionLevel = 10;
            }
            catch (ArgumentOutOfRangeException)
            {
                exceptionThrown = true;
            }
            Assert.IsTrue(exceptionThrown, "CompressionLevel allowed a number greater than nine");
            for (int loop = 0; loop <= 9; loop++)
            {
                publisher.CompressionLevel = loop;
            }
        }
        #endregion

        #region MinimalRun()
        /// <summary>
        /// Run the publisher with the minimum options.
        /// </summary>
        [Test]
        public void MinimalRun()
        {
            IntegrationRequest request = new IntegrationRequest(BuildCondition.ForceBuild, "Somewhere", null);
            IntegrationSummary summary = new IntegrationSummary(IntegrationStatus.Success, "A Label", "Another Label", new DateTime(2009, 1, 1));
            IntegrationResult result = new IntegrationResult("Test project", "Working directory", "Artifact directory", request, summary);
            Modification modification1 = GenerateModification("first file", "Add");
            Modification modification2 = GenerateModification("second file", "Modify");
            result.Modifications = new Modification[] { modification1, modification2 };
            result.Status = IntegrationStatus.Success;
            result.ArtifactDirectory = Path.GetTempPath();

            string packageLocation = Path.Combine(Path.GetTempPath(), "Test Package-1");
            string packageName = packageLocation + ".zip";
            if (File.Exists(packageName)) File.Delete(packageName);
            PackagePublisher publisher = new PackagePublisher();
            publisher.PackageName = packageLocation;
            publisher.PackageList = new IPackageItem[] { 
                new PackageFile(dataFilePath) 
            };
            publisher.Run(result);
            Assert.IsTrue(File.Exists(packageName), "Package not generated");
            Assert.IsTrue(
                File.Exists(Path.Combine(Path.GetTempPath(), Path.Combine("A Label", "Test project-packages.xml"))), 
                "Project package list not generated");
        }
        #endregion

        #region RunWithFlatten()
        /// <summary>
        /// Run the publisher with the minimum options.
        /// </summary>
        [Test]
        public void RunWithFlatten()
        {
            IntegrationRequest request = new IntegrationRequest(BuildCondition.ForceBuild, "Somewhere", null);
            IntegrationSummary summary = new IntegrationSummary(IntegrationStatus.Success, "A Label", "Another Label", new DateTime(2009, 1, 1));
            IntegrationResult result = new IntegrationResult("Test project", "Working directory", "Artifact directory", request, summary);
            Modification modification1 = GenerateModification("first file", "Add");
            Modification modification2 = GenerateModification("second file", "Modify");
            result.Modifications = new Modification[] { modification1, modification2 };
            result.Status = IntegrationStatus.Success;
            result.ArtifactDirectory = Path.GetTempPath();

            string packageLocation = Path.Combine(Path.GetTempPath(), "Test Package-1");
            string packageName = packageLocation + ".zip";
            if (File.Exists(packageName)) File.Delete(packageName);
            PackagePublisher publisher = new PackagePublisher();
            publisher.PackageName = packageLocation;
            //publisher.Flatten = true;
            publisher.PackageList = new IPackageItem[] { 
                new PackageFile(dataFilePath) 
            };
            publisher.Run(result);
            Assert.IsTrue(File.Exists(packageName), "Package not generated");
            Assert.IsTrue(
                File.Exists(Path.Combine(Path.GetTempPath(), Path.Combine("A Label", "Test project-packages.xml"))),
                "Project package list not generated");
        }
        #endregion

        #region RunForNonExistantFiles()
        /// <summary>
        /// Run the publisher for files that don't exist.
        /// </summary>
        [Test]
        public void RunForNonExistantFiles()
        {
            IntegrationRequest request = new IntegrationRequest(BuildCondition.ForceBuild, "Somewhere", null);
            IntegrationSummary summary = new IntegrationSummary(IntegrationStatus.Success, "A Label", "Another Label", new DateTime(2009, 1, 1));
            IntegrationResult result = new IntegrationResult("Test project", "Working directory", "Artifact directory", request, summary);
            Modification modification1 = GenerateModification("first file", "Add");
            Modification modification2 = GenerateModification("second file", "Modify");
            result.Modifications = new Modification[] { modification1, modification2 };
            result.Status = IntegrationStatus.Success;
            result.ArtifactDirectory = Path.GetTempPath();

            string packageLocation = Path.Combine(Path.GetTempPath(), "Test Package-1");
            string packageName = packageLocation + ".zip";
            if (File.Exists(packageName)) File.Delete(packageName);
            PackagePublisher publisher = new PackagePublisher();
            publisher.PackageName = packageLocation;
            publisher.PackageList = new IPackageItem[] { 
                new PackageFile(Path.GetTempFileName()) 
            };
            publisher.Run(result);
            Assert.IsTrue(File.Exists(packageName), "Package not generated");
            Assert.IsTrue(
                File.Exists(Path.Combine(Path.GetTempPath(), Path.Combine("A Label", "Test project-packages.xml"))),
                "Project package list not generated");
        }
        #endregion

        #region RunForWildCard()
        /// <summary>
        /// Run the publisher for files that don't exist.
        /// </summary>
        [Test]
        public void RunForWildCard()
        {
            IntegrationRequest request = new IntegrationRequest(BuildCondition.ForceBuild, "Somewhere", null);
            IntegrationSummary summary = new IntegrationSummary(IntegrationStatus.Success, "A Label", "Another Label", new DateTime(2009, 1, 1));
            IntegrationResult result = new IntegrationResult("Test project", "Working directory", "Artifact directory", request, summary);
            Modification modification1 = GenerateModification("first file", "Add");
            Modification modification2 = GenerateModification("second file", "Modify");
            result.Modifications = new Modification[] { modification1, modification2 };
            result.Status = IntegrationStatus.Success;
            result.ArtifactDirectory = Path.GetTempPath();

            string tempPath = Path.Combine(Path.GetTempPath(), "CCNetTest");
            if (Directory.Exists(tempPath)) Directory.Delete(tempPath, true);
            Directory.CreateDirectory(tempPath);
            File.WriteAllText(Path.Combine(tempPath, "test.txt"), "Some text data");
            File.WriteAllText(Path.Combine(tempPath, "test.tst"), "Some text data");
            File.WriteAllText(Path.Combine(tempPath, "test2.txt"), "Some text data");
            File.WriteAllText(Path.Combine(tempPath, "test2.tst"), "Some text data");

            string packageLocation = Path.Combine(Path.GetTempPath(), "Test Package-1");
            string packageName = packageLocation + ".zip";
            if (File.Exists(packageName)) File.Delete(packageName);
            PackagePublisher publisher = new PackagePublisher();
            publisher.PackageName = packageLocation;
            publisher.PackageList = new IPackageItem[] { 
                new PackageFile(Path.Combine(tempPath, "*.txt")) 
            };
            publisher.Run(result);
            Assert.IsTrue(File.Exists(packageName), "Package not generated");
            Assert.IsTrue(
                File.Exists(Path.Combine(Path.GetTempPath(), Path.Combine("A Label", "Test project-packages.xml"))),
                "Project package list not generated");
        }
        #endregion

        #region RunForDirectoryWildCard()
        /// <summary>
        /// Run the publisher for files that don't exist.
        /// </summary>
        [Test]
        public void RunForDirectoryWildCard()
        {
            IntegrationRequest request = new IntegrationRequest(BuildCondition.ForceBuild, "Somewhere", null);
            IntegrationSummary summary = new IntegrationSummary(IntegrationStatus.Success, "A Label", "Another Label", new DateTime(2009, 1, 1));
            IntegrationResult result = new IntegrationResult("Test project", "Working directory", "Artifact directory", request, summary);
            Modification modification1 = GenerateModification("first file", "Add");
            Modification modification2 = GenerateModification("second file", "Modify");
            result.Modifications = new Modification[] { modification1, modification2 };
            result.Status = IntegrationStatus.Success;
            result.ArtifactDirectory = Path.GetTempPath();

            string packageLocation = Path.Combine(Path.GetTempPath(), "Test Package-1");
            string packageName = packageLocation + ".zip";
            if (File.Exists(packageName)) File.Delete(packageName);
            PackagePublisher publisher = new PackagePublisher();
            publisher.PackageName = packageLocation;
            publisher.PackageList = new IPackageItem[] { 
                new PackageFile(Path.Combine(Path.GetTempPath(), "**\\datafile.txt"))
            };
            publisher.Run(result);
            Assert.IsTrue(File.Exists(packageName), "Package not generated");
            Assert.IsTrue(
                File.Exists(Path.Combine(Path.GetTempPath(), Path.Combine("A Label", "Test project-packages.xml"))),
                "Project package list not generated");
        }
        #endregion

        #region IncludeManifestInPackage()
        /// <summary>
        /// Run the publisher with a manifest generator.
        /// </summary>
        [Test]
        public void IncludeManifestInPackage()
        {
            IntegrationRequest request = new IntegrationRequest(BuildCondition.ForceBuild, "Somewhere", null);
            IntegrationSummary summary = new IntegrationSummary(IntegrationStatus.Success, "A Label", "Another Label", new DateTime(2009, 1, 1));
            IntegrationResult result = new IntegrationResult("Test project", "Working directory", "Artifact directory", request, summary);
            Modification modification1 = GenerateModification("first file", "Add");
            Modification modification2 = GenerateModification("second file", "Modify");
            result.Modifications = new Modification[] { modification1, modification2 };
            result.Status = IntegrationStatus.Success;
            result.ArtifactDirectory = Path.GetTempPath();

            XmlDocument manifest = new XmlDocument();
            manifest.AppendChild(manifest.CreateElement("manifest"));
            DynamicMock generatorMock = new DynamicMock(typeof(IManifestGenerator));
            List<string> files = new List<string>();
            files.Add(dataFilePath);
            generatorMock.ExpectAndReturn("Generate", manifest, result, files.ToArray());

            string packageLocation = Path.Combine(Path.GetTempPath(), "Test Package-1");
            string packageName = packageLocation + ".zip";
            if (File.Exists(packageName)) File.Delete(packageName);
            PackagePublisher publisher = new PackagePublisher();
            publisher.PackageName = packageLocation;
            publisher.ManifestGenerator = generatorMock.MockInstance as IManifestGenerator;
            publisher.PackageList = new IPackageItem[] { 
                new PackageFile(dataFilePath) 
            };
            publisher.Run(result);
            Assert.IsTrue(File.Exists(packageName), "Package not generated");
            Assert.IsTrue(
                File.Exists(Path.Combine(Path.GetTempPath(), Path.Combine("A Label", "Test project-packages.xml"))),
                "Project package list not generated");
            generatorMock.Verify();
        }
        #endregion

        [Test]
        public void Loads15Configuration()
        {
            // NOTE: The namespace is important - this identifies the version!
            var xml = "<package xmlns=\"http://thoughtworks.org/ccnet/1/5\">" +
                "<name>Test</name>" + 
                "<files>" +
                "<file>fileToLoad.1</file>" +
                "<file>fileToLoad.2</file>" + 
                "</files>" + 
                "</package>";
            var publisher = NetReflector.Read(xml);
            Assert.IsNotNull(publisher);
            Assert.IsInstanceOf<PackagePublisher>(publisher);
            var packagePublisher = publisher as PackagePublisher;
            Assert.AreEqual("Test", packagePublisher.PackageName);
            Assert.AreEqual(2, packagePublisher.PackageList.Length);
            this.AssertFilesAreTheSame(new PackageFile("fileToLoad.1"), packagePublisher.PackageList[0]);
            this.AssertFilesAreTheSame(new PackageFile("fileToLoad.2"), packagePublisher.PackageList[1]);
        }

        [Test]
        public void Loads16Configuration()
        {
            // NOTE: The namespace is important - this identifies the version!
            var xml = "<package xmlns=\"http://thoughtworks.org/ccnet/1/6\">" +
                "<name>Test</name>" +
                "<packageList>" +
                "<packageFile sourceFile=\"fileToLoad.1\" />" +
                "<packageFile sourceFile=\"fileToLoad.2\" />" +
                "</packageList>" +
                "</package>";
            var publisher = NetReflector.Read(xml);
            Assert.IsNotNull(publisher);
            Assert.IsInstanceOf<PackagePublisher>(publisher);
            var packagePublisher = publisher as PackagePublisher;
            Assert.AreEqual("Test", packagePublisher.PackageName);
            Assert.AreEqual(2, packagePublisher.PackageList.Length);
            this.AssertFilesAreTheSame(new PackageFile("fileToLoad.1"), packagePublisher.PackageList[0]);
            this.AssertFilesAreTheSame(new PackageFile("fileToLoad.2"), packagePublisher.PackageList[1]);
        }

        #endregion

        #region Private methods
        private void AssertFilesAreTheSame(PackageFile expected, object actual)
        {
            Assert.IsInstanceOf<PackageFile>(actual);
            var actualFile = actual as PackageFile;
            Assert.AreEqual(expected.SourceFile, actualFile.SourceFile);
            Assert.AreEqual(expected.TargetFileName, actualFile.TargetFileName);
            Assert.AreEqual(expected.TargetFolder, actualFile.TargetFolder);
        }

        #region GenerateModification()
        private Modification GenerateModification(string name, string type)
        {
            Modification modification = new Modification();
            modification.ChangeNumber = "1";
            modification.Comment = "A comment";
            modification.EmailAddress = "email@somewhere.com";
            modification.FileName = name;
            modification.ModifiedTime = new DateTime(2009, 1, 1);
            modification.Type = type;
            modification.UserName = "johnDoe";
            modification.Version = "1.1.1.1";
            return modification;
        }
        #endregion
        #endregion
    }
}
