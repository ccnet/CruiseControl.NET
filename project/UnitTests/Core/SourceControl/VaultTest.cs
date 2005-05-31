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
	public class VaultTest : CustomAssertion
	{
		private Vault vault;
		private DynamicMock mockExecutor;
		private DynamicMock mockHistoryParser;
		private IntegrationResult result;

		[SetUp]
		public void SetUp()
		{
			mockExecutor = new DynamicMock(typeof (ProcessExecutor));
			mockHistoryParser = new DynamicMock(typeof(IHistoryParser));
			vault = new Vault((IHistoryParser) mockHistoryParser.MockInstance, (ProcessExecutor) mockExecutor.MockInstance);

			result = IntegrationResultMother.CreateSuccessful("foo");
			result.WorkingDirectory = "c:\\dev";
		}

		[Test]
		public void ValuesShouldBeSetFromConfigurationXml()
		{
			const string ST_XML_SSL = @"<sourceControl type=""vault"">
				<executable>d:\program files\sourcegear\vault client\vault.exe</executable>
				<username>username</username>
				<password>password</password>
				<host>host</host>
				<repository>repository</repository>
				<folder>$\foo</folder>
				<ssl>True</ssl>
				<autoGetSource>True</autoGetSource>
				<applyLabel>True</applyLabel>
			</sourceControl>";

			vault = CreateVault(ST_XML_SSL);
			Assert.AreEqual(@"d:\program files\sourcegear\vault client\vault.exe", vault.Executable);
			Assert.AreEqual("username", vault.Username);
			Assert.AreEqual("password", vault.Password);
			Assert.AreEqual("host", vault.Host);
			Assert.AreEqual("repository", vault.Repository);
			Assert.AreEqual("$\\foo", vault.Folder);
			Assert.AreEqual(true, vault.AutoGetSource);
			Assert.AreEqual(true, vault.ApplyLabel);
		}

		[Test]
		public void ShouldBePopulatedWithDefaultValuesWhenLoadingFromMinimalXml()
		{
			Vault vault = CreateVault(@"<sourceControl type=""vault"" />");
			Assert.AreEqual(Vault.DefaultExecutable, vault.Executable);
			Assert.AreEqual("$", vault.Folder);
			Assert.AreEqual(false, vault.AutoGetSource);
			Assert.AreEqual(false, vault.ApplyLabel);
		}

		[Test]
		public void ShouldBuildGetModificationsArgumentsCorrectly()
		{
			string args = @"history ""$"" -host ""host"" -user ""username"" -password ""password"" -repository ""repository"" -rowlimit 0 -ssl";
			mockExecutor.ExpectAndReturn("Execute", ProcessResultFixture.CreateSuccessfulResult(), ProcessInfoWithArg(args));
			mockHistoryParser.ExpectAndReturn("Parse", new Modification[0], new IsAnything(), new IsAnything(), new IsAnything());

			vault.Host = "host";
			vault.Username = "username";
			vault.Password = "password";
			vault.Repository = "repository";
			vault.Ssl = true;
			vault.GetModifications(result, IntegrationResultMother.CreateSuccessful());
			mockExecutor.Verify();
			mockHistoryParser.Verify();
		}

		[Test]
		public void ShouldNotIncludeUnspecifiedArgumentsWhenGettingModifications()
		{
			mockExecutor.ExpectAndReturn("Execute", ProcessResultFixture.CreateSuccessfulResult(), ProcessInfoWithArg(@"history ""$"" -rowlimit 0"));
			mockHistoryParser.ExpectAndReturn("Parse", new Modification[0], new IsAnything(), new IsAnything(), new IsAnything());
			vault.GetModifications(result, IntegrationResultMother.CreateSuccessful());
			mockExecutor.Verify();
			mockHistoryParser.Verify();
		}

		[Test]
		public void ShouldBuildGetSourceArgumentsCorrectly()
		{
			string args = @"get ""$"" -destpath ""c:\dev"" -merge overwrite -performdeletions removeworkingcopy -setfiletime checkin -makewritable";
			mockExecutor.ExpectAndReturn("Execute", ProcessResultFixture.CreateSuccessfulResult(), ProcessInfoWithArg(args));
			vault.AutoGetSource = true;
			vault.Folder = "$";
			vault.GetSource(result);
			mockExecutor.Verify();
		}

		[Test]
		public void ShouldNotGetSourceIfAutoGetSourceIsFalse()
		{
			mockExecutor.ExpectNoCall("Execute", typeof (ProcessInfo));
			vault.AutoGetSource = false;
			vault.GetSource(IntegrationResultMother.CreateSuccessful());
			mockExecutor.Verify();
		}

		[Test]
		public void ShouldNotApplyLabelIfApplyLabelIsFalse()
		{
			mockExecutor.ExpectNoCall("Execute", typeof (ProcessInfo));
			vault.ApplyLabel = false;
			vault.LabelSourceControl(IntegrationResultMother.CreateSuccessful());
			mockExecutor.Verify();
		}

		[Test]
		public void ShouldBuildApplyLabelArgumentsCorrectly()
		{
			mockExecutor.ExpectAndReturn("Execute", ProcessResultFixture.CreateSuccessfulResult(), ProcessInfoWithArg(@"label ""$"" ""foo"""));
			vault.ApplyLabel = true;
			vault.Folder = "$";
			vault.LabelSourceControl(result);
			mockExecutor.Verify();
		}

		private ProcessInfo ProcessInfoWithArg(string args)
		{
			return new ProcessInfo(Vault.DefaultExecutable, args, "c:\\dev");
		}

		private Vault CreateVault(string xml)
		{
			Vault vault = new Vault();
			NetReflector.Read(xml, vault);
			return vault;
		}
	}
}