using System;
using System.IO;
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
	public class MksTest : CustomAssertion
	{
		private static DateTime FROM = DateTime.Now.AddMinutes(-10);
		private static DateTime TO = DateTime.Now;

		private string sandboxRoot;

		private Mks mks;
		private IHistoryParser mockHistoryParser;
		private Mock mockHistoryParserWrapper;
		private Mock mockExecutorWrapper;
		private ProcessExecutor mockProcessExecutor;
		private Mock mockIntegrationResult;
		private IIntegrationResult integrationResult;
		private Mock mksHistoryParserWrapper;
		private MksHistoryParser mksHistoryParser;

		[SetUp]
		public void SetUp()
		{
			sandboxRoot = TempFileUtil.GetTempPath("MksSandBox");

			mockHistoryParserWrapper = new DynamicMock(typeof (IHistoryParser));
			mockHistoryParser = (IHistoryParser) mockHistoryParserWrapper.MockInstance;

			mksHistoryParserWrapper = new DynamicMock(typeof (MksHistoryParser));
			mksHistoryParser = (MksHistoryParser) mksHistoryParserWrapper.MockInstance;

			mockExecutorWrapper = new DynamicMock(typeof (ProcessExecutor));
			mockProcessExecutor = (ProcessExecutor) mockExecutorWrapper.MockInstance;

			mockIntegrationResult = new DynamicMock(typeof (IIntegrationResult));
			integrationResult = (IIntegrationResult) mockIntegrationResult.MockInstance;
		}

		[TearDown]
		public void TearDown()
		{
			mockExecutorWrapper.Verify();
			mockHistoryParserWrapper.Verify();
			mksHistoryParserWrapper.Verify();
			mockIntegrationResult.Verify();
		}

		private string CreateSourceControlXml()
		{
			return string.Format(
				@"    <sourceControl type=""mks"">
						  <executable>..\bin\si.exe</executable>
						  <port>8722</port>
						  <hostname>hostname</hostname>
						  <user>CCNetUser</user>
						  <password>CCNetPassword</password>
						  <sandboxroot>{0}</sandboxroot>
						  <sandboxfile>myproject.pj</sandboxfile>
						  <autoGetSource>true</autoGetSource>
						  <checkpointOnSuccess>true</checkpointOnSuccess>
					  </sourceControl>
				 ", sandboxRoot);
		}

		[Test]
		public void CheckDefaults()
		{
			Mks defalutMks = new Mks();
			Assert.AreEqual(@"si.exe", defalutMks.Executable);
			Assert.AreEqual(8722, defalutMks.Port);
			Assert.AreEqual(true, defalutMks.AutoGetSource);
			Assert.AreEqual(false, defalutMks.CheckpointOnSuccess);
		}

		[Test]
		public void ValuePopulation()
		{
			mks = CreateMks(CreateSourceControlXml(), null, null);

			Assert.AreEqual(@"..\bin\si.exe", mks.Executable);
			Assert.AreEqual(@"hostname", mks.Hostname);
			Assert.AreEqual(8722, mks.Port);
			Assert.AreEqual(@"CCNetUser", mks.User);
			Assert.AreEqual(@"CCNetPassword", mks.Password);
			Assert.AreEqual(sandboxRoot, mks.SandboxRoot);
			Assert.AreEqual(@"myproject.pj", mks.SandboxFile);
			Assert.AreEqual(true, mks.AutoGetSource);
			Assert.AreEqual(true, mks.CheckpointOnSuccess);
		}

		[Test]
		public void GetSource()
		{
			string expectedResyncCommand = string.Format(@"resync --overwriteChanged --restoreTimestamp -R -S {0}\myproject.pj --user=CCNetUser --password=CCNetPassword --quiet", sandboxRoot);
			mockExecutorWrapper.ExpectAndReturn("Execute", new ProcessResult(null, null, 0, false), ExpectedProcessInfo(expectedResyncCommand));
			string expectedAttribCommand = string.Format(@"-R /s {0}\*", sandboxRoot);
			mockExecutorWrapper.ExpectAndReturn("Execute", new ProcessResult(null, null, 0, false), ExpectedProcessInfo("attrib", expectedAttribCommand));

			mks = CreateMks(CreateSourceControlXml(), mockHistoryParser, mockProcessExecutor);
			mks.GetSource(null);
		}

		[Test]
		public void GetSourceWithSpacesInSandbox()
		{
			sandboxRoot = TempFileUtil.GetTempPath("Mks Sand Box");
			string expectedResyncCommand = string.Format(@"resync --overwriteChanged --restoreTimestamp -R -S ""{0}\myproject.pj"" --user=CCNetUser --password=CCNetPassword --quiet", sandboxRoot);
			mockExecutorWrapper.ExpectAndReturn("Execute", new ProcessResult(null, null, 0, false), ExpectedProcessInfo(expectedResyncCommand));
			string expectedAttribCommand = string.Format(@"-R /s ""{0}\*""", sandboxRoot);
			mockExecutorWrapper.ExpectAndReturn("Execute", new ProcessResult(null, null, 0, false), ExpectedProcessInfo("attrib", expectedAttribCommand));

			mks = CreateMks(CreateSourceControlXml(), mockHistoryParser, mockProcessExecutor);
			mks.GetSource(null);
		}

		[Test]
		public void CheckpointSourceOnSuccessfulBuild()
		{
			string expectedCommand = string.Format(@"checkpoint -d ""Cruise Control.Net Build - 20"" -L ""Build - 20"" -R -S {0}\myproject.pj --user=CCNetUser --password=CCNetPassword --quiet", sandboxRoot);
			ProcessInfo expectedProcessInfo = ExpectedProcessInfo(expectedCommand);
			mockExecutorWrapper.ExpectAndReturn("Execute", new ProcessResult(null, null, 0, false), expectedProcessInfo);
			mockIntegrationResult.ExpectAndReturn("Succeeded", true);
			mockIntegrationResult.ExpectAndReturn("Label", "20");

			mks = CreateMks(CreateSourceControlXml(), mockHistoryParser, mockProcessExecutor);
			mks.LabelSourceControl(integrationResult);
		}

		[Test]
		public void CheckpointSourceOnUnSuccessfulBuild()
		{
			mockExecutorWrapper.ExpectNoCall("Execute", typeof (ProcessInfo));
			mockIntegrationResult.ExpectAndReturn("Succeeded", false);
			mockIntegrationResult.ExpectNoCall("Label", typeof (string));

			mks = CreateMks(CreateSourceControlXml(), mockHistoryParser, mockProcessExecutor);
			mks.LabelSourceControl(integrationResult);
		}

		[Test]
		public void GetModificationsCallsParseOnHistoryParser()
		{
			mksHistoryParserWrapper.ExpectAndReturn("Parse", new Modification[0], new IsTypeOf(typeof (TextReader)), FROM, TO);
			mksHistoryParserWrapper.ExpectNoCall("ParseMemberInfoAndAddToModification", new Type[] {(typeof (Modification)), typeof (StringReader)});
			ProcessInfo expectedProcessInfo = ExpectedProcessInfo(string.Format(@"mods -R -S {0}\myproject.pj --user=CCNetUser --password=CCNetPassword --quiet", sandboxRoot));
			mockExecutorWrapper.ExpectAndReturn("Execute", new ProcessResult(null, null, 0, false), expectedProcessInfo);

			mks = CreateMks(CreateSourceControlXml(), mksHistoryParser, mockProcessExecutor);
			Modification[] modifications = mks.GetModifications(IntegrationResultMother.CreateSuccessful(FROM), IntegrationResultMother.CreateSuccessful(TO));
			Assert.AreEqual(0, modifications.Length);
		}

		[Test]
		public void GetModificationsCallsParseMemberInfo()
		{
			Modification addedModification = ModificationMother.CreateModification("myFile.file", "MyFolder");
			addedModification.Type = "Added";

			mksHistoryParserWrapper.ExpectAndReturn("Parse", new Modification[] {addedModification}, new IsTypeOf(typeof (TextReader)), FROM, TO);
			mksHistoryParserWrapper.ExpectAndReturn("ParseMemberInfoAndAddToModification", new Modification[] {addedModification}, new IsTypeOf(typeof (Modification)), new IsTypeOf(typeof (StringReader)));
			mockExecutorWrapper.ExpectAndReturn("Execute", new ProcessResult("", null, 0, false), new IsTypeOf(typeof (ProcessInfo)));

			string expectedCommand = string.Format(@"memberinfo -S {0}\myproject.pj --user=CCNetUser --password=CCNetPassword --quiet {0}\MyFolder\myFile.file", sandboxRoot);
			ProcessInfo expectedProcessInfo = ExpectedProcessInfo(expectedCommand);
			mockExecutorWrapper.ExpectAndReturn("Execute", new ProcessResult(null, null, 0, false), expectedProcessInfo);

			mks = CreateMks(CreateSourceControlXml(), mksHistoryParser, mockProcessExecutor);
			Modification[] modifications = mks.GetModifications(IntegrationResultMother.CreateSuccessful(FROM), IntegrationResultMother.CreateSuccessful(TO));
			Assert.AreEqual(1, modifications.Length);
		}

		[Test]
		public void HandleSpacesInGetModifications()
		{
			sandboxRoot = TempFileUtil.GetTempPath("MksSandBox With Spaces");
			mksHistoryParserWrapper.ExpectAndReturn("Parse", new Modification[0], new IsTypeOf(typeof (TextReader)), FROM, TO);
			mksHistoryParserWrapper.ExpectNoCall("ParseMemberInfoAndAddToModification", new Type[] {(typeof (Modification)), typeof (StringReader)});
			ProcessInfo expectedProcessInfo = ExpectedProcessInfo(string.Format(@"mods -R -S ""{0}\myproject.pj"" --user=CCNetUser --password=CCNetPassword --quiet", sandboxRoot));
			mockExecutorWrapper.ExpectAndReturn("Execute", new ProcessResult(null, null, 0, false), expectedProcessInfo);

			mks = CreateMks(CreateSourceControlXml(), mksHistoryParser, mockProcessExecutor);
			Modification[] modifications = mks.GetModifications(IntegrationResultMother.CreateSuccessful(FROM), IntegrationResultMother.CreateSuccessful(TO));
			Assert.AreEqual(0, modifications.Length);
		}

		[Test]
		public void HandleSpacesInParseMemberInfo()
		{
			sandboxRoot = TempFileUtil.GetTempPath("MksSandBox With Spaces");
			
			Modification addedModification = ModificationMother.CreateModification("myFile.file", "MyFolder");
			addedModification.Type = "Added";

			mksHistoryParserWrapper.ExpectAndReturn("Parse", new Modification[] {addedModification}, new IsTypeOf(typeof (TextReader)), FROM, TO);
			mksHistoryParserWrapper.ExpectAndReturn("ParseMemberInfoAndAddToModification", new Modification[] {addedModification}, new IsTypeOf(typeof (Modification)), new IsTypeOf(typeof (StringReader)));
			mockExecutorWrapper.ExpectAndReturn("Execute", new ProcessResult("", null, 0, false), new IsTypeOf(typeof (ProcessInfo)));

			string expectedCommand = string.Format(@"memberinfo -S ""{0}\myproject.pj"" --user=CCNetUser --password=CCNetPassword --quiet ""{0}\MyFolder\myFile.file""", sandboxRoot);
			ProcessInfo expectedProcessInfo = ExpectedProcessInfo(expectedCommand);
			mockExecutorWrapper.ExpectAndReturn("Execute", new ProcessResult(null, null, 0, false), expectedProcessInfo);

			mks = CreateMks(CreateSourceControlXml(), mksHistoryParser, mockProcessExecutor);
			Modification[] modifications = mks.GetModifications(IntegrationResultMother.CreateSuccessful(FROM), IntegrationResultMother.CreateSuccessful(TO));
			Assert.AreEqual(1, modifications.Length);
		}

		[Test]
		public void GetModificationsForModificationInRootFolder()
		{
			sandboxRoot = TempFileUtil.GetTempPath("MksSandBox");
			
			Modification addedModification = ModificationMother.CreateModification("myFile.file", null);
			addedModification.Type = "Added";

			mksHistoryParserWrapper.ExpectAndReturn("Parse", new Modification[] {addedModification}, new IsTypeOf(typeof (TextReader)), FROM, TO);
			mksHistoryParserWrapper.ExpectAndReturn("ParseMemberInfoAndAddToModification", new Modification[] {addedModification}, new IsTypeOf(typeof (Modification)), new IsTypeOf(typeof (StringReader)));
			mockExecutorWrapper.ExpectAndReturn("Execute", new ProcessResult("", null, 0, false), new IsTypeOf(typeof (ProcessInfo)));

			string expectedCommand = string.Format(@"memberinfo -S {0}\myproject.pj --user=CCNetUser --password=CCNetPassword --quiet {0}\myFile.file", sandboxRoot);
			ProcessInfo expectedProcessInfo = ExpectedProcessInfo(expectedCommand);
			mockExecutorWrapper.ExpectAndReturn("Execute", new ProcessResult(null, null, 0, false), expectedProcessInfo);

			mks = CreateMks(CreateSourceControlXml(), mksHistoryParser, mockProcessExecutor);
			Modification[] modifications = mks.GetModifications(IntegrationResultMother.CreateSuccessful(FROM), IntegrationResultMother.CreateSuccessful(TO));
			Assert.AreEqual(1, modifications.Length);
		}

		[Test]
		public void GetModificationsCallsMemberInfoForNonDeletedModifications()
		{
			Modification addedModification = ModificationMother.CreateModification("myFile.file", "MyFolder");
			addedModification.Type = "Added";

			Modification modifiedModification = ModificationMother.CreateModification("myFile.file", "MyFolder");
			modifiedModification.Type = "Modified";

			Modification deletedModification = ModificationMother.CreateModification("myFile.file", "MyFolder");
			deletedModification.Type = "Deleted";

			mksHistoryParserWrapper.ExpectAndReturn("Parse", new Modification[] {addedModification, modifiedModification, deletedModification}, new IsTypeOf(typeof (TextReader)), FROM, TO);
			mksHistoryParserWrapper.ExpectAndReturn("ParseMemberInfoAndAddToModification", null, addedModification, new IsTypeOf(typeof(StringReader)));
			mksHistoryParserWrapper.ExpectAndReturn("ParseMemberInfoAndAddToModification", null, modifiedModification, new IsTypeOf(typeof(StringReader)));
			mockExecutorWrapper.SetupResult("Execute", new ProcessResult("", null, 0, false), new Type[]{typeof (ProcessInfo)});

			mks = CreateMks(CreateSourceControlXml(), mksHistoryParser, mockProcessExecutor);
			Modification[] modifications = mks.GetModifications(IntegrationResultMother.CreateSuccessful(FROM), IntegrationResultMother.CreateSuccessful(TO));
			Assert.AreEqual(3, modifications.Length);
		}

		[Test]
		public void GetModificationsFiltersByModifiedTimeIfCheckpointOnSuccessIsFalse()
		{
			Modification modificationBeforePreviousIntegration = ModificationMother.CreateModification("ccnet", FROM.AddMinutes(-2));
			Modification modificationInThisIntegration = ModificationMother.CreateModification("ccnet", TO.AddMinutes(-1));
			Modification modificationAfterIntegrationStartTime = ModificationMother.CreateModification("myFile.file", TO.AddMinutes(1));

			Modification[] integrationModifications = new Modification[] {modificationBeforePreviousIntegration, modificationInThisIntegration, modificationAfterIntegrationStartTime};
			mksHistoryParserWrapper.ExpectAndReturn("Parse", integrationModifications, new IsTypeOf(typeof (TextReader)), FROM, TO);
			mksHistoryParserWrapper.ExpectAndReturn("ParseMemberInfoAndAddToModification", null, modificationBeforePreviousIntegration, new IsTypeOf(typeof(StringReader)));
			mksHistoryParserWrapper.ExpectAndReturn("ParseMemberInfoAndAddToModification", null, modificationInThisIntegration, new IsTypeOf(typeof(StringReader)));
			mksHistoryParserWrapper.ExpectAndReturn("ParseMemberInfoAndAddToModification", null, modificationAfterIntegrationStartTime, new IsTypeOf(typeof(StringReader)));
			mockExecutorWrapper.SetupResult("Execute", new ProcessResult("", null, 0, false), new Type[]{typeof (ProcessInfo)});

			mks = CreateMks(CreateSourceControlXml(), mksHistoryParser, mockProcessExecutor);
			mks.CheckpointOnSuccess = false;
			Modification[] modifications = mks.GetModifications(IntegrationResultMother.CreateSuccessful(FROM), IntegrationResultMother.CreateSuccessful(TO));
			Assert.AreEqual(1, modifications.Length);
		}

		private Mks CreateMks(string xml, IHistoryParser historyParser, ProcessExecutor executor)
		{
			Mks newMks = new Mks(historyParser, executor);
			NetReflector.Read(xml, newMks);
			return newMks;
		}

		private ProcessInfo ExpectedProcessInfo(string arguments)
		{
			return ExpectedProcessInfo(@"..\bin\si.exe", arguments);
		}

		private ProcessInfo ExpectedProcessInfo(string executable, string arguments)
		{
			ProcessInfo expectedProcessInfo = new ProcessInfo(executable, arguments);
			expectedProcessInfo.TimeOut = Timeout.DefaultTimeout.Millis;
			return expectedProcessInfo;
		}
	}
}