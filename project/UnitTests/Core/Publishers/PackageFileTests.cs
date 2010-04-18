namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
    using ICSharpCode.SharpZipLib.Zip;
    using NUnit.Framework;
    using Rhino.Mocks;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Core.Publishers;
    using ThoughtWorks.CruiseControl.Core.Util;
    using System.IO;
    using System.Text;

    [TestFixture]
    public class PackageFileTests
    {
        private MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            this.mocks = new MockRepository();
        }

        [Test]
        public void HandlesShortDirectoryName()
        {
            var testFile = @"C:\Somewhere.txt";
            var result = mocks.StrictMock<IIntegrationResult>();
            SetupResult.For(result.WorkingDirectory).Return(@"C:\OnceUponATime\InALandFarFarWayFromHere\ThereLivedABeautifulPrincess");
            var fileSystem = mocks.StrictMock<IFileSystem>();
            Expect.Call(fileSystem.FileExists(testFile)).Return(true);
            var inputStream = new MemoryStream(Encoding.UTF8.GetBytes("This is some test data"));
            Expect.Call(fileSystem.GetFileLength(testFile)).Return(inputStream.Length);
            Expect.Call(fileSystem.OpenInputStream(testFile)).Return(inputStream);

            var file = new PackageFile(testFile);
            file.FileSystem = fileSystem;
            var outputStream = mocks.DynamicMock<ZipOutputStream>();

            mocks.ReplayAll();
            file.Package(result, outputStream);
            mocks.VerifyAll();
        }
    }
}
