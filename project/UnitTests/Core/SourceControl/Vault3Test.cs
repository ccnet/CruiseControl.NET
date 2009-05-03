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
	public class Vault3Test : ProcessExecutorTestFixtureBase
	{
		protected readonly string listFolderOutputWithWorkingFolderSet = @"
			<vault>
				<listworkingfolders>
						<workingfolder reposfolder=""$"" localfolder=""{0}"" />
				</listworkingfolders>
				<result success=""yes"" />
			</vault>";
		protected readonly string listFolderOutputWithNonXml = @"
			Some junk to be removed
			<vault>
				<listworkingfolders>
						<workingfolder reposfolder=""$"" localfolder=""{0}"" />
				</listworkingfolders>
				<result success=""yes"" />
			</vault>";
		protected readonly string listFolderOutputWithNoWorkingFolderSet = @"
			<vault>
				<listworkingfolders />
				<result success=""yes"" />
			</vault>";

		protected VaultVersionChecker vault;
		protected DynamicMock mockHistoryParser;
		protected IntegrationResult result;

		/* 
		* CleanFolder tests commented out because tests that are tied to a
		* particular file system layout are undesirable, per Owen 11/29/05
		* 
		private string tempFileToTestCleanCopy = null;
		private bool expectCleanCopy = false;
		*/

		[SetUp]
		public virtual void SetUp()
		{
			CreateProcessExecutorMock(VaultVersionChecker.DefaultExecutable);
			mockHistoryParser = new DynamicMock(typeof (IHistoryParser));
			vault = new VaultVersionChecker((IHistoryParser) mockHistoryParser.MockInstance, (ProcessExecutor) mockProcessExecutor.MockInstance, VaultVersionChecker.EForcedVaultVersion.Vault3);

			result = IntegrationResultMother.CreateSuccessful("foo");
			result.WorkingDirectory = this.DefaultWorkingDirectory;
		}

		[Test]
		public virtual void ValuesShouldBeSetFromConfigurationXml()
		{
			const string ST_XML_SSL = @"<vault>
				<executable>d:\program files\sourcegear\vault client\vault.exe</executable>
				<username>username</username>
				<password>password</password>
				<host>localhost</host>
				<repository>repository</repository>
				<folder>$\foo</folder>
				<ssl>True</ssl>
				<autoGetSource>True</autoGetSource>
				<applyLabel>True</applyLabel>
				<useWorkingDirectory>false</useWorkingDirectory>
				<historyArgs>-blah test</historyArgs>
				<timeout>2400000</timeout>
				<workingDirectory>c:\source</workingDirectory>
				<cleanCopy>true</cleanCopy>
				<setFileTime>current</setFileTime>
				<proxyServer>proxyhost</proxyServer>
				<proxyPort>12345</proxyPort>
				<proxyUser>proxyuser</proxyUser>
				<proxyPassword>proxypassword</proxyPassword>
				<proxyDomain>proxydomain</proxyDomain>
				<pollRetryAttempts>10</pollRetryAttempts>
				<pollRetryWait>30</pollRetryWait >
			</vault>";

			vault = CreateVault(ST_XML_SSL);
			Assert.AreEqual(@"d:\program files\sourcegear\vault client\vault.exe", vault.Executable);
			Assert.AreEqual("username", vault.Username);
			Assert.AreEqual("password", vault.Password);
			Assert.AreEqual("localhost", vault.Host);
			Assert.AreEqual("repository", vault.Repository);
			Assert.AreEqual("$\\foo", vault.Folder);
			Assert.AreEqual(true, vault.Ssl);
			Assert.AreEqual(true, vault.AutoGetSource);
			Assert.AreEqual(true, vault.ApplyLabel);
			Assert.AreEqual(false, vault.UseVaultWorkingDirectory);
			Assert.AreEqual("-blah test", vault.HistoryArgs);
			Assert.AreEqual(2400000, vault.Timeout.Millis);
			Assert.AreEqual(@"c:\source", vault.WorkingDirectory);
			Assert.AreEqual(true, vault.CleanCopy);
			Assert.AreEqual("current", vault.setFileTime);
			Assert.AreEqual("proxyhost", vault.proxyServer);
			Assert.AreEqual("12345", vault.proxyPort);
			Assert.AreEqual("proxyuser", vault.proxyUser);
			Assert.AreEqual("proxypassword", vault.proxyPassword);
			Assert.AreEqual("proxydomain", vault.proxyDomain);
			Assert.AreEqual(10, vault.pollRetryAttempts);
			Assert.AreEqual(30, vault.pollRetryWait);
		}

		[Test]
		public virtual void ShouldBePopulatedWithDefaultValuesWhenLoadingFromMinimalXml()
		{
			vault = CreateVault
				(@"
				<vault>
					<host>localhost</host>
					<username>name</username>
					<password>password</password>
					<repository>repository</repository>
				</vault>
			");
			Assert.AreEqual(VaultVersionChecker.DefaultExecutable, vault.Executable);
			Assert.AreEqual("name", vault.Username);
			Assert.AreEqual("password", vault.Password);
			Assert.AreEqual("localhost", vault.Host);
			Assert.AreEqual("repository", vault.Repository);
			Assert.AreEqual("$", vault.Folder);
			Assert.AreEqual(false, vault.Ssl);
			Assert.AreEqual(true, vault.AutoGetSource);
			Assert.AreEqual(false, vault.ApplyLabel);
			Assert.AreEqual(true, vault.UseVaultWorkingDirectory);
			Assert.AreEqual(VaultVersionChecker.DefaultHistoryArgs, vault.HistoryArgs);
			Assert.AreEqual(Timeout.DefaultTimeout, vault.Timeout);
			Assert.IsNull(vault.WorkingDirectory);
			Assert.AreEqual(false, vault.CleanCopy);
			Assert.AreEqual("checkin", vault.setFileTime);
			Assert.IsNull(vault.proxyServer);
			Assert.IsNull(vault.proxyPort);
			Assert.IsNull(vault.proxyUser);
			Assert.IsNull(vault.proxyPassword);
			Assert.IsNull(vault.proxyDomain);
			Assert.AreEqual(VaultVersionChecker.DefaultPollRetryAttempts, vault.pollRetryAttempts);
			Assert.AreEqual(VaultVersionChecker.DefaultPollRetryWait, vault.pollRetryWait);
		}

		[Test]
		public virtual void ShouldBuildGetModificationsArgumentsCorrectly()
		{
			DateTime today = DateTime.Now;
			DateTime yesterday = today.AddDays(-1);
			result.StartTime = yesterday;
			string args = string.Format(@"history $ -excludeactions label,obliterate -rowlimit 0 -begindate {0:s} -enddate {1:s}{2}",
			                            yesterday, today, SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(args);
			ExpectToParseHistory();

			vault.GetModifications(result, IntegrationResultMother.CreateSuccessful(today));
			VerifyAll();
		}

		/*
		 *               Get Source Scenarios           || Correct Action
		 * ---------------------------------------------||-------------------------------------------------------------------
		 *          |       |   Use   | Working |       ||  List   |
		 *          | Apply | Working | Folder  | Clean || Working | Get Command
		 * Scenario | Label | Folder  | Spec'd  | Copy  || Folders | and Arguments
		 * ------------------------------------------------------------------------------------------------------------------
		 *    1     |   T   |    T    |    T    |   T   ||    F    | getlabel -labelworkingfolder <specified working folder>
		 *    2     |   T   |    T    |    T    |   F   ||    F    | getlabel -labelworkingfolder <specified working folder>
		 *    3     |   T   |    T    |    F    |   T   ||    T    | getlabel -labelworkingfolder <retrieved working folder>
		 *    4     |   T   |    T    |    F    |   F   ||    T    | getlabel -labelworkingfolder <retrieved working folder>
		 *    5     |   T   |    F    |    T    |   T   ||    F    | getlabel -destpath <specified working folder>
		 *    6     |   T   |    F    |    T    |   F   ||    F    | getlabel -destpath <specified working folder>
		 *    7     |   T   |    F    |    F    |   T   ||    T    | getlabel -destpath <retrieved working folder>
		 *    8     |   T   |    F    |    F    |   F   ||    T    | getlabel -destpath <retrieved working folder>
		 *    9     |   F   |    T    |    T    |   T   ||    F    | get
		 *   10     |   F   |    T    |    T    |   F   ||    F    | get
		 *   11     |   F   |    T    |    F    |   T   ||    T    | get
		 *   12     |   F   |    T    |    F    |   F   ||    F    | get
		 *   13     |   F   |    F    |    T    |   T   ||    F    | get -destpath <specified working folder>
		 *   14     |   F   |    F    |    T    |   F   ||    F    | get -destpath <specified working folder>
		 *   15     |   F   |    F    |    F    |   T   ||    T    | get -destpath <retrieved working folder>
		 *   16     |   F   |    F    |    F    |   F   ||    T    | get -destpath <retrieved working folder>
		 */

		[Test]
		public virtual void ArgumentsCorrectForGetSourceScenario1()
		{
			vault.Folder = "$";
			vault.AutoGetSource = true;

			vault.ApplyLabel = true;
			vault.UseVaultWorkingDirectory = true;
			vault.WorkingDirectory = DefaultWorkingDirectory;
			vault.CleanCopy = true;

			ExpectToCleanFolder();
			ExpectToExecuteArguments(@"label $ foo" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"getlabel $ foo -labelworkingfolder " + StringUtil.AutoDoubleQuoteString(DefaultWorkingDirectory)
				+ GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public virtual void ArgumentsCorrectForGetSourceScenario2()
		{
			vault.Folder = "$";
			vault.AutoGetSource = true;

			vault.ApplyLabel = true;
			vault.UseVaultWorkingDirectory = true;
			vault.WorkingDirectory = DefaultWorkingDirectory;
			vault.CleanCopy = false;

			ExpectToNotCleanFolder();
			ExpectToExecuteArguments(@"label $ foo" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"getlabel $ foo -labelworkingfolder " + StringUtil.AutoDoubleQuoteString(DefaultWorkingDirectory)
				+ GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public virtual void ArgumentsCorrectForGetSourceScenario3()
		{
			vault.Folder = "$";
			vault.AutoGetSource = true;

			vault.ApplyLabel = true;
			vault.UseVaultWorkingDirectory = true;
			vault.WorkingDirectory = null;
			vault.CleanCopy = true;

			this.ProcessResultOutput = string.Format(listFolderOutputWithWorkingFolderSet, DefaultWorkingDirectory);
			ExpectToExecuteArguments(@"listworkingfolders" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"label $ foo" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"getlabel $ foo -labelworkingfolder " + StringUtil.AutoDoubleQuoteString(DefaultWorkingDirectory)
				+ GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public virtual void ArgumentsCorrectForGetSourceScenario4()
		{
			vault.Folder = "$";
			vault.AutoGetSource = true;

			vault.ApplyLabel = true;
			vault.UseVaultWorkingDirectory = true;
			vault.WorkingDirectory = null;
			vault.CleanCopy = false;

			this.ProcessResultOutput = string.Format(listFolderOutputWithWorkingFolderSet, DefaultWorkingDirectory);
			ExpectToExecuteArguments(@"listworkingfolders" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"label $ foo" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"getlabel $ foo -labelworkingfolder " + StringUtil.AutoDoubleQuoteString(DefaultWorkingDirectory)
				+ GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public virtual void ArgumentsCorrectForGetSourceScenario5()
		{
			vault.Folder = "$";
			vault.AutoGetSource = true;

			vault.ApplyLabel = true;
			vault.UseVaultWorkingDirectory = false;
			vault.WorkingDirectory = DefaultWorkingDirectory;
			vault.CleanCopy = true;

			this.ProcessResultOutput = "";
			ExpectToCleanFolder();
			ExpectToExecuteArguments(@"label $ foo" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"getlabel $ foo -destpath " + StringUtil.AutoDoubleQuoteString(DefaultWorkingDirectory)
				+ GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public virtual void ArgumentsCorrectForGetSourceScenario6()
		{
			vault.Folder = "$";
			vault.AutoGetSource = true;

			vault.ApplyLabel = true;
			vault.UseVaultWorkingDirectory = false;
			vault.WorkingDirectory = DefaultWorkingDirectory;
			vault.CleanCopy = false;

			this.ProcessResultOutput = "";
			ExpectToNotCleanFolder();
			ExpectToExecuteArguments(@"label $ foo" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"getlabel $ foo -destpath " + StringUtil.AutoDoubleQuoteString(DefaultWorkingDirectory)
				+ GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public virtual void ArgumentsCorrectForGetSourceScenario7()
		{
			vault.Folder = "$";
			vault.AutoGetSource = true;

			vault.ApplyLabel = true;
			vault.UseVaultWorkingDirectory = false;
			vault.WorkingDirectory = @"";
			vault.CleanCopy = true;

			this.ProcessResultOutput = string.Format(listFolderOutputWithWorkingFolderSet, DefaultWorkingDirectory);
			ExpectToExecuteArguments(@"listworkingfolders" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"label $ foo" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"getlabel $ foo -destpath " + StringUtil.AutoDoubleQuoteString(DefaultWorkingDirectory)
				+ GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public virtual void ArgumentsCorrectForGetSourceScenario8()
		{
			vault.Folder = "$";
			vault.AutoGetSource = true;

			vault.ApplyLabel = true;
			vault.UseVaultWorkingDirectory = false;
			vault.WorkingDirectory = @"";
			vault.CleanCopy = false;

			this.ProcessResultOutput = string.Format(listFolderOutputWithWorkingFolderSet, DefaultWorkingDirectory);
			ExpectToExecuteArguments(@"listworkingfolders" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"label $ foo" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"getlabel $ foo -destpath " + StringUtil.AutoDoubleQuoteString(DefaultWorkingDirectory)
				+ GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public virtual void ArgumentsCorrectForGetSourceScenario9()
		{
			vault.Folder = "$";
			vault.AutoGetSource = true;

			vault.ApplyLabel = false;
			vault.UseVaultWorkingDirectory = true;
			vault.WorkingDirectory = DefaultWorkingDirectory;
			vault.CleanCopy = true;

			this.ProcessResultOutput = "";
			ExpectToCleanFolder();
			ExpectToExecuteArguments(@"get $" + GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public virtual void ArgumentsCorrectForGetSourceScenario10()
		{
			vault.Folder = "$";
			vault.AutoGetSource = true;

			vault.ApplyLabel = false;
			vault.UseVaultWorkingDirectory = true;
			vault.WorkingDirectory = DefaultWorkingDirectory;
			vault.CleanCopy = false;

			this.ProcessResultOutput = "";
			ExpectToNotCleanFolder();
			ExpectToExecuteArguments(@"get $" + GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public virtual void ArgumentsCorrectForGetSourceScenario11()
		{
			vault.Folder = "$";
			vault.AutoGetSource = true;

			vault.ApplyLabel = false;
			vault.UseVaultWorkingDirectory = true;
			vault.WorkingDirectory = @"";
			vault.CleanCopy = true;

			this.ProcessResultOutput = string.Format(listFolderOutputWithWorkingFolderSet, DefaultWorkingDirectory);
			ExpectToExecuteArguments(@"listworkingfolders" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"get $" + GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public virtual void ArgumentsCorrectForGetSourceScenario12()
		{
			vault.Folder = "$";
			vault.AutoGetSource = true;

			vault.ApplyLabel = false;
			vault.UseVaultWorkingDirectory = true;
			vault.WorkingDirectory = @"";
			vault.CleanCopy = false;

			this.ProcessResultOutput = "";
			ExpectToExecuteArguments(@"get $" + GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public virtual void ArgumentsCorrectForGetSourceScenario13()
		{
			vault.Folder = "$";
			vault.AutoGetSource = true;

			vault.ApplyLabel = false;
			vault.UseVaultWorkingDirectory = false;
			vault.WorkingDirectory = DefaultWorkingDirectory;
			vault.CleanCopy = true;

			this.ProcessResultOutput = "";
			ExpectToCleanFolder();
			ExpectToExecuteArguments(@"get $ -destpath " + StringUtil.AutoDoubleQuoteString(DefaultWorkingDirectory) + GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public virtual void ArgumentsCorrectForGetSourceScenario14()
		{
			vault.Folder = "$";
			vault.AutoGetSource = true;

			vault.ApplyLabel = false;
			vault.UseVaultWorkingDirectory = false;
			vault.WorkingDirectory = DefaultWorkingDirectory;
			vault.CleanCopy = false;

			this.ProcessResultOutput = "";
			ExpectToNotCleanFolder();
			ExpectToExecuteArguments(@"get $ -destpath " + StringUtil.AutoDoubleQuoteString(DefaultWorkingDirectory) + GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public virtual void ArgumentsCorrectForGetSourceScenario15()
		{
			vault.Folder = "$";
			vault.AutoGetSource = true;

			vault.ApplyLabel = false;
			vault.UseVaultWorkingDirectory = false;
			vault.WorkingDirectory = @"";
			vault.CleanCopy = true;

			this.ProcessResultOutput = string.Format(listFolderOutputWithWorkingFolderSet, DefaultWorkingDirectory);
			ExpectToExecuteArguments(@"listworkingfolders" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"get $ -destpath " + StringUtil.AutoDoubleQuoteString(DefaultWorkingDirectory) + GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public virtual void ArgumentsCorrectForGetSourceScenario16()
		{
			vault.Folder = "$";
			vault.AutoGetSource = true;

			vault.ApplyLabel = false;
			vault.UseVaultWorkingDirectory = false;
			vault.WorkingDirectory = @"";
			vault.CleanCopy = false;

			this.ProcessResultOutput = string.Format(listFolderOutputWithWorkingFolderSet, DefaultWorkingDirectory);
			ExpectToExecuteArguments(@"listworkingfolders" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"get $ -destpath " + StringUtil.AutoDoubleQuoteString(DefaultWorkingDirectory) + GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public virtual void ShouldNotGetSourceIfNoWorkingFolderMatchesAndUseWorkingFolderTrueAndNoWorkingFolderSetInVault()
		{
			this.ProcessResultOutput = string.Format(listFolderOutputWithWorkingFolderSet, DefaultWorkingDirectory);
			ExpectToExecuteArguments(@"listworkingfolders");
			vault.AutoGetSource = true;
			vault.WorkingDirectory = "";
			vault.UseVaultWorkingDirectory = true;
			vault.Folder = @"$/noworkingfoldersetforme";
			vault.ApplyLabel = true;
			bool VaultExceptionThrown = false;
			try
			{
				vault.GetSource(result);
			}
			catch (Vault3.VaultException)
			{
				VaultExceptionThrown = true;
			}
			VerifyAll();
			Assert.IsTrue(VaultExceptionThrown, "Vault class did not throw expected exception.");
		}

		[Test, Ignore("Ignored until i get right of the problem.")]
		public virtual void ShouldNotGetSourceIfNoWorkingFolderSpecifiedAndUseWorkingFolderTrueAndNoWorkingFolderSetInVault()
		{
			this.ProcessResultOutput = listFolderOutputWithNoWorkingFolderSet;
			ExpectToExecuteArguments(@"listworkingfolders", null);
			vault.AutoGetSource = true;
			vault.WorkingDirectory = null;
			vault.UseVaultWorkingDirectory = true;
			vault.Folder = @"$";
			vault.ApplyLabel = true;
			bool VaultExceptionThrown = false;
			try
			{
				vault.GetSource(result);
			}
			catch (Vault3.VaultException)
			{
				VaultExceptionThrown = true;
			}
			VerifyAll();
			Assert.IsTrue(VaultExceptionThrown, "Vault class did not throw expected exception.");
		}

		[Test]
		public virtual void ShouldNotGetSourceIfAutoGetSourceIsFalse()
		{
			ExpectThatExecuteWillNotBeCalled();
			vault.AutoGetSource = false;
			vault.GetSource(IntegrationResultMother.CreateSuccessful());
			VerifyAll();
		}

		[Test]
		public virtual void ShouldNotApplyLabelOrGetByLabelIfApplyLabelIsFalse()
		{
			ExpectToExecuteArguments(@"get $ -performdeletions removeworkingcopy -merge overwrite -makewritable -setfiletime checkin");
			vault.ApplyLabel = false;
			vault.AutoGetSource = true;
			vault.UseVaultWorkingDirectory = true;
			vault.Folder = "$";
			vault.CleanCopy = false;
			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public virtual void ShouldBuildApplyLabelArgumentsCorrectlyNonAutoGet()
		{
			ExpectToExecuteArguments(@"label $ foo");
			vault.ApplyLabel = true;
			vault.AutoGetSource = false;
			vault.Folder = "$";
			vault.LabelSourceControl(result);
			VerifyAll();
		}

		[Test]
		public virtual void ShouldSetAndRemoveLabelOnFailure()
		{
			ExpectToExecuteArguments(@"label $ foo");
			ExpectToExecuteArguments(@"getlabel $ foo -destpath " + StringUtil.AutoDoubleQuoteString(DefaultWorkingDirectory) + " -merge overwrite -makewritable -setfiletime checkin");
			ExpectToExecuteArguments(@"deletelabel $ foo");
			vault.ApplyLabel = true;
			vault.UseVaultWorkingDirectory = false;
			vault.WorkingDirectory = DefaultWorkingDirectory;
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
		public virtual void ShouldNotDeleteLabelIfItWasNeverApplied()
		{
			this.ExpectThatExecuteWillNotBeCalled();
			vault.ApplyLabel = true;
			vault.UseVaultWorkingDirectory = false;
			vault.WorkingDirectory = DefaultWorkingDirectory;
			vault.AutoGetSource = true;
			vault.Folder = "$";

			IntegrationResult failed = IntegrationResultMother.CreateFailed();
			failed.Label = result.Label;
			failed.WorkingDirectory = result.WorkingDirectory;
			vault.LabelSourceControl(failed);
			VerifyAll();
		}

		[Test]
		public virtual void ShouldBuildLabelArgumentsCorrectlyOnFailureNonAutoget()
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
		public virtual void ShouldBuildApplyLabelArgumentsIncludingCommonArguments()
		{
			ExpectToExecuteArguments(@"label $ foo" + SetAndGetCommonOptionalArguments());
			vault.ApplyLabel = true;
			vault.AutoGetSource = false;
			vault.Folder = "$";
			vault.LabelSourceControl(result);
			VerifyAll();
		}

		[Test]
		public virtual void ShouldStripNonXmlFromWorkingFolderList()
		{
			vault.Folder = "$";
			vault.AutoGetSource = true;

			vault.ApplyLabel = true;
			vault.UseVaultWorkingDirectory = true;
			vault.WorkingDirectory = null;
			vault.CleanCopy = false;

			this.ProcessResultOutput = string.Format(listFolderOutputWithNonXml, DefaultWorkingDirectory);
			ExpectToExecuteArguments(@"listworkingfolders" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"label $ foo" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"getlabel $ foo -labelworkingfolder " + StringUtil.AutoDoubleQuoteString(DefaultWorkingDirectory)
				+ GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		protected string SetAndGetCommonOptionalArguments()
		{
			vault.Host = "localhost";
			vault.Username = "username";
			vault.Password = "password";
			vault.Repository = "my repository";
			vault.Ssl = true;
			return @" -host localhost -user username -password password -repository ""my repository"" -ssl";
		}

		protected string GetWorkingFolderArguments()
		{
			if (vault.ApplyLabel)
				return @" -merge overwrite -makewritable";
			else
			{
				if (vault.UseVaultWorkingDirectory)
					return @" -performdeletions removeworkingcopy -merge overwrite -makewritable";
				else
					return @" -merge overwrite -makewritable";
			}
		}

		protected string GetFileTimeArgument()
		{
			return @" -setfiletime " + vault.setFileTime;
		}

		protected VaultVersionChecker CreateVault(string xml)
		{
			return (VaultVersionChecker) NetReflector.Read(xml);
		}

		protected void ExpectToParseHistory()
		{
			mockHistoryParser.ExpectAndReturn("Parse", new Modification[0], new IsAnything(), new IsAnything(), new IsAnything());
		}

		/* 
		* CleanFolder tests commented out because tests that are tied to a
		* particular file system layout are undesirable, per Owen 11/29/05
		*
		private void CreateTempFileForCleanFolderTest()
		{
			tempFileToTestCleanCopy = Path.GetTempFileName();
			string tempFileNameOnly = Path.GetFileName(tempFileToTestCleanCopy);
			File.Move(tempFileToTestCleanCopy, Path.Combine(vault.WorkingDirectory, tempFileNameOnly));
			tempFileToTestCleanCopy = Path.Combine(vault.WorkingDirectory, tempFileNameOnly);
		}
		*/

		protected void ExpectToCleanFolder()
		{
			/* 
			 * CleanFolder tests commented out because tests that are tied to a
			 * particular file system layout are undesirable, per Owen 11/29/05
			 * 

			// can't test if we don't know the working folder ahead of time
			if ( StringUtil.IsBlank(vault.WorkingDirectory) )
			{
				Assert.Fail("\"ExpectToCleanFolder\" can only be used when the working directory is specified.");
			}
			CreateTempFileForCleanFolderTest();
			this.expectCleanCopy = true;
			*/
		}

		protected void ExpectToNotCleanFolder()
		{
			/* 
			 * CleanFolder tests commented out because tests that are tied to a
			 * particular file system layout are undesirable, per Owen 11/29/05
			 *

			// can't test if we don't know the working folder ahead of time
			if ( StringUtil.IsBlank(vault.WorkingDirectory) )
			{
				Assert.Fail("\"ExpectNotToCleanFolder\" can only be used when the working directory is specified.");
			}
			CreateTempFileForCleanFolderTest();
			this.expectCleanCopy = false;
			*/
		}

		protected void VerifyAll()
		{
			/* 
			 * CleanFolder tests commented out because tests that are tied to a
			 * particular file system layout are undesirable, per Owen 11/29/05
			 *
			  
			if ( !StringUtil.IsBlank(tempFileToTestCleanCopy) )
			{
				if ( this.expectCleanCopy )
					Assert.IsFalse( File.Exists(tempFileToTestCleanCopy), "The working directory should have been cleaned, but was not." );
				else
					Assert.IsTrue( File.Exists(tempFileToTestCleanCopy), "The working directory should not have been cleaned, but it was." );
				File.Delete(tempFileToTestCleanCopy);
				tempFileToTestCleanCopy = null;
			}
			*/
			Verify();
			mockHistoryParser.Verify();
		}
	}
}