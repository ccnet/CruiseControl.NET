using System;
using Exortech.NetReflector;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	[TestFixture]
	public class SvnTest : ProcessExecutorTestFixtureBase
	{
		private Svn svn;

		[SetUp]
		protected void SetUp()
		{
			CreateProcessExecutorMock(Svn.DefaultExecutable);
			svn = new Svn((ProcessExecutor) mockProcessExecutor.MockInstance);
			svn.TagBaseUrl = "svn://someserver/tags/foo";
			svn.WorkingDirectory = DefaultWorkingDirectory;
		}

		[TearDown]
		protected void TearDown()
		{
			Verify();
		}

		private string CreateSourceControlXml(string trunkUrl)
		{
			return CreateSourceControlXml(trunkUrl, null, null);
		}

		private string CreateSourceControlXml(string trunkUrl, string username, string password)
		{
			string usernameAndPassword = (username == null) ? string.Empty : string.Format("<username>{0}</username><password>{1}</password>", username, password);
			return string.Format(
				@"
<sourceControl type=""svn"">
	<executable>..\tools\subversion-0.37.0\svn.exe</executable>
	<trunkUrl>{0}</trunkUrl>
	<timeout>5</timeout>
	<workingDirectory>c:\dev\src</workingDirectory>
	{1}
</sourceControl>"
				, trunkUrl, usernameAndPassword);
		}

		[Test]
		public void DefaultPropertyPopulationFromXml()
		{
			Svn svn = CreateSvn(CreateSourceControlXml("svn://myserver/mypath"));
			Assert.AreEqual("..\\tools\\subversion-0.37.0\\svn.exe", svn.Executable);
			Assert.AreEqual("svn://myserver/mypath", svn.TrunkUrl);
			Assert.AreEqual(5, svn.Timeout);
			Assert.AreEqual(@"c:\dev\src", svn.WorkingDirectory);
		}

		[Test]
		public void UserAndPasswordPropertyPopulationFromXml()
		{
			Svn svn = CreateSvn(CreateSourceControlXml("svn://myserver/mypath", "user", "password"));
			Assert.AreEqual("user", svn.Username);
			Assert.AreEqual("password", svn.Password);
		}

		[Test]
		public void CreatingHistoryProcessIncludesCorrectlyFormattedArguments()
		{
			Svn svn = CreateSvn(CreateSourceControlXml("svn://someserver/"));
			DateTime from = DateTime.Parse("2001-01-21  20:00:00 'GMT'");
			DateTime to = DateTime.Parse("2001-01-21  20:30:50 'GMT'");
			ProcessInfo actualProcess = svn.CreateHistoryProcessInfo(from, to);

			Assert.AreEqual("log -v -r \"{2001-01-21T20:00:00Z}:{2001-01-21T20:30:50Z}\" --xml --non-interactive svn://someserver/", actualProcess.Arguments);
		}

		[Test]
		public void CreatingHistoryProcessIncludesCorrectlyFormattedArgumentsForUsernameAndPassword()
		{
			Svn svn = CreateSvn(CreateSourceControlXml("svn://someserver/", "user", "password"));
			DateTime from = DateTime.Parse("2001-01-21  20:00:00 'GMT'");
			DateTime to = DateTime.Parse("2001-01-21  20:30:50 'GMT'");
			ProcessInfo actualProcess = svn.CreateHistoryProcessInfo(from, to);

			string expectedOutput = @"log -v -r ""{2001-01-21T20:00:00Z}:{2001-01-21T20:30:50Z}"" --xml --non-interactive svn://someserver/ --username ""user"" --password ""password""";
			Assert.AreEqual(expectedOutput, actualProcess.Arguments);
		}

		[Test]
		public void ShouldApplyLabelIfTagOnSuccessTrue()
		{
			ExpectToExecuteArguments(@"copy -m ""CCNET build foo"" ""c:\source"" svn://someserver/tags/foo/foo --non-interactive");
			svn.TagOnSuccess = true;
			svn.LabelSourceControl(IntegrationResultMother.CreateSuccessful("foo"));
		}

		[Test]
		public void ShouldNotApplyLabelIfTagOnSuccessFalse()
		{
			ExpectThatExecuteWillNotBeCalled();
			svn.TagOnSuccess = false;
			svn.LabelSourceControl(IntegrationResultMother.CreateSuccessful());
		}

		[Test]
		public void ShouldNotApplyLabelIfIntegrationFailed()
		{
			ExpectThatExecuteWillNotBeCalled();
			svn.TagOnSuccess = true;
			svn.LabelSourceControl(IntegrationResultMother.CreateFailed());
		}

		[Test]
		public void CreatingLabelProcessPerformsCopyFromWorkingDirectoryWhenIntegrationResultDoesNotContainRepositoryRevision()
		{
			DynamicMock mockIntegrationResult = new DynamicMock(typeof (IIntegrationResult));
			mockIntegrationResult.ExpectAndReturn("LastChangeNumber", 0);
			mockIntegrationResult.ExpectAndReturn("Label", "foo");

			Svn svn = CreateSvn(CreateSourceControlXml("svn://someserver/"));
			svn.TagBaseUrl = "svn://someserver/tags";
			ProcessInfo actualProcess = svn.NewLabelProcessInfo((IIntegrationResult) mockIntegrationResult.MockInstance);

			string expectedOutput = @"copy -m ""CCNET build foo"" ""c:\dev\src"" svn://someserver/tags/foo --non-interactive";
			Assert.AreEqual(expectedOutput, actualProcess.Arguments);

			mockIntegrationResult.Verify();
		}

		[Test]
		public void CreatingLabelProcessPerformsServerToServerCopyWithRevisionWhenKnown()
		{
			DynamicMock mockIntegrationResult = new DynamicMock(typeof (IIntegrationResult));
			mockIntegrationResult.ExpectAndReturn("LastChangeNumber", 5);
			mockIntegrationResult.ExpectAndReturn("Label", "foo");

			Svn svn = CreateSvn(CreateSourceControlXml("svn://someserver/"));
			svn.TagBaseUrl = "svn://someserver/tags";
			ProcessInfo actualProcess = svn.NewLabelProcessInfo((IIntegrationResult) mockIntegrationResult.MockInstance);

			string expectedOutput = @"copy -m ""CCNET build foo"" ""svn://someserver/"" svn://someserver/tags/foo --non-interactive --revision 5";
			Assert.AreEqual(expectedOutput, actualProcess.Arguments);

			mockIntegrationResult.Verify();
		}

		[Test]
		public void CreatingLabelProcessIncludesCorrectlyFormattedArgumentsForUsernameAndPassword()
		{
			DynamicMock mockIntegrationResult = new DynamicMock(typeof (IIntegrationResult));
			mockIntegrationResult.SetupResult("LastChangeNumber", 0);
			mockIntegrationResult.ExpectAndReturn("Label", "foo");

			Svn svn = CreateSvn(CreateSourceControlXml("svn://someserver/", "user", "password"));
			svn.TagBaseUrl = "svn://someserver/tags";
			ProcessInfo actualProcess = svn.NewLabelProcessInfo((IIntegrationResult) mockIntegrationResult.MockInstance);

			string expectedOutput = @"copy -m ""CCNET build foo"" ""c:\dev\src"" svn://someserver/tags/foo --non-interactive --username ""user"" --password ""password""";
			Assert.AreEqual(expectedOutput, actualProcess.Arguments);

			mockIntegrationResult.Verify();
		}

		[Test]
		public void ShouldGetSourceWithAppropriateRevisionNumberIfTagOnSuccessTrueAndModificationsFound()
		{
			ExpectToExecuteArguments("update --non-interactive -r10");

			IntegrationResult result = new IntegrationResult();
			Modification mod = new Modification();
			mod.ChangeNumber = 10;
			result.Modifications = new Modification[] {mod};

			svn.AutoGetSource = true;
			svn.GetSource(result);
		}

		// This would happen, e.g., for force build
		[Test]
		public void ShouldGetSourceWithoutRevisionNumberIfTagOnSuccessTrueAndModificationsNotFound()
		{
			ExpectToExecuteArguments("update --non-interactive");
			svn.AutoGetSource = true;
			svn.GetSource(new IntegrationResult());
		}

		[Test]
		public void ShouldGetSourceWithCredentialsIfSpecifiedIfAutoGetSourceTrue()
		{
			ExpectToExecuteArguments(@"update --non-interactive --username ""user"" --password ""password""");
			svn.Username = "user";
			svn.Password = "password";
			svn.AutoGetSource = true;
			svn.GetSource(new IntegrationResult());
		}

		[Test]
		public void ShouldNotGetSourceIfAutoGetSourceFalse()
		{
			ExpectThatExecuteWillNotBeCalled();
			svn.AutoGetSource = false;
			svn.GetSource(new IntegrationResult());
		}

		private Svn CreateSvn(string xml)
		{
			Svn svn = new Svn();
			NetReflector.Read(xml, svn);
			return svn;
		}
	}
}