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
		private IMock mockParser;

		[SetUp]
		protected void SetUp()
		{
			CreateProcessExecutorMock(Svn.DefaultExecutable);
			mockParser = new DynamicMock(typeof(IHistoryParser));
			svn = new Svn((ProcessExecutor) mockProcessExecutor.MockInstance, (IHistoryParser) mockParser.MockInstance);
			svn.TrunkUrl = "svn://myserver/mypath";
			svn.TagBaseUrl = "svn://someserver/tags/foo";
			svn.WorkingDirectory = DefaultWorkingDirectory;
		}

		[TearDown]
		protected void TearDown()
		{
			Verify();
			mockParser.Verify();
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
<svn>
	<executable>..\tools\subversion-0.37.0\svn.exe</executable>
	<trunkUrl>{0}</trunkUrl>
	<timeout>5</timeout>
	<workingDirectory>c:\dev\src</workingDirectory>
	{1}
</svn>"
				, trunkUrl, usernameAndPassword);
		}

		[Test]
		public void DefaultPropertyPopulationFromXml()
		{
			Svn svn = (Svn) NetReflector.Read(CreateSourceControlXml("svn://myserver/mypath"));
			Assert.AreEqual("..\\tools\\subversion-0.37.0\\svn.exe", svn.Executable);
			Assert.AreEqual("svn://myserver/mypath", svn.TrunkUrl);
			Assert.AreEqual(5, svn.Timeout);
			Assert.AreEqual(@"c:\dev\src", svn.WorkingDirectory);
		}

		[Test]
		public void UserAndPasswordPropertyPopulationFromXml()
		{
			Svn svn = (Svn) NetReflector.Read(CreateSourceControlXml("svn://myserver/mypath", "user", "password"));
			Assert.AreEqual("user", svn.Username);
			Assert.AreEqual("password", svn.Password);
		}

		[Test]
		public void CreatingHistoryProcessIncludesCorrectlyFormattedArguments()
		{
			ExpectToExecuteArguments("log svn://myserver/mypath -r \"{2001-01-21T20:00:00Z}:{2001-01-21T20:30:50Z}\" --verbose --xml --non-interactive");
			DateTime from = DateTime.Parse("2001-01-21  20:00:00 'GMT'");
			DateTime to = DateTime.Parse("2001-01-21  20:30:50 'GMT'");
			svn.GetModifications(IntegrationResult(from), IntegrationResult(to));
		}

		[Test]
		public void CreatingHistoryProcessIncludesCorrectlyFormattedArgumentsForUsernameAndPassword()
		{
			string args = @"log svn://myserver/mypath -r ""{2001-01-21T20:00:00Z}:{2001-01-21T20:30:50Z}"" --verbose --xml --username user --password password --non-interactive";
			ExpectToExecuteArguments(args);

			DateTime from = DateTime.Parse("2001-01-21  20:00:00 'GMT'");
			DateTime to = DateTime.Parse("2001-01-21  20:30:50 'GMT'");
			svn.Username = "user";
			svn.Password = "password";
			svn.GetModifications(IntegrationResult(from), IntegrationResult(to));
		}

		[Test]
		public void ShouldApplyLabelIfTagOnSuccessTrue()
		{
			ExpectToExecuteArguments(@"copy -m ""CCNET build foo"" c:\source svn://someserver/tags/foo/foo --non-interactive");
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

			svn.TagBaseUrl = "svn://someserver/tags";
			ProcessInfo actualProcess = svn.NewLabelProcessInfo((IIntegrationResult) mockIntegrationResult.MockInstance);

			string expectedOutput = @"copy -m ""CCNET build foo"" c:\source svn://someserver/tags/foo --non-interactive";
			Assert.AreEqual(expectedOutput, actualProcess.Arguments);

			mockIntegrationResult.Verify();
		}

		[Test]
		public void CreatingLabelProcessPerformsServerToServerCopyWithRevisionWhenKnown()
		{
			DynamicMock mockIntegrationResult = new DynamicMock(typeof (IIntegrationResult));
			mockIntegrationResult.ExpectAndReturn("LastChangeNumber", 5);
			mockIntegrationResult.ExpectAndReturn("Label", "foo");

			svn.TagBaseUrl = "svn://someserver/tags";
			ProcessInfo actualProcess = svn.NewLabelProcessInfo((IIntegrationResult) mockIntegrationResult.MockInstance);

			string expectedOutput = @"copy -m ""CCNET build foo"" svn://myserver/mypath svn://someserver/tags/foo --revision 5 --non-interactive";
			Assert.AreEqual(expectedOutput, actualProcess.Arguments);

			mockIntegrationResult.Verify();
		}

		[Test]
		public void CreatingLabelProcessIncludesCorrectlyFormattedArgumentsForUsernameAndPassword()
		{
			string args = @"copy -m ""CCNET build foo"" c:\source svn://someserver/tags/foo --username user --password password --non-interactive";
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
		public void ShouldGetSourceWithAppropriateRevisionNumberIfTagOnSuccessTrueAndModificationsFound()
		{
			ExpectToExecuteArguments("update --revision 10 --non-interactive");

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
			ExpectToExecuteArguments(@"update --username ""Buck Rogers"" --password ""My Password"" --non-interactive");
			svn.Username = "Buck Rogers";
			svn.Password = "My Password";
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
	}
}