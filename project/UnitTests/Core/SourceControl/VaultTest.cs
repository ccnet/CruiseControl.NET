using System;
using Exortech.NetReflector;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	[TestFixture]
	public class VaultTest : ProcessExecutorTestFixtureBase
	{
		private Vault vault;
		private DynamicMock mockHistoryParser;
		private IntegrationResult result;

		[SetUp]
		public void SetUp()
		{
			CreateProcessExecutorMock(Vault.DefaultExecutable);
			mockHistoryParser = new DynamicMock(typeof (IHistoryParser));
			vault = new Vault((IHistoryParser) mockHistoryParser.MockInstance, (ProcessExecutor) mockProcessExecutor.MockInstance);

			result = IntegrationResultMother.CreateSuccessful("foo");
			result.WorkingDirectory = DefaultWorkingDirectory;
		}

		[Test]
		public void ValuesShouldBeSetFromConfigurationXml()
		{
			const string ST_XML_SSL = @"<vault>
				<executable>d:\program files\sourcegear\vault client\vault.exe</executable>
				<username>username</username>
				<password>password</password>
				<host>host</host>
				<repository>repository</repository>
				<folder>$\foo</folder>
				<ssl>True</ssl>
				<autoGetSource>True</autoGetSource>
				<applyLabel>True</applyLabel>
				<historyArgs></historyArgs>
				<useWorkingDirectory>false</useWorkingDirectory>
				<workingDirectory>c:\source\</workingDirectory>
			</vault>";

			vault = CreateVault(ST_XML_SSL);
			Assert.AreEqual(@"d:\program files\sourcegear\vault client\vault.exe", vault.Executable);
			Assert.AreEqual("username", vault.Username);
			Assert.AreEqual("password", vault.Password);
			Assert.AreEqual("host", vault.Host);
			Assert.AreEqual("repository", vault.Repository);
			Assert.AreEqual("$\\foo", vault.Folder);
			Assert.AreEqual(true, vault.AutoGetSource);
			Assert.AreEqual(true, vault.ApplyLabel);
			Assert.AreEqual(false, vault.UseVaultWorkingDirectory);
			Assert.AreEqual(string.Empty, vault.HistoryArgs);
			Assert.AreEqual(@"c:\source\", vault.WorkingDirectory);
		}

		[Test]
		public void ShouldBePopulatedWithDefaultValuesWhenLoadingFromMinimalXml()
		{
			Vault vault = CreateVault(@"<vault />");
			Assert.AreEqual(Vault.DefaultExecutable, vault.Executable);
			Assert.AreEqual("$", vault.Folder);
			Assert.AreEqual(false, vault.AutoGetSource);
			Assert.AreEqual(false, vault.ApplyLabel);
			Assert.AreEqual(true, vault.UseVaultWorkingDirectory);
			Assert.AreEqual(Vault.DefaultHistoryArgs, vault.HistoryArgs);
		}

		[Test]
		public void ShouldBuildGetModificationsArgumentsCorrectly()
		{
			DateTime today = DateTime.Now;
			DateTime yesterday = today.AddDays(-1);
			result.StartTime = yesterday;
			string args = string.Format(@"history $ -excludeactions label -rowlimit 0 -begindate {0:s} -enddate {1:s}{2}",
			                            yesterday, today, CommonOptionalArguments());
			ExpectToExecuteArguments(args);
			ExpectToParseHistory();

			SetHostUsernamePasswordRepositoryAndSsl();
			vault.GetModifications(result, IntegrationResultMother.CreateSuccessful(today));
			VerifyAll();
		}

		[Test]
		public void ShouldNotIncludeUnspecifiedArgumentsWhenGettingModifications()
		{
			DateTime today = DateTime.Now;
			DateTime yesterday = today.AddDays(-1);
			result.StartTime = yesterday;
			string args = string.Format(@"history $ -excludeactions label -rowlimit 0 -begindate {0:s} -enddate {1:s}", yesterday, today);
			ExpectToExecuteArguments(args);
			ExpectToParseHistory();

			vault.GetModifications(result, IntegrationResultMother.CreateSuccessful());
			VerifyAll();
		}

		[Test]
		public void ShouldBuildGetSourceArgumentsCorrectlyUsingWorkingDirectory()
		{
			ExpectToExecuteArguments(@"get $ -merge overwrite -performdeletions removeworkingcopy -setfiletime checkin -makewritable");
			vault.AutoGetSource = true;
			vault.UseVaultWorkingDirectory = true;
			vault.Folder = "$";
			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public void ShouldBuildGetSourceArgumentsCorrectlyNotUsingWorkingDirectory()
		{
			ExpectToExecuteArguments(@"get $ -destpath c:\source\ -merge overwrite -setfiletime checkin -makewritable");
			vault.AutoGetSource = true;
			vault.UseVaultWorkingDirectory = false;
			vault.Folder = "$";
			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public void ShouldBuildGetSourceWithOptionalArgumentsIncluded()
		{
			ExpectToExecuteArguments(@"get $ -merge overwrite -performdeletions removeworkingcopy -setfiletime checkin -makewritable" + CommonOptionalArguments());
			vault.AutoGetSource = true;
			vault.Folder = "$";
			SetHostUsernamePasswordRepositoryAndSsl();
			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public void ShouldNotGetSourceIfAutoGetSourceIsFalse()
		{
			ExpectThatExecuteWillNotBeCalled();
			vault.AutoGetSource = false;
			vault.GetSource(IntegrationResultMother.CreateSuccessful());
			VerifyAll();
		}

		[Test]
		public void ShouldNotApplyLabelIfApplyLabelIsFalse()
		{
			ExpectThatExecuteWillNotBeCalled();
			vault.ApplyLabel = false;
			vault.LabelSourceControl(IntegrationResultMother.CreateSuccessful());
			VerifyAll();
		}

		[Test]
		public void ShouldBuildApplyLabelArgumentsCorrectlyNonAutoGet()
		{
			ExpectToExecuteArguments(@"label $ foo");
			vault.ApplyLabel = true;
			vault.AutoGetSource = false;
			vault.Folder = "$";
			vault.LabelSourceControl(result);
			VerifyAll();
		}

		[Test]
		public void ShouldBuildApplyLabelArgumentsCorrectlyAutoGet()
		{
			ExpectToExecuteArguments(@"label $ foo");
			ExpectToExecuteArguments(@"getlabel $ foo -destpath c:\source\ -merge overwrite -setfiletime checkin -makewritable");
			vault.ApplyLabel = true;
			vault.UseVaultWorkingDirectory = false;
			vault.AutoGetSource = true;
			vault.Folder = "$";
			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public void ShouldSetAndRemoveLabelOnFailure()
		{
			ExpectToExecuteArguments(@"label $ foo");
			ExpectToExecuteArguments(@"getlabel $ foo -destpath c:\source\ -merge overwrite -setfiletime checkin -makewritable");
			ExpectToExecuteArguments(@"deletelabel $ foo");
			vault.ApplyLabel = true;
			vault.UseVaultWorkingDirectory = false;
			vault.AutoGetSource = true;
			vault.Folder = "$";
			vault.GetSource(result);
			IntegrationResult failed = IntegrationResultMother.CreateFailed();
			failed.Label = result.Label;
			failed.WorkingDirectory = result.WorkingDirectory;
			vault.LabelSourceControl(failed);
			VerifyAll();
		}

		[Test]
		public void ShouldSetAndLeaveLabelOnSuccess()
		{
			ExpectToExecuteArguments(@"label $ foo");
			ExpectToExecuteArguments(@"getlabel $ foo -destpath c:\source\ -merge overwrite -setfiletime checkin -makewritable");
			vault.ApplyLabel = true;
			vault.UseVaultWorkingDirectory = false;
			vault.AutoGetSource = true;
			vault.Folder = "$";
			vault.GetSource(result);
			IntegrationResult failed = IntegrationResultMother.CreateSuccessful();
			failed.Label = result.Label;
			failed.WorkingDirectory = result.WorkingDirectory;
			vault.LabelSourceControl(failed);
			VerifyAll();
		}

		[Test]
		public void ShouldBuildRemoveLabelArgumentsCorrectlyOnFailureWithAutoGet()
		{
			ExpectToExecuteArguments(@"deletelabel $ foo");
			vault.ApplyLabel = true;
			vault.AutoGetSource = true;
			vault.Folder = "$";
			IntegrationResult failed = IntegrationResultMother.CreateFailed();
			failed.Label = result.Label;
			failed.WorkingDirectory = result.WorkingDirectory;
			vault.LabelSourceControl(failed);
			VerifyAll();
		}

		[Test]
		public void ShouldBuildLabelArgumentsCorrectlyOnFailureNonAutoget()
		{
			ExpectToExecuteArguments(@"label $ foo");
			vault.ApplyLabel = true;
			vault.AutoGetSource = false;
			vault.Folder = "$";
			IntegrationResult failed = IntegrationResultMother.CreateFailed();
			failed.Label = result.Label;
			failed.WorkingDirectory = result.WorkingDirectory;
			vault.LabelSourceControl(failed);
			VerifyAll();
		}

		[Test]
		public void ShouldBuildApplyLabelArgumentsIncludingCommonOptionalArguments()
		{
			ExpectToExecuteArguments(@"label $ foo" + CommonOptionalArguments());
			vault.ApplyLabel = true;
			vault.Folder = "$";
			SetHostUsernamePasswordRepositoryAndSsl();
			vault.LabelSourceControl(result);
			VerifyAll();
		}

		private string CommonOptionalArguments()
		{
			return @" -host ""my host"" -user username -password password -repository ""my repository"" -ssl";
		}

		private void SetHostUsernamePasswordRepositoryAndSsl()
		{
			vault.Host = "my host";
			vault.Username = "username";
			vault.Password = "password";
			vault.Repository = "my repository";
			vault.Ssl = true;
		}

		private Vault CreateVault(string xml)
		{
			return (Vault) NetReflector.Read(xml);
		}

		private void ExpectToParseHistory()
		{
			mockHistoryParser.ExpectAndReturn("Parse", new Modification[0], new IsAnything(), new IsAnything(), new IsAnything());
		}

		private void VerifyAll()
		{
			Verify();
			mockHistoryParser.Verify();
		}
	}
}