using System;
using System.Globalization;
using System.Text;
using System.Threading;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	[TestFixture]
	public class Vault317Test : Vault3Test
	{
		private bool _bModificationsRetrieved = false;
		private CultureInfo culture = CultureInfo.InvariantCulture;

		[SetUp]
		public override void SetUp()
		{
			Thread.CurrentThread.CurrentCulture = culture;

			CreateProcessExecutorMock(VaultVersionChecker.DefaultExecutable);
			mockHistoryParser = new DynamicMock(typeof (IHistoryParser));
			vault = new VaultVersionChecker(new VaultHistoryParser(culture), (ProcessExecutor) mockProcessExecutor.MockInstance, VaultVersionChecker.EForcedVaultVersion.Vault317);

			result = IntegrationResultMother.CreateSuccessful("foo");
			result.WorkingDirectory = this.DefaultWorkingDirectory;
			_bModificationsRetrieved = false;
		}

		[Test]
		public void FindsSimpleChange()
		{
			DateTime dtBeforeChange = DateTime.Parse("1/18/2006 3:34:06 PM", culture);
			DateTime dtAfterChange = DateTime.Parse("1/18/2006 3:34:08 PM", culture);
			result.StartTime = dtBeforeChange;

			this.ProcessResultOutput = @"
				<vault>
					<history>
						<item version=""78"" date=""1/18/2006 3:34:07 PM"" user=""testuser"" comment=""test comment"" txid=""4"" />
					</history>
					<result success=""yes"" />
				</vault>";
			ExpectToExecuteArguments(@"versionhistory $ -rowlimit 1" + SetAndGetCommonOptionalArguments());

			this.ProcessResultOutput = @"
				<vault>
					<history>
						<item txid=""4"" date=""1/18/2006 3:34:07 PM"" name=""test.cs"" type=""60"" version=""29"" user=""testuser"" comment=""test comment"" actionString=""Checked In"" />
					</history>
					<result success=""yes"" />
				</vault>";
			string itemHistoryArgs = string.Format(@"history $ -excludeactions label,obliterate -rowlimit 0 -begindate {0:s} -enddate {1:s}{2}",
				dtBeforeChange, dtAfterChange, SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(itemHistoryArgs);

			Modification[] mods = vault.GetModifications(result, IntegrationResultMother.CreateSuccessful(dtAfterChange));
			Assert.AreEqual(1, mods.Length, "Should have found 1 modification.");

			VerifyAll();

			this.ProcessResultOutput = @"
				<vault>
					<history>
						<item version=""79"" date=""1/18/2006 3:35:07 PM"" user=""testuser"" comment=""test comment"" txid=""5"" />
					</history>
					<result success=""yes"" />
				</vault>";
			ExpectToExecuteArguments(@"versionhistory $ -beginversion 79 -rowlimit 1" + SetAndGetCommonOptionalArguments());

			this.ProcessResultOutput = @"
				<vault>
					<history>
						<item txid=""5"" date=""1/18/2006 3:35:07 PM"" name=""test.cs"" type=""60"" version=""30"" user=""testuser"" comment=""test comment"" actionString=""Checked In"" />
					</history>
					<result success=""yes"" />
				</vault>";
			dtAfterChange = DateTime.Parse("1/18/2006 3:36:07 PM", culture);
			itemHistoryArgs = string.Format(@"history $ -excludeactions label,obliterate -rowlimit 0 -begindate {0:s} -enddate {1:s}{2}",
				dtBeforeChange, dtAfterChange, SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(itemHistoryArgs);

			mods = vault.GetModifications(result, IntegrationResultMother.CreateSuccessful(dtAfterChange));
			Assert.AreEqual(1, mods.Length, "Should have found 1 modification.");

			VerifyAll();
		}

		[Test]
		public void TakesTransactionIntoAccountWhenGettingModifications()
		{
			DateTime dtBeforeChange = DateTime.Parse("1/18/2006 3:34:06 PM", culture);
			DateTime dtAfterChange = DateTime.Parse("1/18/2006 3:34:08 PM", culture);

			result.StartTime = dtBeforeChange;

			this.ProcessResultOutput = @"
				<vault>
					<history>
						<item version=""78"" date=""1/18/2006 3:34:07 PM"" user=""testuser"" comment=""test comment"" txid=""4"" />
					</history>
					<result success=""yes"" />
				</vault>";
			ExpectToExecuteArguments(@"versionhistory $ -rowlimit 1" + SetAndGetCommonOptionalArguments());

			this.ProcessResultOutput = @"
				<vault>
					<history>
						<item txid=""4"" date=""1/18/2006 3:34:07 PM"" name=""test.cs"" type=""60"" version=""29"" user=""testuser"" comment=""test comment"" actionString=""Checked In"" />
						<item txid=""5"" date=""1/18/2006 3:35:07 PM"" name=""test.cs"" type=""60"" version=""30"" user=""testuser"" comment=""test comment"" actionString=""Checked In"" />
					</history>
					<result success=""yes"" />
				</vault>";
			string itemHistoryArgs = string.Format(@"history $ -excludeactions label,obliterate -rowlimit 0 -begindate {0:s} -enddate {1:s}{2}",
				dtBeforeChange, dtAfterChange, SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(itemHistoryArgs);

			Modification[] mods = vault.GetModifications(result, IntegrationResultMother.CreateSuccessful(dtAfterChange));
			Assert.AreEqual(1, mods.Length, "Should have found 1 modification.");

			VerifyAll();

			this.ProcessResultOutput = @"
				<vault>
					<history>
						<item version=""79"" date=""1/18/2006 3:35:07 PM"" user=""testuser"" comment=""test comment"" txid=""5"" />
					</history>
					<result success=""yes"" />
				</vault>";
			ExpectToExecuteArguments(@"versionhistory $ -beginversion 79 -rowlimit 1" + SetAndGetCommonOptionalArguments());

			this.ProcessResultOutput = @"
				<vault>
					<history>
						<item txid=""5"" date=""1/18/2006 3:35:07 PM"" name=""test.cs"" type=""60"" version=""30"" user=""testuser"" comment=""test comment"" actionString=""Checked In"" />
						<item txid=""6"" date=""1/18/2006 3:36:07 PM"" name=""test.cs"" type=""60"" version=""31"" user=""testuser"" comment=""test comment"" actionString=""Checked In"" />
					</history>
					<result success=""yes"" />
				</vault>";
			DateTime dtEnd = DateTime.Parse("1/18/2006 3:36:07 PM", culture);
			itemHistoryArgs = string.Format(@"history $ -excludeactions label,obliterate -rowlimit 0 -begindate {0:s} -enddate {1:s}{2}",
				dtBeforeChange, dtEnd, SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(itemHistoryArgs);

			mods = vault.GetModifications(result, IntegrationResultMother.CreateSuccessful(dtEnd));
			Assert.AreEqual(1, mods.Length, "Should have found 1 modification.");

			VerifyAll();
		}

		/*
		 *               Get Source Scenarios           || Correct Action
		 * ---------------------------------------------||-------------------------------------------------------------------
		 *          |       |   Use   | Working |       ||  List   |
		 *          | Apply | Working | Folder  | Clean || Working | Get Command
		 * Scenario | Label | Folder  | Spec'd  | Copy  || Folders | and Arguments
		 * ------------------------------------------------------------------------------------------------------------------
		 *    1     |   T   |    T    |    T    |   T   ||    F    | getversion 78 $ c:\source -useworkingfolder
		 *    2     |   T   |    T    |    T    |   F   ||    F    | getversion 78 $ c:\source -useworkingfolder
		 *    3     |   T   |    T    |    F    |   T   ||    T    | getversion 78 $
		 *    4     |   T   |    T    |    F    |   F   ||    F    | getversion 78 $
		 *    5     |   T   |    F    |    T    |   T   ||    F    | getversion 78 $ c:\source
		 *    6     |   T   |    F    |    T    |   F   ||    F    | getversion 78 $ c:\source
		 *    7     |   T   |    F    |    F    |   T   ||    T    | getversion 78 $
		 *    8     |   T   |    F    |    F    |   F   ||    F    | getversion 78 $
		 *    9     |   F   |    T    |    T    |   T   ||    F    | getversion 78 $ c:\source -useworkingfolder
		 *   10     |   F   |    T    |    T    |   F   ||    F    | getversion 78 $ c:\source -useworkingfolder
		 *   11     |   F   |    T    |    F    |   T   ||    T    | getversion 78 $
		 *   12     |   F   |    T    |    F    |   F   ||    F    | getversion 78 $
		 *   13     |   F   |    F    |    T    |   T   ||    F    | getversion 78 $ c:\source
		 *   14     |   F   |    F    |    T    |   F   ||    F    | getversion 78 $ c:\source
		 *   15     |   F   |    F    |    F    |   T   ||    T    | getversion 78 $
		 *   16     |   F   |    F    |    F    |   F   ||    F    | getversion 78 $
		 */

		[Test]
		public override void ArgumentsCorrectForGetSourceScenario1()
		{
			vault.Folder = "$";
			vault.AutoGetSource = true;

			vault.ApplyLabel = true;
			vault.UseVaultWorkingDirectory = true;
			vault.WorkingDirectory = DefaultWorkingDirectory;
			vault.CleanCopy = true;

			GetModsToInitFolderVersionIfNecessary();

			ExpectToCleanFolder();
			ExpectToExecuteArguments(@"getversion 78 $" 
				+ GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public override void ArgumentsCorrectForGetSourceScenario2()
		{
			vault.Folder = "$";
			vault.AutoGetSource = true;

			vault.ApplyLabel = true;
			vault.UseVaultWorkingDirectory = true;
			vault.WorkingDirectory = DefaultWorkingDirectory;
			vault.CleanCopy = false;

			GetModsToInitFolderVersionIfNecessary();

			ExpectToNotCleanFolder();
			ExpectToExecuteArguments(@"getversion 78 $" 
				+ GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public override void ArgumentsCorrectForGetSourceScenario3()
		{
			vault.Folder = "$";
			vault.AutoGetSource = true;

			vault.ApplyLabel = true;
			vault.UseVaultWorkingDirectory = true;
			vault.WorkingDirectory = @"";
			vault.CleanCopy = true;

			GetModsToInitFolderVersionIfNecessary();

			this.ProcessResultOutput = string.Format(listFolderOutputWithWorkingFolderSet, DefaultWorkingDirectory);
			ExpectToExecuteArguments(@"listworkingfolders" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"getversion 78 $" 
				+ GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public override void ArgumentsCorrectForGetSourceScenario4()
		{
			vault.Folder = "$";
			vault.AutoGetSource = true;

			vault.ApplyLabel = true;
			vault.UseVaultWorkingDirectory = true;
			vault.WorkingDirectory = @"";
			vault.CleanCopy = false;

			GetModsToInitFolderVersionIfNecessary();

			ExpectToExecuteArguments(@"getversion 78 $" 
				+ GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public override void ArgumentsCorrectForGetSourceScenario5()
		{
			vault.Folder = "$";
			vault.AutoGetSource = true;

			vault.ApplyLabel = true;
			vault.UseVaultWorkingDirectory = false;
			vault.WorkingDirectory = DefaultWorkingDirectory;
			vault.CleanCopy = true;

			GetModsToInitFolderVersionIfNecessary();

			ExpectToCleanFolder();
			ExpectToExecuteArguments(@"getversion 78 $" 
				+ GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public override void ArgumentsCorrectForGetSourceScenario6()
		{
			vault.Folder = "$";
			vault.AutoGetSource = true;

			vault.ApplyLabel = true;
			vault.UseVaultWorkingDirectory = false;
			vault.WorkingDirectory = DefaultWorkingDirectory;
			vault.CleanCopy = false;

			GetModsToInitFolderVersionIfNecessary();

			ExpectToNotCleanFolder();
			ExpectToExecuteArguments(@"getversion 78 $" 
				+ GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public override void ArgumentsCorrectForGetSourceScenario7()
		{
			vault.Folder = "$";
			vault.AutoGetSource = true;

			vault.ApplyLabel = true;
			vault.UseVaultWorkingDirectory = false;
			vault.WorkingDirectory = @"";
			vault.CleanCopy = true;

			GetModsToInitFolderVersionIfNecessary();

			this.ProcessResultOutput = string.Format(listFolderOutputWithWorkingFolderSet, DefaultWorkingDirectory);
			ExpectToExecuteArguments(@"listworkingfolders" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"getversion 78 $" 
				+ GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public override void ArgumentsCorrectForGetSourceScenario8()
		{
			vault.Folder = "$";
			vault.AutoGetSource = true;

			vault.ApplyLabel = true;
			vault.UseVaultWorkingDirectory = false;
			vault.WorkingDirectory = @"";
			vault.CleanCopy = false;

			GetModsToInitFolderVersionIfNecessary();

			ExpectToExecuteArguments(@"getversion 78 $" 
				+ GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public override void ArgumentsCorrectForGetSourceScenario9()
		{
			vault.Folder = "$";
			vault.AutoGetSource = true;

			vault.ApplyLabel = false;
			vault.UseVaultWorkingDirectory = true;
			vault.WorkingDirectory = DefaultWorkingDirectory;
			vault.CleanCopy = true;

			GetModsToInitFolderVersionIfNecessary();

			ExpectToCleanFolder();
			ExpectToExecuteArguments(@"getversion 78 $" 
				+ GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public override void ArgumentsCorrectForGetSourceScenario10()
		{
			vault.Folder = "$";
			vault.AutoGetSource = true;

			vault.ApplyLabel = false;
			vault.UseVaultWorkingDirectory = true;
			vault.WorkingDirectory = DefaultWorkingDirectory;
			vault.CleanCopy = false;

			GetModsToInitFolderVersionIfNecessary();

			ExpectToNotCleanFolder();
			ExpectToExecuteArguments(@"getversion 78 $" 
				+ GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public override void ArgumentsCorrectForGetSourceScenario11()
		{
			vault.Folder = "$";
			vault.AutoGetSource = true;

			vault.ApplyLabel = false;
			vault.UseVaultWorkingDirectory = true;
			vault.WorkingDirectory = @"";
			vault.CleanCopy = true;

			GetModsToInitFolderVersionIfNecessary();

			this.ProcessResultOutput = string.Format(listFolderOutputWithWorkingFolderSet, DefaultWorkingDirectory);
			ExpectToExecuteArguments(@"listworkingfolders" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"getversion 78 $" 
				+ GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public override void ArgumentsCorrectForGetSourceScenario12()
		{
			vault.Folder = "$";
			vault.AutoGetSource = true;

			vault.ApplyLabel = false;
			vault.UseVaultWorkingDirectory = true;
			vault.WorkingDirectory = @"";
			vault.CleanCopy = false;

			GetModsToInitFolderVersionIfNecessary();

			ExpectToExecuteArguments(@"getversion 78 $" 
				+ GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public override void ArgumentsCorrectForGetSourceScenario13()
		{
			vault.Folder = "$";
			vault.AutoGetSource = true;

			vault.ApplyLabel = false;
			vault.UseVaultWorkingDirectory = false;
			vault.WorkingDirectory = DefaultWorkingDirectory;
			vault.CleanCopy = true;

			GetModsToInitFolderVersionIfNecessary();

			ExpectToCleanFolder();
			ExpectToExecuteArguments(@"getversion 78 $" 
				+ GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public override void ArgumentsCorrectForGetSourceScenario14()
		{
			vault.Folder = "$";
			vault.AutoGetSource = true;

			vault.ApplyLabel = false;
			vault.UseVaultWorkingDirectory = false;
			vault.WorkingDirectory = DefaultWorkingDirectory;
			vault.CleanCopy = false;

			GetModsToInitFolderVersionIfNecessary();

			ExpectToNotCleanFolder();
			ExpectToExecuteArguments(@"getversion 78 $" 
				+ GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public override void ArgumentsCorrectForGetSourceScenario15()
		{
			vault.Folder = "$";
			vault.AutoGetSource = true;

			vault.ApplyLabel = false;
			vault.UseVaultWorkingDirectory = false;
			vault.WorkingDirectory = @"";
			vault.CleanCopy = true;

			GetModsToInitFolderVersionIfNecessary();

			this.ProcessResultOutput = string.Format(listFolderOutputWithWorkingFolderSet, DefaultWorkingDirectory);
			ExpectToExecuteArguments(@"listworkingfolders" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"getversion 78 $" 
				+ GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public override void ArgumentsCorrectForGetSourceScenario16()
		{
			vault.Folder = "$";
			vault.AutoGetSource = true;

			vault.ApplyLabel = false;
			vault.UseVaultWorkingDirectory = false;
			vault.WorkingDirectory = @"";
			vault.CleanCopy = false;

			GetModsToInitFolderVersionIfNecessary();

			ExpectToExecuteArguments(@"getversion 78 $" 
				+ GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public override void ShouldBuildApplyLabelArgumentsIncludingCommonArguments()
		{
			vault.ApplyLabel = true;
			vault.Folder = "$";

			GetModsToInitFolderVersionIfNecessary();

			ExpectToExecuteArguments(@"label $ foo 78" + SetAndGetCommonOptionalArguments());

			vault.LabelSourceControl(result);
			VerifyAll();
		}

		[Test]
		public override void ShouldBuildApplyLabelArgumentsCorrectlyNonAutoGet()
		{
			vault.ApplyLabel = true;
			vault.AutoGetSource = false;
			vault.Folder = "$";

			GetModsToInitFolderVersionIfNecessary();

			ExpectToExecuteArguments(@"label $ foo 78" + SetAndGetCommonOptionalArguments());

			vault.LabelSourceControl(result);
			VerifyAll();
		}
		
		[Test]
		public override void ShouldBuildGetModificationsArgumentsCorrectly()
		{
			// Doesn't apply to Vault 3.1.6+
		}

		[Test]
		public override void ShouldBuildLabelArgumentsCorrectlyOnFailureNonAutoget()
		{
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
		public override void ShouldNotApplyLabelOrGetByLabelIfApplyLabelIsFalse()
		{
			vault.ApplyLabel = false;
			vault.AutoGetSource = true;
			vault.UseVaultWorkingDirectory = true;
			vault.Folder = "$";
			vault.CleanCopy = false;

			GetModsToInitFolderVersionIfNecessary();

			ExpectToExecuteArguments(@"getversion 78 $"
				+ GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		[Test]
		public override void ShouldNotGetSourceIfNoWorkingFolderMatchesAndUseWorkingFolderTrueAndNoWorkingFolderSetInVault()
		{
			GetModsToInitFolderVersionIfNecessary();

			vault.AutoGetSource = true;
			vault.WorkingDirectory =string.Empty;
			vault.UseVaultWorkingDirectory = true;
			vault.Folder = @"$/noworkingfoldersetforme";
			vault.CleanCopy = true;
			bool VaultExceptionThrown = false;

			this.ProcessResultOutput = string.Format(listFolderOutputWithWorkingFolderSet, DefaultWorkingDirectory);
			ExpectToExecuteArguments(@"listworkingfolders" + SetAndGetCommonOptionalArguments());

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
		public override void ShouldNotGetSourceIfNoWorkingFolderSpecifiedAndUseWorkingFolderTrueAndNoWorkingFolderSetInVault()
		{
			GetModsToInitFolderVersionIfNecessary();

			vault.AutoGetSource = true;
			vault.WorkingDirectory =string.Empty;
			vault.UseVaultWorkingDirectory = true;
			vault.Folder = @"$";
			vault.CleanCopy = true;
			bool VaultExceptionThrown = false;

			this.ProcessResultOutput = listFolderOutputWithNoWorkingFolderSet;
			ExpectToExecuteArguments(@"listworkingfolders" + SetAndGetCommonOptionalArguments());

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
		public override void ShouldSetAndRemoveLabelOnFailure()
		{
			// Doesn't apply to Vault 3.1.7+
		}

		[Test]
		public override void ShouldStripNonXmlFromWorkingFolderList()
		{
			GetModsToInitFolderVersionIfNecessary();

			vault.Folder = "$";
			vault.AutoGetSource = true;

			vault.ApplyLabel = true;
			vault.UseVaultWorkingDirectory = true;
			vault.WorkingDirectory = null;
			vault.CleanCopy = true;

			this.ProcessResultOutput = string.Format(listFolderOutputWithNonXml, DefaultWorkingDirectory);
			ExpectToExecuteArguments(@"listworkingfolders" + SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(@"getversion 78 $" 
				+ GetWorkingFolderArguments() + GetFileTimeArgument() + SetAndGetCommonOptionalArguments());

			vault.GetSource(result);
			VerifyAll();
		}

		#region Non-Test Helper Functions

		/// <summary>
		/// Initialize Vault317's folderVersion field by doing a GetModifications, the same way IntegrationRunner does.
		/// GetSource() and LabelSourceControl() don't work if this step hasn't been performed.
		/// </summary>
		private void GetModsToInitFolderVersionIfNecessary()
		{
			if ( _bModificationsRetrieved )
				return;

			DateTime dtBeforeChange = DateTime.Parse("1/18/2006 3:34:06 PM", CultureInfo.InvariantCulture);
			DateTime dtAfterChange = DateTime.Parse("1/18/2006 3:34:08 PM", CultureInfo.InvariantCulture);
			result.StartTime = dtBeforeChange;

			this.ProcessResultOutput = @"
				<vault>
					<history>
						<item version=""78"" date=""1/18/2006 3:34:07 PM"" user=""testuser"" comment=""test comment"" txid=""4"" />
					</history>
					<result success=""yes"" />
				</vault>";
			ExpectToExecuteArguments(@"versionhistory $ -rowlimit 1" + SetAndGetCommonOptionalArguments());

			this.ProcessResultOutput = @"
				<vault>
					<history>
						<item txid=""4"" date=""1/18/2006 3:34:07 PM"" name=""test.cs"" type=""60"" version=""29"" user=""testuser"" comment=""test comment"" actionString=""Checked In"" />
					</history>
					<result success=""yes"" />
				</vault>";
			string itemHistoryArgs = string.Format(@"history $ -excludeactions label,obliterate -rowlimit 0 -begindate {0:s} -enddate {1:s}{2}",
				dtBeforeChange, dtAfterChange, SetAndGetCommonOptionalArguments());
			ExpectToExecuteArguments(itemHistoryArgs);

			Modification[] mods = vault.GetModifications(result, IntegrationResultMother.CreateSuccessful(dtAfterChange));
			Assert.AreEqual(1, mods.Length, "Should have found 1 modification.");

			VerifyAll();

			_bModificationsRetrieved = true;
		}

		protected new string GetWorkingFolderArguments()
		{
			StringBuilder args = new StringBuilder(200);
			args.Append(' ');
            if (!string.IsNullOrEmpty(vault.WorkingDirectory))
			{
				args.Append(string.Concat(StringUtil.AutoDoubleQuoteString(DefaultWorkingDirectory), " "));
				if ( vault.UseVaultWorkingDirectory )
					args.Append(@"-useworkingfolder ");
			}
			args.Append(@"-merge overwrite -makewritable -backup no");
			Log.Info(args.ToString());
			return args.ToString();
		}

		#endregion

	}
}
