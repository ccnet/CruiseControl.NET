using System;
using System.IO;
using System.Text;
using Exortech.NetReflector;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{

    [TestFixture]
    public class SvnTest : ProcessExecutorTestFixtureBase
    {
        private Svn svn;
        private Mock<IHistoryParser> mockHistoryParser;
        private DateTime from;
        private DateTime to;
        private Mock<IFileSystem> mockFileSystem;

        [SetUp]
        protected void SetUp()
        {
            from = DateTime.Parse("2001-01-21 20:00:00Z");
            to = DateTime.Parse("2001-01-21 20:30:50Z");
            CreateProcessExecutorMock(Svn.DefaultExecutable);
            mockHistoryParser = new Mock<IHistoryParser>();
            mockFileSystem = new Mock<IFileSystem>();
            svn = new Svn((ProcessExecutor)mockProcessExecutor.Object, (IHistoryParser)mockHistoryParser.Object, (IFileSystem)mockFileSystem.Object);
            svn.TrunkUrl = "svn://myserver/mypath";
            svn.TagBaseUrl = "svn://someserver/tags/foo";
            svn.WorkingDirectory = DefaultWorkingDirectory;
        }

        [TearDown]
        protected void TearDown()
        {
            Verify();
            mockHistoryParser.Verify();
            mockFileSystem.Verify();
        }

        [Test]
        public void ShouldGetModificationsEvenWhenTrunkUrlIsNull()
        {
            svn.TrunkUrl = null;
            mockHistoryParser.Setup(parser => parser.Parse(It.IsAny<TextReader>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(new Modification[0]).Verifiable();
            ExpectToExecuteArguments("log -r \"{2001-01-21T20:00:00Z}:{2001-01-21T20:30:50Z}\" --verbose --xml --no-auth-cache --non-interactive");

            Modification[] modifications = svn.GetModifications(IntegrationResult(from), IntegrationResult(to));

            Assert.AreEqual(new Modification[0], modifications);
        }

        [Test]
        public void PopulateFromFullySpecifiedXml()
        {
            string xml = @"
<svn>
	<executable>c:\svn\svn.exe</executable>
	<trunkUrl>svn://myserver/mypath</trunkUrl>
	<timeout>5</timeout>
	<workingDirectory>c:\dev\src</workingDirectory>
	<username>user</username>
	<password>password</password>
	<tagOnSuccess>true</tagOnSuccess>
    <tagCommitMessage>MyTagMessage</tagCommitMessage>
    <tagNameFormat>MyTagNameFormat</tagNameFormat>
	<tagWorkingCopy>true</tagWorkingCopy>
	<tagBaseUrl>svn://myserver/mypath/tags</tagBaseUrl>
	<autoGetSource>true</autoGetSource>
	<checkExternals>true</checkExternals>
</svn>";

            svn = (Svn)NetReflector.Read(xml);
            Assert.AreEqual(@"c:\svn\svn.exe", svn.Executable);
            Assert.AreEqual("svn://myserver/mypath", svn.TrunkUrl);
            Assert.AreEqual(new Timeout(5), svn.Timeout);
            Assert.AreEqual(@"c:\dev\src", svn.WorkingDirectory);
            Assert.AreEqual("user", svn.Username);
            Assert.AreEqual("password", svn.Password.PrivateValue);
            Assert.AreEqual(true, svn.TagOnSuccess);
            Assert.AreEqual("MyTagMessage", svn.TagCommitMessage);
            Assert.AreEqual("MyTagNameFormat", svn.TagNameFormat);
            Assert.AreEqual(true, svn.TagWorkingCopy);
            Assert.AreEqual(true, svn.AutoGetSource);
            Assert.AreEqual(true, svn.CheckExternals);
            Assert.AreEqual("svn://myserver/mypath/tags", svn.TagBaseUrl);
        }

        [Test]
        public void SpecifyFromMinimallySpecifiedXml()
        {
            string xml = @"<svn/>";
            svn = (Svn)NetReflector.Read(xml);
            Assert.AreEqual("svn", svn.Executable);
            Assert.AreEqual(false, svn.TagWorkingCopy);
        }

        [Test]
        public void CreatingHistoryProcessIncludesCorrectlyFormattedArguments()
        {
            ExpectToExecuteArguments("log svn://myserver/mypath -r \"{2001-01-21T20:00:00Z}:{2001-01-21T20:30:50Z}\" --verbose --xml --no-auth-cache --non-interactive");
            svn.GetModifications(IntegrationResult(from), IntegrationResult(to));
        }

        [Test]
        public void CreatingHistoryProcessIncludesCorrectlyFormattedArgumentsForUsernameAndPassword()
        {
            string args = @"log svn://myserver/mypath -r ""{2001-01-21T20:00:00Z}:{2001-01-21T20:30:50Z}"" --verbose --xml --username user --password password --no-auth-cache --non-interactive";
            ExpectToExecuteArguments(args);
            svn.Username = "user";
            svn.Password = "password";
            svn.GetModifications(IntegrationResult(from), IntegrationResult(to));
        }

        [Test]
        public void CreatingHistoryProcessShouldQuoteTrunkUrl()
        {
            ExpectToExecuteArguments("log \"svn://my server/mypath\" -r \"{2001-01-21T20:00:00Z}:{2001-01-21T20:30:50Z}\" --verbose --xml --no-auth-cache --non-interactive");
            svn.TrunkUrl = "svn://my server/mypath";
            svn.GetModifications(IntegrationResult(from), IntegrationResult(to));
        }

        [Test]
        public void CreatingHistoryProcessShouldHandleImplicitTrunkUrl()
        {
            ExpectToExecuteArguments("log -r \"{2001-01-21T20:00:00Z}:{2001-01-21T20:30:50Z}\" --verbose --xml --no-auth-cache --non-interactive");
            svn.TrunkUrl = null;
            svn.GetModifications(IntegrationResult(from), IntegrationResult(to));
        }

        [Test]
        public void ShouldRebaseWorkingDirectoryForHistory()
        {
            ExpectToExecuteArguments("log svn://myserver/mypath -r \"{2001-01-21T20:00:00Z}:{2001-01-21T20:30:50Z}\" --verbose --xml --no-auth-cache --non-interactive");
            svn.WorkingDirectory = DefaultWorkingDirectory;
            IIntegrationResult result = IntegrationResult(to);
            result.WorkingDirectory = DefaultWorkingDirectory;
            svn.GetModifications(IntegrationResult(from), result);
        }

        [Test]
        public void ShouldApplyLabelIfTagOnSuccessTrue()
        {
            ExpectToExecuteArguments(string.Format(@"copy -m ""CCNET build foo"" {0} svn://someserver/tags/foo/foo --no-auth-cache --non-interactive", StringUtil.AutoDoubleQuoteString(DefaultWorkingDirectory)));
            svn.TagOnSuccess = true;
            svn.LabelSourceControl(IntegrationResultMother.CreateSuccessful("foo"));
        }

        [Test]
        public void ShouldApplyLabelWithCustomMessageIfTagOnSuccessTrueAndACustomMessageIsSpecified()
        {
            ExpectToExecuteArguments(string.Format(@"copy -m ""a---- foo ----a"" {0} svn://someserver/tags/foo/foo --no-auth-cache --non-interactive", StringUtil.AutoDoubleQuoteString(DefaultWorkingDirectory)));

            svn.TagOnSuccess = true;
            svn.TagCommitMessage = "a---- {0} ----a";
            svn.LabelSourceControl(IntegrationResultMother.CreateSuccessful("foo"));
        }

        [Test]
        public void ShouldApplyTagNameFormatWithCustomFormatIfTagOnSuccessTrueAndATagNameFormatIsSpecified()
        {
            ExpectToExecuteArguments(string.Format(@"copy -m ""CCNET build bar"" {0} svn://someserver/tags/foo/Build/bar --no-auth-cache --non-interactive", StringUtil.AutoDoubleQuoteString(DefaultWorkingDirectory)));

            svn.TagOnSuccess = true;
            svn.TagNameFormat = "Build/{0}";

            svn.LabelSourceControl(IntegrationResultMother.CreateSuccessful("bar"));
        }
        [Test]
        public void ShouldApplyLabelUsingRebasedWorkingDirectory()
        {
            ExpectToExecuteArguments(string.Format(@"copy -m ""CCNET build foo"" {0} svn://someserver/tags/foo/foo --no-auth-cache --non-interactive", StringUtil.AutoDoubleQuoteString(DefaultWorkingDirectory)));
            svn.TagOnSuccess = true;
            svn.WorkingDirectory = null;
            IIntegrationResult result = IntegrationResult(from);
            result.Label = "foo";
            svn.LabelSourceControl(result);
        }

        [Test]
        public void CreatingLabelProcessPerformsServerToServerCopyWithRevisionWhenKnown()
        {
            IntegrationResult result = IntegrationResultMother.CreateSuccessful("foo");
            Modification mod = new Modification
            {
                ModifiedTime = new DateTime(2009, 1, 1),
                ChangeNumber = "5"
            };
            svn.latestRevision = 5;
            result.Modifications = new Modification[] { mod };
            svn.mods = result.Modifications;
            ExpectToExecuteArguments(@"copy -m ""CCNET build foo"" svn://myserver/mypath svn://someserver/tags/foo --revision 5 --no-auth-cache --non-interactive");

            svn.TagOnSuccess = true;
            svn.TagBaseUrl = "svn://someserver/tags";
            svn.LabelSourceControl(result);
        }

        [Test]
        public void ShouldNotApplyLabelIfTagOnSuccessFalse()
        {
            svn.TagOnSuccess = false;
            svn.LabelSourceControl(IntegrationResultMother.CreateSuccessful());
            mockProcessExecutor.VerifyNoOtherCalls();
        }

        [Test]
        public void ShouldNotApplyLabelIfIntegrationFailed()
        {
            svn.TagOnSuccess = true;
            svn.LabelSourceControl(IntegrationResultMother.CreateFailed());
            mockProcessExecutor.VerifyNoOtherCalls();
        }

        [Test]
        public void CreatingLabelProcessIncludesCorrectlyFormattedArgumentsForUsernameAndPassword()
        {
            string args = string.Format(@"copy -m ""CCNET build foo"" {0} svn://someserver/tags/foo --username user --password password --no-auth-cache --non-interactive", StringUtil.AutoDoubleQuoteString(DefaultWorkingDirectory));
            ExpectToExecuteArguments(args);

            IIntegrationResult result = IntegrationResult();
            result.Label = "foo";

            svn.TagOnSuccess = true;
            svn.Username = "user";
            svn.Password = "password";
            svn.TagBaseUrl = "svn://someserver/tags";
            svn.LabelSourceControl(result);
        }

        [Test]
        public void CreateLabelFromWorkingCopyWhenTagWorkingCopyTrue()
        {
            string args = string.Format(@"copy -m ""CCNET build foo"" {0} svn://someserver/tags/foo --no-auth-cache --non-interactive", StringUtil.AutoDoubleQuoteString(DefaultWorkingDirectory));
            ExpectToExecuteArguments(args);

            IIntegrationResult result = IntegrationResult();
            result.Label = "foo";
            svn.latestRevision = 10;
            svn.TagOnSuccess = true;
            svn.TagWorkingCopy = true;
            svn.TagBaseUrl = "svn://someserver/tags";
            svn.LabelSourceControl(result);
        }

        [Test]
        public void ShouldGetSourceWithAppropriateRevisionNumberIfTagOnSuccessTrueAndModificationsFound()
        {
            ExpectSvnDirectoryExists(true);
            ExpectToExecuteArguments(string.Format(@"update {0} --revision 10 --no-auth-cache --non-interactive", StringUtil.AutoDoubleQuoteString(DefaultWorkingDirectory)));

            IIntegrationResult result = IntegrationResult();
            Modification mod = new Modification();
            mod.ChangeNumber = "10";
            svn.latestRevision = 10;
            result.Modifications = new Modification[] { mod };
            svn.mods = result.Modifications;

            svn.AutoGetSource = true;
            svn.GetSource(result);
        }

        // This would happen, e.g., for force build
        [Test]
        public void ShouldGetSourceWithoutRevisionNumberIfTagOnSuccessTrueAndModificationsNotFound()
        {
            ExpectSvnDirectoryExists(true);
            ExpectToExecuteArguments(string.Format(@"update {0} --no-auth-cache --non-interactive", StringUtil.AutoDoubleQuoteString(DefaultWorkingDirectory)));
            svn.AutoGetSource = true;
            svn.GetSource(IntegrationResult());
        }

        [Test]
        public void ShouldGetSourceWithCredentialsIfSpecifiedIfAutoGetSourceTrue()
        {
            ExpectSvnDirectoryExists(true);
            ExpectToExecuteArguments(string.Format(@"update {0} --username ""Buck Rogers"" --password ""My Password"" --no-auth-cache --non-interactive", StringUtil.AutoDoubleQuoteString(DefaultWorkingDirectory)));
            svn.Username = "Buck Rogers";
            svn.Password = "My Password";
            svn.AutoGetSource = true;
            svn.GetSource(IntegrationResult());
        }

        [Test]
        public void ShouldGetSourceWithSpacesInPath()
        {
            mockFileSystem.Setup(fileSystem => fileSystem.DirectoryExists(Path.Combine(DefaultWorkingDirectoryWithSpaces, ".svn"))).Returns(true).Verifiable();

            ExpectToExecuteArguments(string.Format(@"update {0} --no-auth-cache --non-interactive", StringUtil.AutoDoubleQuoteString(DefaultWorkingDirectoryWithSpaces)), DefaultWorkingDirectoryWithSpaces);
            svn.AutoGetSource = true;
            svn.WorkingDirectory = DefaultWorkingDirectoryWithSpaces;
            svn.GetSource(IntegrationResult());
        }

        [Test]
        public void ShouldNotGetSourceIfAutoGetSourceFalse()
        {
            svn.AutoGetSource = false;
            svn.GetSource(IntegrationResult());
            mockProcessExecutor.VerifyNoOtherCalls();
        }

        [Test]
        public void ShouldCheckoutInsteadOfUpdateIfSVNFoldersDoNotExist()
        {
            ExpectToExecuteArguments(string.Format(@"checkout svn://myserver/mypath {0} --no-auth-cache --non-interactive", StringUtil.AutoDoubleQuoteString(DefaultWorkingDirectory)));
            ExpectSvnDirectoryExists(false);
            ExpectUnderscoreSvnDirectoryExists(false);

            ExpectNoSvnParentDirectoryExists();

            svn.AutoGetSource = true;
            svn.WorkingDirectory = DefaultWorkingDirectory;
            svn.GetSource(IntegrationResult());
        }

        [Test]
        public void ShouldCheckoutWrappingTrunkUrlInDoubleQuotes()
        {
            ExpectToExecuteArguments(string.Format(@"checkout ""svn://myserver/my path"" {0} --no-auth-cache --non-interactive", StringUtil.AutoDoubleQuoteString(DefaultWorkingDirectory)));
            ExpectSvnDirectoryExists(false);
            ExpectUnderscoreSvnDirectoryExists(false);
            ExpectNoSvnParentDirectoryExists();

            svn.TrunkUrl = "svn://myserver/my path";
            svn.AutoGetSource = true;
            svn.WorkingDirectory = DefaultWorkingDirectory;
            svn.GetSource(IntegrationResult());
        }

        [Test]
        public void ShouldNotCheckoutIfSVNFoldersWithAspNetHackExist()
        {
            ExpectSvnDirectoryExists(false);
            ExpectUnderscoreSvnDirectoryExists(true);
            ExpectToExecuteArguments(string.Format(@"update {0} --no-auth-cache --non-interactive", StringUtil.AutoDoubleQuoteString(DefaultWorkingDirectory)));

            svn.AutoGetSource = true;
            svn.WorkingDirectory = DefaultWorkingDirectory;
            svn.GetSource(IntegrationResult());
        }

        [Test]
        public void ShouldThrowExceptionIfTrunkUrlIsNotSpecifiedAndSVNFoldersDoNotExist()
        {
            ExpectSvnDirectoryExists(false);
            ExpectUnderscoreSvnDirectoryExists(false);
            ExpectNoSvnParentDirectoryExists();

            svn.TrunkUrl = string.Empty;
            svn.AutoGetSource = true;
            svn.WorkingDirectory = DefaultWorkingDirectory;
            Assert.That(delegate { svn.GetSource(IntegrationResult()); },
                        Throws.TypeOf<ConfigurationException>());
        }

        [Test]
        public void SvnProcessInfoShouldSetEncodingToUTF8()
        {
            ExpectSvnDirectoryExists(false);
            ExpectUnderscoreSvnDirectoryExists(false);
            ExpectNoSvnParentDirectoryExists();

            mockProcessExecutor.Setup(executor => executor.Execute(It.Is<ProcessInfo>(info => Encoding.UTF8 == info.StreamEncoding))).Returns(SuccessfulProcessResult()).Verifiable();

            svn.GetSource(IntegrationResult());
        }

        private void ExpectSvnDirectoryExists(bool doesSvnDirectoryExist)
        {
            string folder = Path.Combine(DefaultWorkingDirectory, ".svn");
            mockFileSystem.Setup(fileSystem => fileSystem.DirectoryExists(folder)).Returns(doesSvnDirectoryExist).Verifiable();
        }

        private void ExpectUnderscoreSvnDirectoryExists(bool doesSvnDirectoryExist)
        {
            string folder = Path.Combine(DefaultWorkingDirectory, "_svn");
            mockFileSystem.Setup(fileSystem => fileSystem.DirectoryExists(folder)).Returns(doesSvnDirectoryExist).Verifiable();
        }

        private void ExpectNoSvnParentDirectoryExists()
        {
            var parent = Directory.GetParent(DefaultWorkingDirectory);

            while (parent != null)
            {
                string folder1 = Path.Combine(parent.FullName, ".svn");
                mockFileSystem.Setup(fileSystem => fileSystem.DirectoryExists(folder1)).Returns(false).Verifiable();
                string folder2 = Path.Combine(parent.FullName, "_svn");
                mockFileSystem.Setup(fileSystem => fileSystem.DirectoryExists(folder2)).Returns(false).Verifiable();
                parent = Directory.GetParent(parent.FullName);
            }
        }

    }
}
