namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol.Mercurial
{
	using System;
	using System.IO;
	using Exortech.NetReflector;
	using Moq;
	using NUnit.Framework;
	using ThoughtWorks.CruiseControl.Core;
	using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
	using ThoughtWorks.CruiseControl.Core.Sourcecontrol.Mercurial;
	using ThoughtWorks.CruiseControl.Core.Util;

	/// <summary>
	/// Test fixture for the <see cref="Mercurial"/> source control implementation.
	/// </summary>
	[TestFixture]
	public class MercurialTest : ProcessExecutorTestFixtureBase
	{
		#region Constants

		private const string ConfigMin = "<hg/>";
		private const string ConfigFull = @"
<hg>
<autoGetSource>true</autoGetSource>
<branch>trunk</branch>
<commitModifications>true</commitModifications>
<committerName>CCNet</committerName>
<commitUntracked>true</commitUntracked>
<executable>c:\Python25\Scripts\hg.bat</executable>
<modificationsCommitMessage>Modifications for build {0}</modificationsCommitMessage>
<multipleHeadsFail>false</multipleHeadsFail>
<purgeModifications>true</purgeModifications>
<pushModifications>true</pushModifications>
<repo>c:\hg\ccnet\myhgrepo</repo>
<tagCommitMessage>Tag for build {0}</tagCommitMessage>
<tagNameFormat>tag_{0}</tagNameFormat>
<tagOnSuccess>true</tagOnSuccess>
</hg>";

		#endregion

		#region Private Members

		private Mercurial hg;
		private DateTime from;
		private DateTime to;
		private Mock<IFileSystem> mockFileSystem;
		private Mock<IFileDirectoryDeleter> mockFileDirectoryDeleter;
		private Mock<IHistoryParser> mockHistoryParser;
		private string tempWorkDir;
		private string tempHgDir;
		private string outputTemplate;

		#endregion

		#region SetUp Method

		[SetUp]
		protected void SetUp()
		{
			mockHistoryParser = new Mock<IHistoryParser>();
			mockFileSystem = new Mock<IFileSystem>();
			mockFileDirectoryDeleter = new Mock<IFileDirectoryDeleter>();
			CreateProcessExecutorMock(Mercurial.DefaultExecutable);
			from = new DateTime(2001, 1, 21, 20, 0, 0);
			to = from.AddDays(1);

			tempWorkDir = TempFileUtil.CreateTempDir("ccnet-hg-test");
			tempHgDir = Path.Combine(TempFileUtil.CreateTempDir("ccnet-hg-test"), ".hg");
			Directory.CreateDirectory(tempHgDir);
			outputTemplate = Path.Combine(tempHgDir, "ccnet.template");

			hg = new Mercurial((IHistoryParser) mockHistoryParser.Object, (ProcessExecutor) mockProcessExecutor.Object, new StubFileSystem(), new StubFileDirectoryDeleter());
			hg.WorkingDirectory = tempWorkDir;
		}

		#endregion

		#region TearDown Method

		[TearDown]
		protected void TearDown()
		{
			Verify();
			mockHistoryParser.Verify();
			mockFileSystem.Verify();
			mockFileDirectoryDeleter.Verify();
			TempFileUtil.DeleteTempDir(tempWorkDir);
			TempFileUtil.DeleteTempDir(tempHgDir);
		}

		#endregion

		#region Configuration Tests

		[Test]
		public void DefaultConfigurationTest()
		{
			Assert.That(hg.AutoGetSource, Is.True);
			Assert.That(hg.Branch, Is.Null.Or.Empty);
			Assert.That(hg.CommitModifications, Is.False);
			Assert.That(hg.CommitterName, Is.EqualTo("CruiseControl.NET"));
			Assert.That(hg.CommitUntracked, Is.False);
			Assert.That(hg.Executable, Is.EqualTo("hg"));
			Assert.That(hg.MultipleHeadsFail, Is.False);
			Assert.That(hg.ModificationsCommitMessage, Is.EqualTo("Modifications of CC.NET build {0}"));
			Assert.That(hg.PurgeModifications, Is.False);
			Assert.That(hg.PushModifications, Is.False);
			Assert.That(hg.Repository, Is.Null.Or.Empty);
			Assert.That(hg.TagCommitMessage, Is.EqualTo("Tagging CC.NET build {0}"));
			Assert.That(hg.TagNameFormat, Is.EqualTo("ccnet_build_{0}"));
			Assert.That(hg.TagOnSuccess, Is.False);
		}

		[Test]
		public void PopulateFromFullySpecifiedXml()
		{
			hg = (CruiseControl.Core.Sourcecontrol.Mercurial.Mercurial) NetReflector.Read(ConfigFull);

			Assert.That(hg.AutoGetSource, Is.True);
			Assert.That(hg.Branch, Is.EqualTo("trunk"));
			Assert.That(hg.CommitModifications, Is.True);
			Assert.That(hg.CommitterName, Is.EqualTo("CCNet"));
			Assert.That(hg.CommitUntracked, Is.True);
			Assert.That(hg.Executable, Is.EqualTo(@"c:\Python25\Scripts\hg.bat"));
			Assert.That(hg.MultipleHeadsFail, Is.False);
			Assert.That(hg.ModificationsCommitMessage, Is.EqualTo("Modifications for build {0}"));
			Assert.That(hg.PurgeModifications, Is.True);
			Assert.That(hg.PushModifications, Is.True);
			Assert.That(hg.Repository, Is.EqualTo(@"c:\hg\ccnet\myhgrepo"));
			Assert.That(hg.TagCommitMessage, Is.EqualTo("Tag for build {0}"));
			Assert.That(hg.TagNameFormat, Is.EqualTo("tag_{0}"));
			Assert.That(hg.TagOnSuccess, Is.True);
		}

		[Test]
		public void PopulateFromMinimallySpecifiedXml()
		{
			hg = (CruiseControl.Core.Sourcecontrol.Mercurial.Mercurial) NetReflector.Read(ConfigMin);

			Assert.That(hg.AutoGetSource, Is.True);
			Assert.That(hg.Branch, Is.Null.Or.Empty);
			Assert.That(hg.CommitModifications, Is.False);
			Assert.That(hg.CommitterName, Is.EqualTo("CruiseControl.NET"));
			Assert.That(hg.CommitUntracked, Is.False);
			Assert.That(hg.Executable, Is.EqualTo("hg"));
			Assert.That(hg.MultipleHeadsFail, Is.False);
			Assert.That(hg.ModificationsCommitMessage, Is.EqualTo("Modifications of CC.NET build {0}"));
			Assert.That(hg.PurgeModifications, Is.False);
			Assert.That(hg.PushModifications, Is.False);
			Assert.That(hg.Repository, Is.Null.Or.Empty);
			Assert.That(hg.TagCommitMessage, Is.EqualTo("Tagging CC.NET build {0}"));
			Assert.That(hg.TagNameFormat, Is.EqualTo("ccnet_build_{0}"));
			Assert.That(hg.TagOnSuccess, Is.False);
		}

		#endregion

		#region GetModifications() Workflow Tests

		[Test]
		public void ShouldBuildUrlIfUrlBuilderSpecified()
		{
			var mockUrlBuilder = new Mock<IModificationUrlBuilder>();
			Modification[] modifications = new Modification[2] {new Modification(), new Modification()};
			hg.UrlBuilder = (IModificationUrlBuilder) mockUrlBuilder.Object;
			MockSequence sequence = new MockSequence();
			mockProcessExecutor.InSequence(sequence).Setup(executor => executor.Execute(It.IsAny<ProcessInfo>())).Returns(new ProcessResult("", "", 0, false)).Verifiable();
			mockProcessExecutor.InSequence(sequence).Setup(executor => executor.Execute(It.IsAny<ProcessInfo>())).Returns(new ProcessResult("3", "", 0, false)).Verifiable();
			mockProcessExecutor.InSequence(sequence).Setup(executor => executor.Execute(It.IsAny<ProcessInfo>())).Returns(new ProcessResult("4", "", 0, false)).Verifiable();
			mockHistoryParser.Setup(parser => parser.Parse(It.IsAny<TextReader>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(modifications).Verifiable();
			mockUrlBuilder.Setup(builder => builder.SetupModification(modifications)).Verifiable();

			Modification[] result = hg.GetModifications(IntegrationResult(from), IntegrationResult(to));
			Assert.That(result, Is.EqualTo(modifications));
			mockUrlBuilder.Verify();
		}

		[Test]
		public void ShouldCreateWorkingDirectoryIfItDoesntExistOrIsNotARepository()
		{
			hg = new Mercurial((IHistoryParser) mockHistoryParser.Object, (ProcessExecutor) mockProcessExecutor.Object,
			                   (IFileSystem) mockFileSystem.Object, (IFileDirectoryDeleter) mockFileDirectoryDeleter.Object);
			hg.WorkingDirectory = tempWorkDir;
			hg.Repository = @"C:\foo";

			MockSequence sequence = new MockSequence();
			mockFileSystem.Setup(fileSystem => fileSystem.EnsureFolderExists(tempWorkDir)).Verifiable();
			mockFileSystem.Setup(fileSystem => fileSystem.EnsureFolderExists(tempHgDir)).Verifiable();
			mockFileSystem.InSequence(sequence).Setup(fileSystem => fileSystem.DirectoryExists(tempWorkDir)).Returns(true).Verifiable();
			mockFileSystem.Setup(fileSystem => fileSystem.DirectoryExists(tempHgDir)).Returns(false).Verifiable();
			mockFileDirectoryDeleter.Setup(deleter => deleter.DeleteIncludingReadOnlyObjects(tempWorkDir)).Verifiable();
			mockFileSystem.InSequence(sequence).Setup(fileSystem => fileSystem.DirectoryExists(tempWorkDir)).Returns(false).Verifiable();
			ExpectToExecuteArguments(sequence, @"init " + StringUtil.AutoDoubleQuoteString(tempWorkDir), Directory.GetParent(Path.GetFullPath(tempWorkDir)).FullName);
			ExpectToExecuteArguments(sequence, @"pull C:\foo", tempWorkDir);

			hg.GetModifications(IntegrationResult(from), IntegrationResult(to));
		}

		[Test]
		public void ShouldGetModificationsWithBranchNameIfSpecified()
		{
			hg.Branch = "branch";

			ExpectToExecuteWithArgumentsAndReturn("parents --template {rev}:", new ProcessResult("1:", "", 0, false));
			ExpectToExecuteWithArgumentsAndReturn("log -b branch -r branch --template {rev}", new ProcessResult("3", "", 0, false));
			ExpectToExecuteArguments("log -b branch -r 2:3 --style xml -v");

			hg.GetModifications(IntegrationResult(from), IntegrationResult(to));
		}

		[Test]
		public void ShouldGetModificationsWithRepoNameIfSpecified()
		{
			hg.Repository = @"c:\myrepo\";
			Modification[] modifications = new Modification[2] {new Modification(), new Modification()};

			ExpectToExecuteArguments(@"pull c:\myrepo\");
			ExpectToExecuteWithArgumentsAndReturn("parents --template {rev}:", new ProcessResult("2:3:", "", 0, false));
			ExpectToExecuteWithArgumentsAndReturn("log -r tip --template {rev}", new ProcessResult("5", "", 0, false));
			ExpectToExecuteArguments("log -r 3:5 --style xml -v");
			mockHistoryParser.Setup(parser => parser.Parse(It.IsAny<TextReader>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(modifications).Verifiable();

			Modification[] result = hg.GetModifications(IntegrationResult(from), IntegrationResult(to));
			Assert.That(result, Is.EqualTo(modifications));
		}

		[Test]
		public void ShouldOnlyParseLogWhenNoPropertiesAreSet()
		{
			ExpectToExecuteWithArgumentsAndReturn("parents --template {rev}:", new ProcessResult("1:", "", 0, false));
			ExpectToExecuteWithArgumentsAndReturn("log -r tip --template {rev}", new ProcessResult("3", "", 0, false));
			ExpectToExecuteArguments("log -r 2:3 --style xml -v");

			hg.GetModifications(IntegrationResult(from), IntegrationResult(to));
		}

		[Test]
		public void ShouldPullFromRemoteRepoIfSpecified()
		{
			hg.Repository = "http://somehost.org/repo.hg";

			ExpectToExecuteArguments(@"pull http://somehost.org/repo.hg");
			ExpectToExecuteWithArgumentsAndReturn(@"parents --template {rev}:", new ProcessResult("", "", 0, false));
			ExpectToExecuteWithArgumentsAndReturn(@"log -r tip --template {rev}", new ProcessResult("1", "", 0, false));
			ExpectToExecuteWithArgumentsAndReturn(@"log -r 0:1 --style xml -v", new ProcessResult("1", "", 0, false));

			hg.GetModifications(IntegrationResult(), IntegrationResult());
		}

		[Test]
		public void ShouldPullBranchFromRemoteRepoIfSpecified()
		{
			hg.Repository = "http://somehost.org/repo.hg";
			hg.Branch = "trunk";

			ExpectToExecuteArguments(@"pull -b trunk http://somehost.org/repo.hg");
			ExpectToExecuteWithArgumentsAndReturn(@"parents --template {rev}:", new ProcessResult("", "", 0, false));
			ExpectToExecuteWithArgumentsAndReturn(@"log -b trunk -r trunk --template {rev}", new ProcessResult("1", "", 0, false));
			ExpectToExecuteWithArgumentsAndReturn(@"log -b trunk -r 0:1 --style xml -v", new ProcessResult("1", "", 0, false));

			hg.GetModifications(IntegrationResult(), IntegrationResult());
		}

		[Test]
		public void ShouldNotReturnModificationsWhenParentIsEqualToTip()
		{
			ExpectToExecuteWithArgumentsAndReturn("parents --template {rev}:", new ProcessResult("1:", "", 0, false));
			ExpectToExecuteWithArgumentsAndReturn("log -r tip --template {rev}", new ProcessResult("1", "", 0, false));

			Modification[] result = hg.GetModifications(IntegrationResult(from), IntegrationResult(to));
			Assert.That(result, Is.EqualTo(new Modification[0]));
		}

		#endregion

		#region GetSource() Workflow Tests

		[Test]
		public void ShouldCheckForMultipleHeadsAndGetSource()
		{
			hg.MultipleHeadsFail = true;

			ExpectToExecuteWithArgumentsAndReturn("heads --template {rev}:", new ProcessResult("1:", "", 0, false));
			ExpectToExecuteArguments("update", tempWorkDir);

			hg.GetSource(IntegrationResult());
		}

		[Test]
		public void ShouldNotGetSourceIfSpecified()
		{
			hg.AutoGetSource = false;

			hg.GetSource(IntegrationResult());

			mockProcessExecutor.VerifyNoOtherCalls();
		}

		[Test]
		public void ShouldGetSourceIfModificationsFound()
		{
			ExpectToExecuteArguments("update");

			hg.GetSource(IntegrationResult());
		}

		[Test]
		public void ShouldPurgeModificationsIfSpecified()
		{
			hg.PurgeModifications = true;

			ExpectToExecuteArguments(@"purge --all");
			ExpectToExecuteArguments(@"update");

			hg.GetSource(IntegrationResult());
		}

		[Test]
		public void ShouldRevertModificationsIfSpecified()
		{
			hg.RevertModifications = true;

			ExpectToExecuteArguments(@"revert --all --no-backup");
			ExpectToExecuteArguments(@"update");

			hg.GetSource(IntegrationResult());
		}

		[Test]
		public void ShouldThrowMultipleHeadsExceptionWhenMultipleHeadsAreFound()
		{
			hg.MultipleHeadsFail = true;
			Modification[] heads = new Modification[2];

			ExpectToExecuteWithArgumentsAndReturn("heads --template {rev}:", new ProcessResult("1:2:", "", 0, false));

			Assert.That(delegate { hg.GetSource(IntegrationResult()); },
			            Throws.TypeOf<MultipleHeadsFoundException>());
		}

		[Test]
		public void ShouldUseBranchNameWhenGettingSourceIfSpecified()
		{
			hg.Branch = "branch";

			ExpectToExecuteArguments("update branch");

			hg.GetSource(IntegrationResult());
		}

		#endregion

		#region LabelSourceControl() Workflow Tests

		[Test]
		public void ShouldApplyLabelIfTagOnSuccessTrue()
		{
			hg.TagOnSuccess = true;

			ExpectToExecuteArguments(@"tag -m ""Tagging CC.NET build foo"" -u CruiseControl.NET -f ccnet_build_foo", tempWorkDir);

			hg.LabelSourceControl(IntegrationResultMother.CreateSuccessful("foo"));
		}

		[Test]
		public void ShouldCommitModificationsIfSpecified()
		{
			hg.CommitModifications = true;

			ExpectToExecuteArguments(@"commit -u CruiseControl.NET -m ""Modifications of CC.NET build foo""", tempWorkDir);

			hg.LabelSourceControl(IntegrationResultMother.CreateSuccessful("foo"));
		}

		[Test]
		public void ShouldCommitModificationsAnduntrackedFilesIfSpecified()
		{
			hg.CommitModifications = true;
			hg.CommitUntracked = true;

			ExpectToExecuteArguments(@"commit -A -u CruiseControl.NET -m ""Modifications of CC.NET build foo""", tempWorkDir);

			hg.LabelSourceControl(IntegrationResultMother.CreateSuccessful("foo"));
		}

		[Test]
		public void ShouldApplyLabelWithCustomMessageIfTagOnSuccessTrueAndACustomMessageIsSpecified()
		{
			hg.TagOnSuccess = true;
			hg.TagCommitMessage = "a---- {0} ----a";

			ExpectToExecuteArguments(@"tag -m ""a---- foo ----a"" -u CruiseControl.NET -f ccnet_build_foo", tempWorkDir);

			hg.LabelSourceControl(IntegrationResultMother.CreateSuccessful("foo"));
		}

		[Test]
		public void ShouldNotApplyLabelIfIntegrationFailed()
		{
			hg.TagOnSuccess = true;

			hg.LabelSourceControl(IntegrationResultMother.CreateFailed());

			mockProcessExecutor.VerifyNoOtherCalls();
		}

		[Test]
		public void ShouldNotApplyLabelIfTagOnSuccessFalse()
		{
			hg.TagOnSuccess = false;

			hg.LabelSourceControl(IntegrationResultMother.CreateSuccessful());

			mockProcessExecutor.VerifyNoOtherCalls();
		}

		[Test]
		public void ShouldPushTagCommitToRepoIfSpecified()
		{
			hg.TagOnSuccess = true;
			hg.PushModifications = true;
			hg.Repository = @"c:\myrepo\";

			ExpectToExecuteArguments(@"tag -m ""Tagging CC.NET build foo"" -u CruiseControl.NET -f ccnet_build_foo");
			ExpectToExecuteArguments(@"push -f c:\myrepo\");

			hg.LabelSourceControl(IntegrationResultMother.CreateSuccessful("foo"));
		}

		[Test]
		public void ShouldPushModificationsToRemoteRepoIfSpecified()
		{
			hg.CommitModifications = true;
			hg.PushModifications = true;
			hg.Repository = @"c:\myrepo\";

			ExpectToExecuteArguments(@"commit -u CruiseControl.NET -m ""Modifications of CC.NET build foo""", tempWorkDir);
			ExpectToExecuteArguments(@"push -f c:\myrepo\");

			hg.LabelSourceControl(IntegrationResultMother.CreateSuccessful("foo"));
		}

		[Test]
		public void ShouldPushModificationsInBranchToRemoteRepoIfSpecified()
		{
			hg.CommitModifications = true;
			hg.PushModifications = true;
			hg.Repository = @"c:\myrepo\";
			hg.Branch = "trunk";

			ExpectToExecuteArguments(@"commit -u CruiseControl.NET -m ""Modifications of CC.NET build foo""", tempWorkDir);
			ExpectToExecuteArguments(@"push -b trunk -f c:\myrepo\");

			hg.LabelSourceControl(IntegrationResultMother.CreateSuccessful("foo"));
		}

		[Test]
		public void ShouldPushModificationsIncludingUntrackedToRemoteRepoIfSpecified()
		{
			hg.CommitModifications = true;
			hg.CommitUntracked = true;
			hg.PushModifications = true;
			hg.Repository = @"c:\myrepo\";

			ExpectToExecuteArguments(@"commit -A -u CruiseControl.NET -m ""Modifications of CC.NET build foo""", tempWorkDir);
			ExpectToExecuteArguments(@"push -f c:\myrepo\");

			hg.LabelSourceControl(IntegrationResultMother.CreateSuccessful("foo"));
		}

		#endregion

		#region Helper Methods

		private void ExpectToExecuteWithArgumentsAndReturn(string args, ProcessResult returnValue)
		{
			mockProcessExecutor.Setup(executor => executor.Execute(NewProcessInfo(args, tempWorkDir))).Returns(returnValue).Verifiable();
		}

		private new void ExpectToExecuteArguments(string args)
		{
			ExpectToExecuteArguments(args, tempWorkDir);
		}

		#endregion
	}
}
