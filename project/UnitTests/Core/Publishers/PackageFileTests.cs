namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
    using System.IO;
    using System.Text;
    using ICSharpCode.SharpZipLib.Zip;
    using Moq;
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Core.Publishers;
    using ThoughtWorks.CruiseControl.Core.Util;

    [TestFixture]
    public class PackageFileTests
    {
        private MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            this.mocks = new MockRepository(MockBehavior.Default);
        }

        [Test]
        public void HandlesShortDirectoryName()
        {
            var testFile = Platform.IsWindows ? @"C:\Somewhere.txt" : @"/Somewhere.txt";
            var result = mocks.Create<IIntegrationResult>(MockBehavior.Strict).Object;
            Mock.Get(result).SetupGet(_result => _result.WorkingDirectory).Returns(@"C:\OnceUponATime\InALandFarFarWayFromHere\ThereLivedABeautifulPrincess");
            var fileSystem = mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.FileExists(testFile)).Returns(true).Verifiable();
            var inputStream = new MemoryStream(Encoding.UTF8.GetBytes("This is some test data"));
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.GetFileLength(testFile)).Returns(inputStream.Length).Verifiable();
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.OpenInputStream(testFile)).Returns(inputStream).Verifiable();
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.GetLastWriteTime(testFile)).Returns(System.DateTime.Now).Verifiable();

            var file = new PackageFile(testFile);
            file.FileSystem = fileSystem;
            var stream = new MemoryStream();
            var outputStream = new ZipOutputStream(stream);

            file.Package(result, outputStream);
            mocks.VerifyAll();
        }
    }
}
