using System;
using System.IO;
using Exortech.NetReflector;
using NMock;
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
		private IMock mockParser;
		private DateTime from;
		private DateTime to;
		private DynamicMock mockFileSystem;

		[SetUp]
		protected void SetUp()
		{
			from = DateTime.Parse("2001-01-21  20:00:00 'GMT'");
			to = DateTime.Parse("2001-01-21  20:30:50 'GMT'");
			CreateProcessExecutorMock(Svn.DefaultExecutable);
			mockParser = new DynamicMock(typeof(IHistoryParser));
			mockFileSystem = new DynamicMock(typeof (IFileSystem));
			svn = new Svn((ProcessExecutor) mockProcessExecutor.MockInstance, (IHistoryParser) mockParser.MockInstance, (IFileSystem) mockFileSystem.MockInstance);
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
	<tagBaseUrl>svn://myserver/mypath/tags</tagBaseUrl>
	<autoGetSource>true</autoGetSource>
</svn>";

			Svn svn = (Svn) NetReflector.Read(xml);
			Assert.AreEqual(@"c:\svn\svn.exe", svn.Executable);
			Assert.AreEqual("svn://myserver/mypath", svn.TrunkUrl);
			Assert.AreEqual(new Timeout(5), svn.Timeout);
			Assert.AreEqual(@"c:\dev\src", svn.WorkingDirectory);
			Assert.AreEqual("user", svn.Username);
			Assert.AreEqual("password", svn.Password);
			Assert.AreEqual(true, svn.TagOnSuccess);
			Assert.AreEqual(true, svn.AutoGetSource);
			Assert.AreEqual("svn://myserver/mypath/tags", svn.TagBaseUrl);
		}

		[Test]
		public void SpecifyFromMinimallySpecifiedXml()
		{
			string xml = @"<svn/>";
			Svn svn = (Svn) NetReflector.Read(xml);
			Assert.AreEqual("svn.exe", svn.Executable);			
		}

		[Test]
		public void CreatingHistoryProcessIncludesCorrectlyFormattedArguments()
		{
			ExpectToExecuteArguments("log svn://myserver/mypath -r \"{2001-01-21T20:00:00Z}:{2001-01-21T20:30:50Z}\" --verbose --xml --non-interactive --no-auth-cache");
			svn.GetModifications(IntegrationResult(from), IntegrationResult(to));
		}

		[Test]
		public void CreatingHistoryProcessIncludesCorrectlyFormattedArgumentsForUsernameAndPassword()
		{
			string args = @"log svn://myserver/mypath -r ""{2001-01-21T20:00:00Z}:{2001-01-21T20:30:50Z}"" --verbose --xml --username user --password password --non-interactive --no-auth-cache";
			ExpectToExecuteArguments(args);
			svn.Username = "user";
			svn.Password = "password";
			svn.GetModifications(IntegrationResult(from), IntegrationResult(to));
		}

		[Test]
		public void CreatingHistoryProcessShouldQuoteTrunkUrl()
		{
			ExpectToExecuteArguments("log \"svn://my server/mypath\" -r \"{2001-01-21T20:00:00Z}:{2001-01-21T20:30:50Z}\" --verbose --xml --non-interactive --no-auth-cache");
			svn.TrunkUrl = "svn://my server/mypath";
			svn.GetModifications(IntegrationResult(from), IntegrationResult(to));
		}

		[Test]
		public void CreatingHistoryProcessShouldHandleImplicitTrunkUrl()
		{
			ExpectToExecuteArguments("log -r \"{2001-01-21T20:00:00Z}:{2001-01-21T20:30:50Z}\" --verbose --xml --non-interactive --no-auth-cache");
			svn.TrunkUrl = null;
			svn.GetModifications(IntegrationResult(from), IntegrationResult(to));
		}

		[Test]
		public void ShouldRebaseWorkingDirectoryForHistory()
		{
			ExpectToExecuteArguments("log svn://myserver/mypath -r \"{2001-01-21T20:00:00Z}:{2001-01-21T20:30:50Z}\" --verbose --xml --non-interactive --no-auth-cache");
			svn.WorkingDirectory = @"source\";
			IIntegrationResult result = IntegrationResult(to);
			result.WorkingDirectory = @"c:\";
			svn.GetModifications(IntegrationResult(from), result);
		}

		[Test]
		public void ShouldApplyLabelIfTagOnSuccessTrue()
		{
			ExpectToExecuteArguments(@"copy -m ""CCNET build foo"" c:\source svn://someserver/tags/foo/foo --non-interactive --no-auth-cache");
			svn.TagOnSuccess = true;
			svn.LabelSourceControl(IntegrationResultMother.CreateSuccessful("foo"));
		}

		[Test]
		public void ShouldApplyLabelUsingRebasedWorkingDirectory()
		{
			ExpectToExecuteArguments(@"copy -m ""CCNET build foo"" c:\source svn://someserver/tags/foo/foo --non-interactive --no-auth-cache");
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
			Modification mod = new Modification();
			mod.ChangeNumber = 5;
			result.Modifications = new Modification[] { mod };
			ExpectToExecuteArguments(@"copy -m ""CCNET build foo"" svn://myserver/mypath svn://someserver/tags/foo --revision 5 --non-interactive --no-auth-cache");

			svn.TagOnSuccess = true;
			svn.TagBaseUrl = "svn://someserver/tags";
			svn.LabelSourceControl(result);
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
		public void CreatingLabelProcessIncludesCorrectlyFormattedArgumentsForUsernameAndPassword()
		{
			string args = @"copy -m ""CCNET build foo"" c:\source svn://someserver/tags/foo --username user --password password --non-interactive --no-auth-cache";
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
			ExpectSvnDirectoryExists(true);
			ExpectToExecuteArguments("update --revision 10 --non-interactive --no-auth-cache");

			IIntegrationResult result = IntegrationResult();
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
			ExpectSvnDirectoryExists(true);
			ExpectToExecuteArguments("update --non-interactive --no-auth-cache");
			svn.AutoGetSource = true;
			svn.GetSource(IntegrationResult());
		}

		[Test]
		public void ShouldGetSourceWithCredentialsIfSpecifiedIfAutoGetSourceTrue()
		{
			ExpectSvnDirectoryExists(true);
			ExpectToExecuteArguments(@"update --username ""Buck Rogers"" --password ""My Password"" --non-interactive --no-auth-cache");
			svn.Username = "Buck Rogers";
			svn.Password = "My Password";
			svn.AutoGetSource = true;
			svn.GetSource(IntegrationResult());
		}

		[Test]
		public void ShouldNotGetSourceIfAutoGetSourceFalse()
		{
			ExpectThatExecuteWillNotBeCalled();
			svn.AutoGetSource = false;
			svn.GetSource(IntegrationResult());
		}
		
		[Test]
		public void ShouldCheckoutInsteadOfUpdateIfSVNFoldersDoNotExist()
		{
			ExpectToExecuteArguments(@"checkout svn://myserver/mypath --non-interactive --no-auth-cache");
			ExpectSvnDirectoryExists(false);

			svn.AutoGetSource = true;
			svn.WorkingDirectory = DefaultWorkingDirectory;
			svn.GetSource(IntegrationResult());
		}

		[Test, ExpectedException(typeof(ConfigurationException))]
		public void ShouldThrowExceptionIfTrunkUrlIsNotSpecifiedAndSVNFoldersDoNotExist()
		{
			ExpectSvnDirectoryExists(false);
			
			svn.TrunkUrl = string.Empty;
			svn.AutoGetSource = true;
			svn.WorkingDirectory = DefaultWorkingDirectory;
			svn.GetSource(IntegrationResult());
		}

		private void ExpectSvnDirectoryExists(bool doesSvnDirectoryExist)
		{
			mockFileSystem.ExpectAndReturn("DirectoryExists", doesSvnDirectoryExist, Path.Combine(DefaultWorkingDirectory, ".svn"));
		}
	}
}
