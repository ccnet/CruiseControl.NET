using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Core.Util;
using System.IO;
using NMock;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.UnitTests.Core;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using NMock.Constraints;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	[TestFixture]
	public class GitTest : ProcessExecutorTestFixtureBase
	{
		const string GIT_CLONE = "clone xyz.git";
		const string GIT_FETCH = "fetch origin";
		const string GIT_REMOTE_HASH = "log origin/master -1 --pretty=format:\"%H\"";
		const string GIT_LOCAL_HASH = "log -1 --pretty=format:\"%H\"";
		const string GIT_COMMIT_KEY = "commit";
		const string FROM_COMMIT = "0123456789abcdef";
		const string TO_COMMIT = "fedcba9876543210";
		const string GIT_LOG_OPTIONS = "-100 --name-status --pretty=format:\"Commit:%H%nTime:%ci%nAuthor:%an%nE-Mail:%ae%nMessage:%s%n%n%b%nChanges:\" -m";
		const string GIT_LOG_REMOTE_COMMITS = "log " + FROM_COMMIT + "..origin/master " + GIT_LOG_OPTIONS;
		const string GIT_LOG_ALL = "log origin/master " + GIT_LOG_OPTIONS;

		private Git git;
		private IMock mockHistoryParser;
		private IMock mockFileSystem;
		private IMock mockFileDirectoryDeleter;

		[SetUp]
		protected void CreateGit()
		{
			mockHistoryParser = new DynamicMock(typeof(IHistoryParser));
			mockFileSystem = new DynamicMock(typeof(IFileSystem));
			mockFileDirectoryDeleter = new DynamicMock(typeof(IFileDirectoryDeleter));
			CreateProcessExecutorMock("git");

			SetupGit((IFileSystem)mockFileSystem.MockInstance, (IFileDirectoryDeleter)mockFileDirectoryDeleter.MockInstance);
		}

		[TearDown]
		protected void VerifyAll()
		{
			Verify();
			mockHistoryParser.Verify();
			mockFileSystem.Verify();
		}

		[Test]
		public void GitShouldBeDefaultExecutable()
		{
			Assert.AreEqual("git", git.Executable, "#A1");
		}

		[Test]
		public void PopulateFromFullySpecifiedXml()
		{
			const string xml = @"
<git>
	<executable>git</executable>
	<repository>c:\git\ccnet\mygitrepo</repository>
	<branch>master</branch>
	<timeout>5</timeout>
	<workingDirectory>c:\git\working</workingDirectory>
	<tagOnSuccess>true</tagOnSuccess>
	<commitBuildModifications>true</commitBuildModifications>
	<commitUntrackedFiles>true</commitUntrackedFiles>
    <maxAmountOfModificationsToFetch>500</maxAmountOfModificationsToFetch>
	<autoGetSource>true</autoGetSource>
	<tagCommitMessage>CCNet Test Build {0}</tagCommitMessage>
	<tagNameFormat>{0}</tagNameFormat>
	<committerName>Max Mustermann</committerName>
	<committerEMail>max.mustermann@gmx.de</committerEMail>
</git>";

			git = (Git)NetReflector.Read(xml);
			Assert.AreEqual("git", git.Executable, "#B1");
			Assert.AreEqual(@"c:\git\ccnet\mygitrepo", git.Repository, "#B2");
			Assert.AreEqual("master", git.Branch, "#B3");
			Assert.AreEqual(new Timeout(5), git.Timeout, "#B4");
			Assert.AreEqual(@"c:\git\working", git.WorkingDirectory, "#B5");
			Assert.AreEqual(true, git.TagOnSuccess, "#B6");
			Assert.AreEqual(true, git.AutoGetSource, "#B7");
			Assert.AreEqual("CCNet Test Build {0}", git.TagCommitMessage, "#B8");
			Assert.AreEqual("{0}", git.TagNameFormat, "#B9");
			Assert.AreEqual("Max Mustermann", git.CommitterName, "#B10");
			Assert.AreEqual("max.mustermann@gmx.de", git.CommitterEMail, "#B11");
			Assert.AreEqual(true, git.CommitBuildModifications, "#B12");
			Assert.AreEqual(true, git.CommitUntrackedFiles, "#B13");
            Assert.AreEqual(500, git.MaxAmountOfModificationsToFetch,  "#B14");

		}

		[Test]
		public void PopulateFromMinimallySpecifiedXml()
		{
			const string xml = @"
<git>
    <repository>c:\git\ccnet\mygitrepo</repository>
</git>";
			git = (Git)NetReflector.Read(xml);
			Assert.AreEqual(@"git", git.Executable, "#C1");
			Assert.AreEqual(@"c:\git\ccnet\mygitrepo", git.Repository, "#C2");
			Assert.AreEqual(@"master", git.Branch, "#C3");
			Assert.AreEqual(new Timeout(600000), git.Timeout, "#C4");
			Assert.AreEqual(null, git.WorkingDirectory, "#C5");
			Assert.AreEqual(false, git.TagOnSuccess, "#C6");
			Assert.AreEqual(true, git.AutoGetSource, "#C7");
			Assert.AreEqual("CCNet Build {0}", git.TagCommitMessage, "#C8");
			Assert.AreEqual("CCNet-Build-{0}", git.TagNameFormat, "#C9");
			Assert.AreEqual(null, git.CommitterName, "#C10");
			Assert.AreEqual(null, git.CommitterEMail, "#C11");
			Assert.AreEqual(false, git.CommitBuildModifications, "#C12");
			Assert.AreEqual(false, git.CommitUntrackedFiles, "#C13");
            Assert.AreEqual(100, git.MaxAmountOfModificationsToFetch, "#C14");
		}

		[Test]
		public void ShouldApplyLabelIfTagOnSuccessTrue()
		{
			git.TagOnSuccess = true;

			ExpectToExecuteArguments(@"tag -a -m ""CCNet Build foo"" CCNet-Build-foo");
			ExpectToExecuteArguments(@"push origin tag CCNet-Build-foo");

			git.LabelSourceControl(IntegrationResultMother.CreateSuccessful("foo"));
		}

		[Test]
		public void ShouldCommitBuildModificationsAndApplyLabelIfCommitBuildModificationsAndTagOnSuccessIsTrue()
		{
			git.TagOnSuccess = true;
			git.CommitBuildModifications = true;

			ExpectToExecuteArguments(@"commit --all --allow-empty -m ""CCNet Build foo""");
			ExpectToExecuteArguments(@"tag -a -m ""CCNet Build foo"" CCNet-Build-foo");
			ExpectToExecuteArguments(@"push origin tag CCNet-Build-foo");

			git.LabelSourceControl(IntegrationResultMother.CreateSuccessful("foo"));
		}

		[Test]
		public void ShouldAddAndCommitBuildModificationsAndApplyLabelIfCommitUntrackedFilesAndCommitBuildModificationsAndTagOnSuccessIsTrue()
		{
			git.TagOnSuccess = true;
			git.CommitBuildModifications = true;
			git.CommitUntrackedFiles = true;

			ExpectToExecuteArguments(@"add --all");
			ExpectToExecuteArguments(@"commit --all --allow-empty -m ""CCNet Build foo""");
			ExpectToExecuteArguments(@"tag -a -m ""CCNet Build foo"" CCNet-Build-foo");
			ExpectToExecuteArguments(@"push origin tag CCNet-Build-foo");

			git.LabelSourceControl(IntegrationResultMother.CreateSuccessful("foo"));
		}

		[Test]
		public void ShouldApplyLabelIfTagOnSuccessTrueAndNotAddFilesIfCommitBuildModificationsIsFalseAndCommitUntrackedFilesIsTrue()
		{
			git.TagOnSuccess = true;
			git.CommitBuildModifications = false;
			git.CommitUntrackedFiles = true;

			ExpectToExecuteArguments(@"tag -a -m ""CCNet Build foo"" CCNet-Build-foo");
			ExpectToExecuteArguments(@"push origin tag CCNet-Build-foo");

			git.LabelSourceControl(IntegrationResultMother.CreateSuccessful("foo"));
		}

		[Test]
		public void ShouldApplyLabelWithCustomMessageIfTagOnSuccessTrueAndACustomMessageIsSpecified()
		{
			git.TagOnSuccess = true;
			git.TagCommitMessage = "a---- {0} ----a";

			ExpectToExecuteArguments(@"tag -a -m ""a---- foo ----a"" CCNet-Build-foo");
			ExpectToExecuteArguments(@"push origin tag CCNet-Build-foo");

			git.LabelSourceControl(IntegrationResultMother.CreateSuccessful("foo"));
		}

		[Test]
		public void ShouldApplyTagNameFormatWithCustomFormatIfTagOnSuccessTrueAndATagNameFormatIsSpecified()
		{
			git.TagOnSuccess = true;
			git.TagNameFormat = "Build/{0}";

			ExpectToExecuteArguments(@"tag -a -m ""CCNet Build foo"" Build/foo");
			ExpectToExecuteArguments(@"push origin tag Build/foo");

			git.LabelSourceControl(IntegrationResultMother.CreateSuccessful("foo"));
		}

        [Test]
        public void ShouldApplyTagNameFormatWithJustBuildLabelAsCustomFormatTagOnSuccessTrue()
        {
            git.TagOnSuccess = true;
            git.TagNameFormat = "{0}";
            git.TagCommitMessage = "{0}";

            ExpectToExecuteArguments(@"tag -a -m foo foo");
            ExpectToExecuteArguments(@"push origin tag foo");

            git.LabelSourceControl(IntegrationResultMother.CreateSuccessful("foo"));
        }



		[Test]
		public void ShouldCloneIfDirectoryDoesNotExist()
		{
			mockFileSystem.ExpectAndReturn("DirectoryExists", false, DefaultWorkingDirectory);
			ExpectCloneAndInitialiseRepository();

			ExpectToExecuteArguments(GIT_LOG_REMOTE_COMMITS);
			ExpectLogRemoteHead(TO_COMMIT);
			mockHistoryParser.ExpectAndReturn("Parse", new Modification[] { }, new IsAnything(), new IsAnything(), new IsAnything());

			IIntegrationResult to = IntegrationResult();
			git.GetModifications(IntegrationResult(FROM_COMMIT), to);

			AssertIntegrationResultTaggedWithCommit(to, TO_COMMIT);
		}

		[Test]
		public void ShouldCloneAndDeleteWorkingDirIfGitDirectoryDoesNotExist()
		{
			mockFileSystem.ExpectAndReturn("DirectoryExists", true, DefaultWorkingDirectory);
			mockFileSystem.ExpectAndReturn("DirectoryExists", false, Path.Combine(DefaultWorkingDirectory, ".git"));
			mockFileDirectoryDeleter.Expect("DeleteIncludingReadOnlyObjects", DefaultWorkingDirectory);
			mockFileSystem.ExpectAndReturn("DirectoryExists", false, DefaultWorkingDirectory);
			ExpectCloneAndInitialiseRepository();

			ExpectToExecuteArguments(GIT_LOG_REMOTE_COMMITS);
			ExpectLogRemoteHead(TO_COMMIT);
			mockHistoryParser.ExpectAndReturn("Parse", new Modification[] { }, new IsAnything(), new IsAnything(), new IsAnything());

			IIntegrationResult to = IntegrationResult();
			git.GetModifications(IntegrationResult(FROM_COMMIT), to);

			AssertIntegrationResultTaggedWithCommit(to, TO_COMMIT);
		}

		[Test]
		public void ShouldLogWholeHistoryIfCommitNotPresentInFromIntegrationResult()
		{
			mockFileSystem.ExpectAndReturn("DirectoryExists", true, DefaultWorkingDirectory);
			mockFileSystem.ExpectAndReturn("DirectoryExists", true, Path.Combine(DefaultWorkingDirectory, ".git"));

			ExpectToExecuteArguments(GIT_FETCH);
			ExpectToExecuteArguments(GIT_LOG_ALL);
			ExpectLogRemoteHead(TO_COMMIT);

			IIntegrationResult to = IntegrationResult();
			git.GetModifications(IntegrationResult(), to);

			AssertIntegrationResultTaggedWithCommit(to, TO_COMMIT);
		}

		private void ExpectToExecuteWithArgumentsAndReturn(string args, ProcessResult returnValue)
		{
			var processInfo = NewProcessInfo(args, DefaultWorkingDirectory);
			processInfo.StandardInputContent = "";
			mockProcessExecutor.ExpectAndReturn("Execute", returnValue, processInfo);
		}

		private new void ExpectToExecuteArguments(string args)
		{
			ExpectToExecuteArguments(args, DefaultWorkingDirectory);
		}

		protected new void ExpectToExecuteArguments(string args, string workingDirectory)
		{
			ProcessInfo processInfo = NewProcessInfo(args, workingDirectory);
			processInfo.StandardInputContent = "";
			ExpectToExecute(processInfo);
		}

		[Test]
		public void ShouldGetSourceIfModificationsFound()
		{
			git.AutoGetSource = true;

			ExpectToExecuteArguments("checkout -q -f origin/master");
			ExpectToExecuteArguments("clean -d -f -x");

			git.GetSource(IntegrationResult());
		}

		[Test]
		public void ShouldNotApplyLabelIfIntegrationFailed()
		{
			git.TagOnSuccess = true;

			ExpectThatExecuteWillNotBeCalled();

			git.LabelSourceControl(IntegrationResultMother.CreateFailed());
		}

		[Test]
		public void ShouldNotApplyLabelIfTagOnSuccessFalse()
		{
			git.TagOnSuccess = false;

			ExpectThatExecuteWillNotBeCalled();

			git.LabelSourceControl(IntegrationResultMother.CreateSuccessful());
		}

		[Test]
		public void ShouldNotGetSourceIfAutoGetSourceFalse()
		{
			git.AutoGetSource = false;

			ExpectThatExecuteWillNotBeCalled();

			git.GetSource(IntegrationResult());
		}

		[Test]
		public void ShouldReturnModificationsWhenHashsDifferent()
		{
			mockFileSystem.ExpectAndReturn("DirectoryExists", true, DefaultWorkingDirectory);
			mockFileSystem.ExpectAndReturn("DirectoryExists", true, Path.Combine(DefaultWorkingDirectory, ".git"));

			Modification[] modifications = new Modification[2] { new Modification(), new Modification() };

			ExpectToExecuteArguments(GIT_FETCH);
			ExpectToExecuteArguments(GIT_LOG_REMOTE_COMMITS);
			ExpectLogRemoteHead(TO_COMMIT);

			mockHistoryParser.ExpectAndReturn("Parse", modifications, new IsAnything(), new IsAnything(), new IsAnything());

			IIntegrationResult to = IntegrationResult();
			Modification[] result = git.GetModifications(IntegrationResult(FROM_COMMIT), to);

			Assert.AreEqual(modifications, result);
			AssertIntegrationResultTaggedWithCommit(to, TO_COMMIT);
		}

		private void SetupGit(IFileSystem filesystem, IFileDirectoryDeleter fileDirectoryDeleter)
		{
			git = new Git((IHistoryParser)mockHistoryParser.MockInstance, (ProcessExecutor)mockProcessExecutor.MockInstance, filesystem, fileDirectoryDeleter);
			git.Repository = @"xyz.git";
			git.WorkingDirectory = DefaultWorkingDirectory;
		}

		private IIntegrationResult IntegrationResult(string commit)
		{
			IntegrationResult r = new IntegrationResult();
			r.SourceControlData.Add(new NameValuePair(GIT_COMMIT_KEY, commit));
			return r;
		}

		/// <summary>
		/// Sets an expectation that git will call 'log' to get the remote head commit, printing the value of
		/// <paramref name="commit"/> to stdout.
		/// </summary>
		/// <param name="commit"></param>
		private void ExpectLogRemoteHead(string commit)
		{
			ExpectToExecuteWithArgumentsAndReturn(GIT_REMOTE_HASH, new ProcessResult(commit, "", 0, false));
		}

		private void ExpectCloneAndInitialiseRepository()
		{
			ExpectToExecuteArguments(string.Concat(GIT_CLONE, " ", StringUtil.AutoDoubleQuoteString(DefaultWorkingDirectory)), Path.GetDirectoryName(DefaultWorkingDirectory.TrimEnd(Path.DirectorySeparatorChar)));
			ExpectToExecuteArguments("config --get user.name");
			ExpectToExecuteArguments("config --get user.email");
		}

		private void AssertIntegrationResultTaggedWithCommit(IIntegrationResult result, string commit)
		{
			Dictionary<string, string> data = NameValuePair.ToDictionary(result.SourceControlData);
			Assert.That(data.ContainsKey(GIT_COMMIT_KEY), "IntegrationResult.SourceControlData did not contain commit info.");
			Assert.That(data[GIT_COMMIT_KEY], Is.EqualTo(commit));
		}
	}
}
