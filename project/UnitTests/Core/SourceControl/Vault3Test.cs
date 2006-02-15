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
		protected static readonly string listFolderOutputWithWorkingFolderSet = @"
			<vault>
				<listworkingfolders>
						<workingfolder reposfolder=""$"" localfolder=""c:\source"" />
				</listworkingfolders>
				<result success=""yes"" />
			</vault>";
		protected static readonly string listFolderOutputWithNonXml = @"
			Some junk to be removed
			<vault>
				<listworkingfolders>
						<workingfolder reposfolder=""$"" localfolder=""c:\source"" />
				</listworkingfolders>
				<result success=""yes"" />
			</vault>";
		protected static readonly string listFolderOutputWithNoWorkingFolderSet = @"
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

			this.DefaultWorkingDirectory = @"c:\source";

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
				<historyArgs></historyArgs>
				<useWorkingDirectory>false</useWorkingDirectory>
				<workingDirectory>c:\source</workingDirectory>
				<cleanCopy>true</cleanCopy>
				<setFileTime>current</setFileTime>
			</vault>";

			vault = CreateVault(ST_XML_SSL);
			Assert.AreEqual(@"d:\program files\sourcegear\vault client\vault.exe", vault.Executable);
			Assert.AreEqual("username", vault.Username);
			Assert.AreEqual("password", vault.Password);
			Assert.AreEqual("localhost", vault.Host);
			Assert.AreEqual("repository", vault.Repository);
			Assert.AreEqual("$\\foo", vault.Folder);
			Assert.AreEqual(true, vault.AutoGetSource);
			Assert.AreEqual(true, vault.ApplyLabel);
			Assert.AreEqual(false, vault.UseVaultWorkingDirectory);
			Assert.AreEqual(string.Empty, vault.HistoryArgs);
			Assert.AreEqual(@"c:\source", vault.WorkingDirectory);
			Assert.AreEqual(true, vault.CleanCopy);
			Assert.AreEqual("current", vault.setFileTime);
		}

		[Test]
		public virtual void ShouldBePopulatedWithDefaultValuesWhenLoadingFromMinimalXml()
		{
			VaultVersionChecker vault = CreateVault
				(@"
				<vault>
					<host>localhost</host>
					<username>name</username>
					<password>password</password>
					<repository>repository</repository>
				</vault>
			");
			Assert.AreEqual(VaultVersionChecker.DefaultExecutable, vault.Executable);
			Assert.AreEqual("$", vault.Folder);
			Assert.AreEqual(false, vault.AutoGetSource);
			Assert.AreEqual(false, vault.ApplyLabel);
			Assert.AreEqual(true, vault.UseVaultWorkingDirectory);
			Assert.AreEqual(VaultVersionChecker.DefaultHistoryArgs, vault.HistoryArgs);
			Assert.AreEqual(false, vault.Ssl);
			Assert.AreEqual(false, vault.CleanCopy);
			Assert.AreEqual("checkin", vault.setFileTime);
			Assert.AreEqual("name", vault.Username);
			Assert.AreEqual("password", vault.Password);
			Assert.AreEqual("localhost", vault.Host);
			Assert.AreEqual("repository", vault.Repository);
		}

		[Test]
		public virtual void ShouldBuildGetModificationsArgumentsCorrectly()
		{
			DateTime today = DateTime.Now;
			DateTime yesterday = today.AddDays(-1);
			result.StartTime = yesterday;
			string args = string.Format(@"history $ -excludeactions label -rowlimit 0 -begindate {0:s} -enddate {1:s}{2}",
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
			vault.WorkingDirectory = @"c:\source";
			vault.CleanCopy = true;

			ExpectToCleanFolder();
			ExpectToExecuteArguments(@"label $ foo" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"getlabel $ foo -labelworkingfolder c:\source" 
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
			vault.WorkingDirectory = @"c:\source";
			vault.CleanCopy = false;

			ExpectToNotCleanFolder();
			ExpectToExecuteArguments(@"label $ foo" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"getlabel $ foo -labelworkingfolder c:\source" 
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

			this.ProcessResultOutput = listFolderOutputWithWorkingFolderSet;
			ExpectToExecuteArguments(@"listworkingfolders" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"label $ foo" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"getlabel $ foo -labelworkingfolder c:\source" 
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

			this.ProcessResultOutput = listFolderOutputWithWorkingFolderSet;
			ExpectToExecuteArguments(@"listworkingfolders" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"label $ foo" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"getlabel $ foo -labelworkingfolder c:\source" 
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
			vault.WorkingDirectory = @"c:\source";
			vault.CleanCopy = true;

			this.ProcessResultOutput = "";
			ExpectToCleanFolder();
			ExpectToExecuteArguments(@"label $ foo" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"getlabel $ foo -destpath c:\source" 
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
			vault.WorkingDirectory = @"c:\source";
			vault.CleanCopy = false;

			this.ProcessResultOutput = "";
			ExpectToNotCleanFolder();
			ExpectToExecuteArguments(@"label $ foo" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"getlabel $ foo -destpath c:\source" 
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

			this.ProcessResultOutput = listFolderOutputWithWorkingFolderSet;
			ExpectToExecuteArguments(@"listworkingfolders" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"label $ foo" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"getlabel $ foo -destpath c:\source" 
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

			this.ProcessResultOutput = listFolderOutputWithWorkingFolderSet;
			ExpectToExecuteArguments(@"listworkingfolders" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"label $ foo" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"getlabel $ foo -destpath c:\source" 
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
			vault.WorkingDirectory = @"c:\source";
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
			vault.WorkingDirectory = @"c:\source";
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

			this.ProcessResultOutput = listFolderOutputWithWorkingFolderSet;
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
			vault.WorkingDirectory = @"c:\source";
			vault.CleanCopy = true;

			this.ProcessResultOutput = "";
			ExpectToCleanFolder();
			ExpectToExecuteArguments(@"get $ -destpath c:\source" + GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

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
			vault.WorkingDirectory = @"c:\source";
			vault.CleanCopy = false;

			this.ProcessResultOutput = "";
			ExpectToNotCleanFolder();
			ExpectToExecuteArguments(@"get $ -destpath c:\source" + GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

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

			this.ProcessResultOutput = listFolderOutputWithWorkingFolderSet;
			ExpectToExecuteArguments(@"listworkingfolders" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"get $ -destpath c:\source" + GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

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

			this.ProcessResultOutput = listFolderOutputWithWorkingFolderSet;
			ExpectToExecuteArguments(@"listworkingfolders" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"get $ -destpath c:\source" + GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public virtual void ShouldNotGetSourceIfNoWorkingFolderMatchesAndUseWorkingFolderTrueAndNoWorkingFolderSetInVault()
		{
			this.ProcessResultOutput = listFolderOutputWithWorkingFolderSet;
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
			catch ( Vault3.VaultException )
			{
				VaultExceptionThrown = true;
			}
			VerifyAll();
			Assert.IsTrue(VaultExceptionThrown, "Vault class did not throw expected exception.");
		}

		[Test]
		public virtual void ShouldNotGetSourceIfNoWorkingFolderSpecifiedAndUseWorkingFolderTrueAndNoWorkingFolderSetInVault()
		{
			this.ProcessResultOutput = listFolderOutputWithNoWorkingFolderSet;
			ExpectToExecuteArguments(@"listworkingfolders");
			vault.AutoGetSource = true;
			vault.WorkingDirectory = "";
			vault.UseVaultWorkingDirectory = true;
			vault.Folder = @"$";
			vault.ApplyLabel = true;
			this.DefaultWorkingDirectory = "";
			bool VaultExceptionThrown = false;
			try
			{
				vault.GetSource(result);
			}
			catch ( Vault3.VaultException )
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
			ExpectToExecuteArguments(@"getlabel $ foo -destpath c:\source -merge overwrite -makewritable -setfiletime checkin");
			ExpectToExecuteArguments(@"deletelabel $ foo");
			vault.ApplyLabel = true;
			vault.UseVaultWorkingDirectory = false;
			vault.WorkingDirectory = @"c:\source";
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
			vault.WorkingDirectory = @"c:\source";
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

			this.ProcessResultOutput = listFolderOutputWithNonXml;
			ExpectToExecuteArguments(@"listworkingfolders" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"label $ foo" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"getlabel $ foo -labelworkingfolder c:\source" 
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
			if ( vault.ApplyLabel )
				return @" -merge overwrite -makewritable";
			else
			{
				if ( vault.UseVaultWorkingDirectory )
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