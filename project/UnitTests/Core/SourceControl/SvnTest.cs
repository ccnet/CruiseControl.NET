using System;
using Exortech.NetReflector;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Test
{
	[TestFixture]
	public class SvnTest : CustomAssertion
	{
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
			DynamicMock executor = new DynamicMock(typeof(ProcessExecutor));
			Svn svn = new Svn((ProcessExecutor) executor.MockInstance);

			svn.TagOnSuccess = true;
			executor.ExpectAndReturn("Execute", new ProcessResult("foo", null, 0, false), new IsAnything());
			svn.LabelSourceControl("foo", new IntegrationResult());
			executor.Verify();
		}

		[Test]
		public void ShouldNotApplyLabelIfTagOnSuccessFalse()
		{
			DynamicMock executor = new DynamicMock(typeof(ProcessExecutor));
			Svn svn = new Svn((ProcessExecutor) executor.MockInstance);

			svn.TagOnSuccess = false;
			executor.ExpectNoCall("Execute", typeof(ProcessInfo));
			svn.LabelSourceControl("foo", new IntegrationResult());
			executor.Verify();
		}

		[Test]
		public void CreatingLabelProcessPerformsCopyFromWorkingDirectoryWhenIntegrationResultDoesNotContainRepositoryRevision()
		{
			DynamicMock mockIntegrationResult = new DynamicMock(typeof(IIntegrationResult));
			mockIntegrationResult.ExpectAndReturn("LastChangeNumber", 0);

			Svn svn = CreateSvn(CreateSourceControlXml("svn://someserver/"));
			svn.TagBaseUrl = "svn://someserver/tags";
			ProcessInfo actualProcess = svn.CreateLabelProcessInfo("foo", (IIntegrationResult) mockIntegrationResult.MockInstance);

			string expectedOutput = @"copy -m ""CCNET build foo"" ""c:\dev\src"" svn://someserver/tags/foo --non-interactive";
			Assert.AreEqual(expectedOutput, actualProcess.Arguments);

			mockIntegrationResult.Verify();
		}


		[Test]
		public void CreatingLabelProcessPerformsServerToServerCopyWithRevisionWhenKnown()
		{
			DynamicMock mockIntegrationResult = new DynamicMock(typeof(IIntegrationResult));
			mockIntegrationResult.ExpectAndReturn("LastChangeNumber", 5);

			Svn svn = CreateSvn(CreateSourceControlXml("svn://someserver/"));
			svn.TagBaseUrl = "svn://someserver/tags";
			ProcessInfo actualProcess = svn.CreateLabelProcessInfo("foo", (IIntegrationResult) mockIntegrationResult.MockInstance);

			string expectedOutput = @"copy -m ""CCNET build foo"" ""svn://someserver/"" svn://someserver/tags/foo --non-interactive --revision 5";
			Assert.AreEqual(expectedOutput, actualProcess.Arguments);

			mockIntegrationResult.Verify();
		}


		[Test]
		public void CreatingLabelProcessIncludesCorrectlyFormattedArgumentsForUsernameAndPassword()
		{
			DynamicMock mockIntegrationResult = new DynamicMock(typeof(IIntegrationResult));
			mockIntegrationResult.SetupResult("LastChangeNumber", 0);

			Svn svn = CreateSvn(CreateSourceControlXml("svn://someserver/", "user", "password"));
			svn.TagBaseUrl = "svn://someserver/tags";
			ProcessInfo actualProcess = svn.CreateLabelProcessInfo("foo", (IIntegrationResult) mockIntegrationResult.MockInstance);

			string expectedOutput = @"copy -m ""CCNET build foo"" ""c:\dev\src"" svn://someserver/tags/foo --non-interactive --username ""user"" --password ""password""";
			Assert.AreEqual(expectedOutput, actualProcess.Arguments);

			mockIntegrationResult.Verify();
		}

		[Test]
		public void ShouldGetSourceWithAppropriateRevisionNumberIfTagOnSuccessTrueAndModificationsFound()
		{
			DynamicMock executor = new DynamicMock(typeof(ProcessExecutor));
			Svn svn = new Svn((ProcessExecutor) executor.MockInstance);
			svn.WorkingDirectory = "myWorkingDirectory";
			IntegrationResult result = new IntegrationResult();
			Modification mod = new Modification();
			mod.ChangeNumber = 10;
			result.Modifications = new Modification[] { mod };
			ProcessInfo expectedProcessRequest = new ProcessInfo("svn.exe", "update --non-interactive -r10", "myWorkingDirectory");

			svn.AutoGetSource = true;
			executor.ExpectAndReturn("Execute", new ProcessResult("foo", null, 0, false), expectedProcessRequest);
			svn.GetSource(result);
			executor.Verify();
		}

		// This would happen, e.g., for force build
		[Test]
		public void ShouldGetSourceWithoutRevisionNumberIfTagOnSuccessTrueAndModificationsNotFound()
		{
			DynamicMock executor = new DynamicMock(typeof(ProcessExecutor));
			Svn svn = new Svn((ProcessExecutor) executor.MockInstance);
			svn.WorkingDirectory = "myWorkingDirectory";
			IntegrationResult result = new IntegrationResult();
			ProcessInfo expectedProcessRequest = new ProcessInfo("svn.exe", "update --non-interactive", "myWorkingDirectory");

			svn.AutoGetSource = true;
			executor.ExpectAndReturn("Execute", new ProcessResult("foo", null, 0, false), expectedProcessRequest);
			svn.GetSource(result);
			executor.Verify();
		}

		[Test]
		public void ShouldGetSourceWithCredentialsIfSpecifiedIfAutoGetSourceTrue()
		{
			DynamicMock executor = new DynamicMock(typeof(ProcessExecutor));
			Svn svn = new Svn((ProcessExecutor) executor.MockInstance);
			svn.WorkingDirectory = "myWorkingDirectory";
			svn.Username = "user";
			svn.Password = "password";
			IntegrationResult result = new IntegrationResult();
			ProcessInfo expectedProcessRequest = new ProcessInfo("svn.exe", @"update --non-interactive --username ""user"" --password ""password""", "myWorkingDirectory");

			svn.AutoGetSource = true;
			executor.ExpectAndReturn("Execute", new ProcessResult("foo", null, 0, false), expectedProcessRequest);
			svn.GetSource(result);
			executor.Verify();
		}

		[Test]
		public void ShouldNotGetSourceIfAutoGetSourceFalse()
		{
			DynamicMock executor = new DynamicMock(typeof(ProcessExecutor));
			Svn svn = new Svn((ProcessExecutor) executor.MockInstance);
			IntegrationResult result = new IntegrationResult();

			svn.AutoGetSource = false;
			executor.ExpectNoCall("Execute", typeof(ProcessInfo));
			svn.GetSource(result);
			executor.Verify();
		}

		private Svn CreateSvn(string xml)
		{
			Svn svn = new Svn();
			NetReflector.Read(xml, svn);
			return svn;
		}
	} 
}
