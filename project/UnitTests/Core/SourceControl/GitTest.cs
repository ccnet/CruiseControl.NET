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

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	[TestFixture]
	public class GitTest : ProcessExecutorTestFixtureBase
	{
		const string GIT_CLONE = "clone xyz.git";
		const string GIT_FETCH = "fetch origin";
		const string GIT_REMOTE_HASH = "log origin/master --date-order -1 --pretty=format:\"%H\"";
		const string GIT_LOCAL_HASH = "log --date-order -1 --pretty=format:\"%H\"";
        readonly string GIT_REMOTE_COMMITS = string.Format(
            "log origin/master --date-order --name-status \"--after=Sun, 21 Jan 2001 {0:HH:mm:ss} GMT\" \"--before=Mon, 22 Jan 2001 {0:HH:mm:ss} GMT\" --pretty=format:\"Commit:%H%nTime:%ci%nAuthor:%an%nE-Mail:%ae%nMessage:%s%n%n%b%nChanges:\"",
            new DateTime(2001, 1, 22, 18, 0, 0, DateTimeKind.Utc).ToLocalTime());

		private Git git;
		private IMock mockHistoryParser;
		private DateTime from;
		private DateTime to;
		private IMock mockFileSystem;
		private IMock mockFileDirectoryDeleter;

		[SetUp]
		protected void CreateGit()
		{
			mockHistoryParser = new DynamicMock(typeof(IHistoryParser));
			mockFileSystem = new DynamicMock(typeof(IFileSystem));
			mockFileDirectoryDeleter = new DynamicMock(typeof(IFileDirectoryDeleter));
			CreateProcessExecutorMock("git");
			from = new DateTime(2001, 1, 21, 20, 0, 0, DateTimeKind.Local);
			to = from.AddDays(1);
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
	<autoGetSource>true</autoGetSource>
</git>";

			git = (Git)NetReflector.Read(xml);
			Assert.AreEqual(@"git", git.Executable, "#B1");
			Assert.AreEqual(@"c:\git\ccnet\mygitrepo", git.Repository, "#B2");
			Assert.AreEqual(@"master", git.Branch, "#B3");
			Assert.AreEqual(new Timeout(5), git.Timeout, "#B4");
			Assert.AreEqual(@"c:\git\working", git.WorkingDirectory, "#B5");
			Assert.AreEqual(true, git.TagOnSuccess, "#B6");
			Assert.AreEqual(true, git.AutoGetSource, "#B7");
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
		}

		[Test]
		public void ShouldApplyLabelIfTagOnSuccessTrue()
		{
			git.TagOnSuccess = true;

			ExpectToExecuteArguments(@"tag -a -m ""CCNET build foo"" foo");
			ExpectToExecuteArguments(@"push origin tag foo");

			git.LabelSourceControl(IntegrationResultMother.CreateSuccessful("foo"));
		}

		[Test]
		public void ShouldApplyLabelWithCustomMessageIfTagOnSuccessTrueAndACustomMessageIsSpecified()
		{
			git.TagOnSuccess = true;
			git.TagCommitMessage = "a---- {0} ----a";

			ExpectToExecuteArguments(@"tag -a -m ""a---- foo ----a"" foo");
			ExpectToExecuteArguments(@"push origin tag foo");

			git.LabelSourceControl(IntegrationResultMother.CreateSuccessful("foo"));
		}

		[Test]
		public void ShouldCloneIfDirectoryDoesntExist()
		{
			mockFileSystem.ExpectAndReturn("DirectoryExists", false, DefaultWorkingDirectory);

			ExpectToExecuteArguments(string.Concat(GIT_CLONE, " ", StringUtil.AutoDoubleQuoteString(DefaultWorkingDirectory)), Path.GetDirectoryName(DefaultWorkingDirectory.TrimEnd(Path.DirectorySeparatorChar)));

			ExpectToExecuteArguments("log origin/master --date-order -1 --pretty=format:\"%H\"");

			ExpectToExecuteArguments(GIT_REMOTE_COMMITS);

			git.GetModifications(IntegrationResult(from), IntegrationResult(to));
		}

		[Test]
		[Ignore("Does not correctly work")]
		public void ShouldCloneAndDeleteWorkingDirIfGitDirectoryDoesntExist()
		{
			mockFileSystem.ExpectAndReturn("DirectoryExists", true, DefaultWorkingDirectory);
			mockFileSystem.ExpectAndReturn("DirectoryExists", false, Path.Combine(DefaultWorkingDirectory, ".git"));

			ExpectToExecuteArguments(string.Concat(GIT_CLONE, " ", StringUtil.AutoDoubleQuoteString(DefaultWorkingDirectory)), Path.GetDirectoryName(DefaultWorkingDirectory.TrimEnd(Path.DirectorySeparatorChar)));

			ExpectToExecuteArguments("log origin/master --date-order -1 --pretty=format:\"%H\"");

			ExpectToExecuteArguments(GIT_REMOTE_COMMITS);

			git.GetModifications(IntegrationResult(from), IntegrationResult(to));
		}

		[Test]
		public void ShouldNotGetModificationsWhenHashsMatch()
		{
			mockFileSystem.ExpectAndReturn("DirectoryExists", true, DefaultWorkingDirectory);
			mockFileSystem.ExpectAndReturn("DirectoryExists", true, Path.Combine(DefaultWorkingDirectory, ".git"));

			ExpectToExecuteArguments(GIT_FETCH);

			ExpectToExecuteWithArgumentsAndReturn(GIT_REMOTE_HASH, new ProcessResult("abcdef", "", 0, false));
			ExpectToExecuteWithArgumentsAndReturn(GIT_LOCAL_HASH, new ProcessResult("abcdef", "", 0, false));

			Modification[] mods = git.GetModifications(IntegrationResult(from), IntegrationResult(to));

			Assert.AreEqual(0, mods.Length);
		}

		private void ExpectToExecuteWithArgumentsAndReturn(string args, ProcessResult returnValue)
		{
			mockProcessExecutor.ExpectAndReturn("Execute", returnValue, NewProcessInfo(args, DefaultWorkingDirectory));
		}

		[Test]
		public void ShouldGetSourceIfModificationsFound()
		{
			git.AutoGetSource = true;

			ExpectToExecuteArguments("checkout -f origin/master");
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

			ExpectToExecuteWithArgumentsAndReturn(GIT_REMOTE_HASH, new ProcessResult("abcdef", "", 0, false));
			ExpectToExecuteWithArgumentsAndReturn(GIT_LOCAL_HASH, new ProcessResult("ghijkl", "", 0, false));

			ExpectToExecuteArguments(GIT_REMOTE_COMMITS);

			mockHistoryParser.ExpectAndReturn("Parse", modifications, new IsAnything(), from, new IsAnything());

			Modification[] result = git.GetModifications(IntegrationResult(from), IntegrationResult(to));
			Assert.AreEqual(modifications, result);
		}

		private void SetupGit(IFileSystem filesystem, IFileDirectoryDeleter fileDirectoryDeleter)
		{
			git = new Git((IHistoryParser)mockHistoryParser.MockInstance, (ProcessExecutor)mockProcessExecutor.MockInstance, filesystem, fileDirectoryDeleter);
			git.Repository = @"xyz.git";
			git.WorkingDirectory = DefaultWorkingDirectory;
		}
	}
}
