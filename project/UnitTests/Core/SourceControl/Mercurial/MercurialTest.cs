using System;
using System.IO;
using System.Text;
using Exortech.NetReflector;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol.Mercurial;
using ThoughtWorks.CruiseControl.Core.Util;
using System.Collections.Generic;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol.Mercurial
{
    [TestFixture]
    public class MercurialTest : ProcessExecutorTestFixtureBase
    {
        private class StubFileSystem : IFileSystem
        {
            public void Copy(string sourcePath, string destPath) { }

            public void Save(string file, string content) { }

            public void AtomicSave(string file, string content) { }

            public void AtomicSave(string file, string content, Encoding encoding) { }

            public TextReader Load(string file) { return null; }

            public bool FileExists(string file) { return true; }

            public bool DirectoryExists(string folder) { return true; }

            public void EnsureFolderExists(string fileName) { }

            /// <summary>
            /// Ensures that the specified file exists.
            /// </summary>
            /// <param name="fileName">The name of the file.</param>
            public void EnsureFileExists(string fileName)
            {
                throw new NotImplementedException();
            }

            public long GetFreeDiskSpace(string driveName) { return int.MaxValue; }

            public string[] GetFilesInDirectory(string directory) { return new string[0]; }
            public string[] GetFilesInDirectory(string directory, bool includeSubDirectories) { return new string[0]; }

            public DateTime GetLastWriteTime(string fileName) { return DateTime.MinValue; }

            public ITaskResult GenerateTaskResultFromFile(string fileName) { return null; }

            public Stream OpenOutputStream(string fileName) { return null;}

            public Stream OpenInputStream(string fileName) { return null; }

            #region CreateDirectory()
            /// <summary>
            /// Creates a directory.
            /// </summary>
            /// <param name="folder">The name of the folder to create.</param>
            public void CreateDirectory(string folder)
            {
            }
            #endregion

            public void DeleteFile(string filePath)
            {
            }

            #region DeleteDirectory()
            /// <summary>
            /// Deletes a directory.
            /// </summary>
            /// <param name="folder">The name of the folder to delete.</param>
            public void DeleteDirectory(string folder)
            {
            }

            /// <summary>
            /// Deletes a directory, optionally deleting all sub-directories.
            /// </summary>
            /// <param name="folder">The name of the folder to delete.</param>
            /// <param name="recursive">If set to <c>true</c> recursively delete folders.</param>
            public void DeleteDirectory(string folder, bool recursive)
            {
            }
            #endregion

            public long GetFileLength(string fullName)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Gets the files in directory.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="pattern">The pattern.</param>
            /// <param name="searchOption">The search option.</param>
            /// <returns>The files in the directory that match the pattern.</returns>
            public IEnumerable<string> GetFilesInDirectory(string path, string pattern, SearchOption searchOption)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void StubFileSystemCoverage()
        {
            StubFileSystem sf = new StubFileSystem();
            sf.Copy("asdf", "asdf");
            sf.Save("asdf", "Asdf");
            sf.Load("asdf");
            sf.FileExists("asdf");
            sf.DirectoryExists("asdf");
            sf.EnsureFolderExists("asdf");
            sf.GetFreeDiskSpace("asdf");
        }

        private CruiseControl.Core.Sourcecontrol.Mercurial.Mercurial hg;
        private IMock mockHistoryParser;
        private DateTime from;
        private DateTime to;
        private IMock mockFileSystem;

        [SetUp]
        protected void CreateHg()
        {
            mockHistoryParser = new DynamicMock(typeof (IHistoryParser));
            mockFileSystem = new DynamicMock(typeof (IFileSystem));
            CreateProcessExecutorMock(CruiseControl.Core.Sourcecontrol.Mercurial.Mercurial.DefaultExecutable);
            from = new DateTime(2001, 1, 21, 20, 0, 0);
            to = from.AddDays(1);
            setupHg(new StubFileSystem());
        }

        [TearDown]
        protected void VerifyAll()
        {
            Verify();
            mockHistoryParser.Verify();
            mockFileSystem.Verify();
        }

        /// <summary>
        /// Hg should be the default executable
        /// (hg.bat in python scripts dir or simply hg in python scripts dir in a mono environment).
        /// </summary>
        [Test]
        public void HgShouldBeDefaultExecutable() { Assert.AreEqual("hg", hg.Executable); }

        [Test]
        public void PopulateFromFullySpecifiedXml()
        {
            const string xml = @"
<hg>
	<executable>c:\Python25\Scripts\hg.bat</executable>
	<repo>c:\hg\ccnet\myhgrepo</repo>
	<timeout>5</timeout>
	<workingDirectory>c:\hg\working</workingDirectory>
	<tagOnSuccess>true</tagOnSuccess>
	<autoGetSource>true</autoGetSource>
    <branch>trunk</branch>
</hg>";

            hg = (CruiseControl.Core.Sourcecontrol.Mercurial.Mercurial) NetReflector.Read(xml);
            Assert.AreEqual(@"c:\Python25\Scripts\hg.bat", hg.Executable);
            Assert.AreEqual(@"c:\hg\ccnet\myhgrepo", hg.Repo);
            Assert.AreEqual(new Timeout(5), hg.Timeout);
            Assert.AreEqual(@"c:\hg\working", hg.WorkingDirectory);
            Assert.AreEqual(true, hg.TagOnSuccess);
            Assert.AreEqual(true, hg.AutoGetSource);
            Assert.AreEqual("trunk", hg.Branch);
        }

        [Test]
        public void PopulateFromMinimallySpecifiedXml()
        {
            const string xml = @"<hg/>";
            hg = (CruiseControl.Core.Sourcecontrol.Mercurial.Mercurial) NetReflector.Read(xml);
        }

        [Test]
        public void ShouldApplyLabelIfTagOnSuccessTrue()
        {
            hg.TagOnSuccess = true;

            ExpectToExecuteArguments(@"tag -m ""Tagging successful build foo"" --noninteractive foo", @"c:\source\");
            ExpectToExecuteArguments(@"push --noninteractive", @"c:\source\");

            hg.LabelSourceControl(IntegrationResultMother.CreateSuccessful("foo"));
        }

        [Test]
        public void ShouldApplyLabelWithCustomMessageIfTagOnSuccessTrueAndACustomMessageIsSpecified()
        {
            hg.TagOnSuccess = true;
            hg.TagCommitMessage = "a---- {0} ----a";

            ExpectToExecuteArguments(@"tag -m ""a---- foo ----a"" --noninteractive foo", @"c:\source\");
            ExpectToExecuteArguments(@"push --noninteractive", @"c:\source\");

            hg.LabelSourceControl(IntegrationResultMother.CreateSuccessful("foo"));
        }

        [Test]
        public void ShouldBuildUrlIfUrlBuilderSpecified()
        {
            IMock mockUrlBuilder = new DynamicMock(typeof (IModificationUrlBuilder));
            Modification[] modifications = new Modification[2] {new Modification(), new Modification()};
            hg.UrlBuilder = (IModificationUrlBuilder) mockUrlBuilder.MockInstance;

            mockProcessExecutor.ExpectAndReturn("Execute", new ProcessResult("", "", 0, false), new IsAnything());
            mockProcessExecutor.ExpectAndReturn("Execute", new ProcessResult("", "", 0, false), new IsAnything());
            mockProcessExecutor.ExpectAndReturn("Execute", new ProcessResult("3", "", 0, false), new IsAnything());
            mockProcessExecutor.ExpectAndReturn("Execute", new ProcessResult("4", "", 0, false), new IsAnything());
            mockHistoryParser.ExpectAndReturn("Parse", modifications, new IsAnything(), new IsAnything(), new IsAnything());
            mockUrlBuilder.ExpectAndReturn("SetupModification", modifications, new object[] {modifications});

            Modification[] result = hg.GetModifications(IntegrationResult(from), IntegrationResult(to));
            Assert.AreEqual(modifications, result);
            mockUrlBuilder.Verify();
        }

        [Test]
        public void ShouldCheckForMultipleHeadsAndGetSourceModificationsFound()
        {
            hg.AutoGetSource = true;
            hg.MultipleHeadsFail = true;
            Modification[] singleHead = new Modification[1];

            ExpectToExecuteArguments("heads --template <modification><node>{node|short}</node><author>{author|user}</author><date>{date|rfc822date}</date><desc>{desc|escape}</desc><rev>{rev}</rev><email>{author|email|obfuscate}</email><files>{files}</files></modification> --noninteractive", @"c:\source\");
            mockHistoryParser.ExpectAndReturn("Parse", singleHead, new IsAnything(), new IsAnything(), new IsAnything());
            ExpectToExecuteArguments("update --noninteractive", @"c:\source\");

            hg.GetSource(IntegrationResult());
        }

        [Test]
        public void ShouldCreateWorkingDirectoryIfItDoesntExist()
        {
            setupHg((IFileSystem) mockFileSystem.MockInstance);

            mockFileSystem.Expect("EnsureFolderExists", @"c:\source\");

            mockFileSystem.ExpectAndReturn("DirectoryExists", false, @"c:\source\.hg");
            ExpectToExecuteArguments("init --noninteractive", @"c:\source\");

            ExpectToExecuteArguments("pull --noninteractive", @"c:\source\");
            ExpectToExecuteWithArgumentsAndReturn("parents --template {rev} --noninteractive", new ProcessResult("", "", 0, false));
            ExpectToExecuteWithArgumentsAndReturn("log -r tip --template {rev} --noninteractive", new ProcessResult("4", "", 0, false));
            ExpectToExecuteArguments("log -r 0:4 --template <modification><node>{node|short}</node><author>{author|user}</author><date>{date|rfc822date}</date><desc>{desc|escape}</desc><rev>{rev}</rev><email>{author|email|obfuscate}</email><files>{files}</files></modification> --noninteractive", @"c:\source\");

            hg.GetModifications(IntegrationResult(from), IntegrationResult(to));
        }

        [Test]
        public void ShouldGetModificationsWithBranchNameIfSpecified()
        {
            hg.Branch = "branch";

            ExpectToExecuteArguments("pull -r branch --noninteractive");
            ExpectToExecuteWithArgumentsAndReturn("parents --template {rev} --noninteractive", new ProcessResult("3", "", 0, false));
            ExpectToExecuteWithArgumentsAndReturn("log -r tip --template {rev} --noninteractive", new ProcessResult("4", "", 0, false));
            ExpectToExecuteArguments("log -r 4:4 --template <modification><node>{node|short}</node><author>{author|user}</author><date>{date|rfc822date}</date><desc>{desc|escape}</desc><rev>{rev}</rev><email>{author|email|obfuscate}</email><files>{files}</files></modification> --noninteractive");

            hg.GetModifications(IntegrationResult(from), IntegrationResult(to));
        }

        private void ExpectToExecuteWithArgumentsAndReturn(string args, ProcessResult returnValue) { mockProcessExecutor.ExpectAndReturn("Execute", returnValue, NewProcessInfo(args, @"c:\source\")); }
        private new void ExpectToExecuteArguments(string args) { ExpectToExecuteArguments(args, @"c:\source\"); }

        [Test]
        public void ShouldGetModificationsWithRepoNameIfSpecified()
        {
            hg.Repo = @"c:\myrepo\";
            Modification[] modifications = new Modification[2] {new Modification(), new Modification()};

            ExpectToExecuteArguments(@"pull --noninteractive c:\myrepo\");
            ExpectToExecuteWithArgumentsAndReturn("parents --template {rev} --noninteractive", new ProcessResult("3", "", 0, false));
            ExpectToExecuteWithArgumentsAndReturn("log -r tip --template {rev} --noninteractive", new ProcessResult("4", "", 0, false));
            ExpectToExecuteArguments("log -r 4:4 --template <modification><node>{node|short}</node><author>{author|user}</author><date>{date|rfc822date}</date><desc>{desc|escape}</desc><rev>{rev}</rev><email>{author|email|obfuscate}</email><files>{files}</files></modification> --noninteractive");
            mockHistoryParser.ExpectAndReturn("Parse", modifications, new IsAnything(), new IsAnything(), new IsAnything());

            Modification[] result = hg.GetModifications(IntegrationResult(from), IntegrationResult(to));
            Assert.AreEqual(modifications, result);
        }

        [Test]
        public void ShouldGetSourceIfModificationsFound()
        {
            hg.AutoGetSource = true;
            hg.MultipleHeadsFail = false;

            ExpectToExecuteArguments("update --noninteractive");

            hg.GetSource(IntegrationResult());
        }

        [Test]
        public void ShouldNotApplyLabelIfIntegrationFailed()
        {
            hg.TagOnSuccess = true;

            ExpectThatExecuteWillNotBeCalled();

            hg.LabelSourceControl(IntegrationResultMother.CreateFailed());
        }

        [Test]
        public void ShouldNotApplyLabelIfTagOnSuccessFalse()
        {
            hg.TagOnSuccess = false;

            ExpectThatExecuteWillNotBeCalled();

            hg.LabelSourceControl(IntegrationResultMother.CreateSuccessful());
        }

        [Test]
        public void ShouldNotGetSourceIfAutoGetSourceFalse()
        {
            hg.AutoGetSource = false;

            ExpectThatExecuteWillNotBeCalled();

            hg.GetSource(IntegrationResult());
        }

        [Test]
        public void ShouldPullAndLogWhenNoPropertiesAreSet()
        {
            ExpectToExecuteArguments("pull --noninteractive");
            ExpectToExecuteWithArgumentsAndReturn("parents --template {rev} --noninteractive", new ProcessResult("3", "", 0, false));
            ExpectToExecuteWithArgumentsAndReturn("log -r tip --template {rev} --noninteractive", new ProcessResult("4", "", 0, false));
            ExpectToExecuteArguments("log -r 4:4 --template <modification><node>{node|short}</node><author>{author|user}</author><date>{date|rfc822date}</date><desc>{desc|escape}</desc><rev>{rev}</rev><email>{author|email|obfuscate}</email><files>{files}</files></modification> --noninteractive");

            hg.GetModifications(IntegrationResult(from), IntegrationResult(to));
        }

        [Test]
        public void ShouldPushTagCommitToRepoIfSpecified()
        {
            hg.TagOnSuccess = true;
            hg.Repo = @"c:\myrepo\";

            ExpectToExecuteArguments(@"tag -m ""Tagging successful build foo"" --noninteractive foo");
            ExpectToExecuteArguments(@"push --noninteractive c:\myrepo\");

            hg.LabelSourceControl(IntegrationResultMother.CreateSuccessful("foo"));
        }

        [Test]
        public void ShouldReturnModifications()
        {
            Modification[] modifications = new Modification[2] {new Modification(), new Modification()};

            ExpectToExecuteArguments("pull --noninteractive");
            ExpectToExecuteWithArgumentsAndReturn("parents --template {rev} --noninteractive", new ProcessResult("3", "", 0, false));
            ExpectToExecuteWithArgumentsAndReturn("log -r tip --template {rev} --noninteractive", new ProcessResult("4", "", 0, false));
            ExpectToExecuteArguments("log -r 4:4 --template <modification><node>{node|short}</node><author>{author|user}</author><date>{date|rfc822date}</date><desc>{desc|escape}</desc><rev>{rev}</rev><email>{author|email|obfuscate}</email><files>{files}</files></modification> --noninteractive");
            mockHistoryParser.ExpectAndReturn("Parse", modifications, new IsAnything(), from, new IsAnything());

            Modification[] result = hg.GetModifications(IntegrationResult(from), IntegrationResult(to));
            Assert.AreEqual(modifications, result);
        }

        [Test]
        public void ShouldThrowMultipleHeadsExceptionWhenMultipleHeadsAreFound()
        {
            hg.AutoGetSource = true;
            hg.MultipleHeadsFail = true;
            Modification[] heads = new Modification[2];

            ExpectToExecuteArguments("heads --template <modification><node>{node|short}</node><author>{author|user}</author><date>{date|rfc822date}</date><desc>{desc|escape}</desc><rev>{rev}</rev><email>{author|email|obfuscate}</email><files>{files}</files></modification> --noninteractive");
            mockHistoryParser.ExpectAndReturn("Parse", heads, new IsAnything(), new IsAnything(), new IsAnything());

            Assert.That(delegate { hg.GetSource(IntegrationResult()); },
                        Throws.TypeOf<MultipleHeadsFoundException>());
        }

        [Test]
        public void ShouldUseBranchNameWhenGettingSourceIfSpecified()
        {
            hg.AutoGetSource = true;
            hg.MultipleHeadsFail = true;
            hg.Branch = "branch";
            Modification[] singleHead = new Modification[1];

            ExpectToExecuteArguments("heads -r branch --template <modification><node>{node|short}</node><author>{author|user}</author><date>{date|rfc822date}</date><desc>{desc|escape}</desc><rev>{rev}</rev><email>{author|email|obfuscate}</email><files>{files}</files></modification> --noninteractive");
            mockHistoryParser.ExpectAndReturn("Parse", singleHead, new IsAnything(), new IsAnything(), new IsAnything());
            ExpectToExecuteArguments("update -r branch --noninteractive");

            hg.GetSource(IntegrationResult());
        }

        private void setupHg(IFileSystem filesystem)
        {
            hg = new CruiseControl.Core.Sourcecontrol.Mercurial.Mercurial((IHistoryParser) mockHistoryParser.MockInstance, (ProcessExecutor) mockProcessExecutor.MockInstance, filesystem);
            hg.WorkingDirectory = @"c:\source\";
        }
    }
}
