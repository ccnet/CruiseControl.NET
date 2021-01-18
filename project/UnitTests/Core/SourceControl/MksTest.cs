using System;
using System.IO;
using Exortech.NetReflector;
using Moq;
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
		private Mock<IHistoryParser> mockHistoryParserWrapper;
		private Mock<ProcessExecutor> mockExecutorWrapper;
		private ProcessExecutor mockProcessExecutor;
		private Mock<IIntegrationResult> mockIntegrationResult;
		private IIntegrationResult integrationResult;
		private Mock<MksHistoryParser> mksHistoryParserWrapper;
		private MksHistoryParser mksHistoryParser;

		[SetUp]
		public void SetUp()
		{
			sandboxRoot = TempFileUtil.GetTempPath("MksSandBox");

			mockHistoryParserWrapper = new Mock<IHistoryParser>();
			mockHistoryParser = (IHistoryParser) mockHistoryParserWrapper.Object;

			mksHistoryParserWrapper = new Mock<MksHistoryParser>();
			mksHistoryParser = (MksHistoryParser) mksHistoryParserWrapper.Object;

			mockExecutorWrapper = new Mock<ProcessExecutor>();
			mockProcessExecutor = (ProcessExecutor) mockExecutorWrapper.Object;

			mockIntegrationResult = new Mock<IIntegrationResult>();
			integrationResult = (IIntegrationResult) mockIntegrationResult.Object;
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
						  <autoDisconnect>true</autoDisconnect>
					  </sourceControl>
				 ", sandboxRoot).Replace('\\', System.IO.Path.DirectorySeparatorChar);
		}

		[Test]
		public void CheckDefaults()
		{
			Mks defalutMks = new Mks();
			Assert.AreEqual(@"si.exe", defalutMks.Executable);
			Assert.AreEqual(8722, defalutMks.Port);
			Assert.AreEqual(true, defalutMks.AutoGetSource);
			Assert.AreEqual(false, defalutMks.CheckpointOnSuccess);
			Assert.AreEqual(false, defalutMks.AutoDisconnect);
		}

		[Test]
		public void ValuePopulation()
		{
			mks = CreateMks(CreateSourceControlXml(), null, null);

			Assert.AreEqual(System.IO.Path.Combine("..", "bin", "si.exe"), mks.Executable);
			Assert.AreEqual(@"hostname", mks.Hostname);
			Assert.AreEqual(8722, mks.Port);
			Assert.AreEqual(@"CCNetUser", mks.User);
			Assert.AreEqual(@"CCNetPassword", mks.Password);
			Assert.AreEqual(sandboxRoot, mks.SandboxRoot);
			Assert.AreEqual(@"myproject.pj", mks.SandboxFile);
			Assert.AreEqual(true, mks.AutoGetSource);
			Assert.AreEqual(true, mks.CheckpointOnSuccess);
			Assert.AreEqual(true, mks.AutoDisconnect);
		}

		[Test]
		public void GetSource()
		{
            string expectedResyncCommand = string.Format(@"resync --overwriteChanged --restoreTimestamp --forceConfirm=yes --includeDropped -R -S {0} --user=CCNetUser --password=CCNetPassword --quiet", 
                GeneratePath(@"{0}\myproject.pj".Replace('\\', System.IO.Path.DirectorySeparatorChar), sandboxRoot));
			mockExecutorWrapper.Setup(executor => executor.Execute(ExpectedProcessInfo(expectedResyncCommand))).Returns(new ProcessResult(null, null, 0, false)).Verifiable();
			string expectedAttribCommand = string.Format(@"-R /s {0}", 
                GeneratePath(@"{0}\*".Replace('\\', System.IO.Path.DirectorySeparatorChar), sandboxRoot));
			mockExecutorWrapper.Setup(executor => executor.Execute(ExpectedProcessInfo("attrib", expectedAttribCommand))).Returns(new ProcessResult(null, null, 0, false)).Verifiable();

			string expectedDisconnectCommand = string.Format(@"disconnect --user=CCNetUser --password=CCNetPassword --quiet --forceConfirm=yes");
			ProcessInfo expectedDisconnectProcessInfo = ExpectedProcessInfo(expectedDisconnectCommand);
			mockExecutorWrapper.Setup(executor => executor.Execute(expectedDisconnectProcessInfo)).Returns(new ProcessResult(null, null, 0, false)).Verifiable();
			
			mks = CreateMks(CreateSourceControlXml(), mockHistoryParser, mockProcessExecutor);
            mks.GetSource(new IntegrationResult());
		}

		[Test]
		public void GetSourceWithSpacesInSandbox()
		{
			sandboxRoot = TempFileUtil.GetTempPath("Mks Sand Box");
            string expectedResyncCommand = string.Format(@"resync --overwriteChanged --restoreTimestamp --forceConfirm=yes --includeDropped -R -S ""{0}\myproject.pj"" --user=CCNetUser --password=CCNetPassword --quiet".Replace('\\', System.IO.Path.DirectorySeparatorChar), sandboxRoot);
			mockExecutorWrapper.Setup(executor => executor.Execute(ExpectedProcessInfo(expectedResyncCommand))).Returns(new ProcessResult(null, null, 0, false)).Verifiable();
			
			string expectedAttribCommand = string.Format(@"-R /s ""{0}\*""".Replace('\\', System.IO.Path.DirectorySeparatorChar), sandboxRoot);
			mockExecutorWrapper.Setup(executor => executor.Execute(ExpectedProcessInfo("attrib", expectedAttribCommand))).Returns(new ProcessResult(null, null, 0, false)).Verifiable();
			
			string expectedDisconnectCommand = string.Format(@"disconnect --user=CCNetUser --password=CCNetPassword --quiet --forceConfirm=yes");
			ProcessInfo expectedDisconnectProcessInfo = ExpectedProcessInfo(expectedDisconnectCommand);
			mockExecutorWrapper.Setup(executor => executor.Execute(expectedDisconnectProcessInfo)).Returns(new ProcessResult(null, null, 0, false)).Verifiable();
			
			mks = CreateMks(CreateSourceControlXml(), mockHistoryParser, mockProcessExecutor);
            mks.GetSource(new IntegrationResult());
		}

		[Test]
		public void CheckpointSourceOnSuccessfulBuild()
		{
            string path = GeneratePath(@"{0}\myproject.pj".Replace('\\', System.IO.Path.DirectorySeparatorChar), sandboxRoot);
			string expectedCommand = string.Format(@"checkpoint -d ""Cruise Control.Net Build - 20"" -L ""Build - 20"" -R -S {0} --user=CCNetUser --password=CCNetPassword --quiet", path);
			ProcessInfo expectedProcessInfo = ExpectedProcessInfo(expectedCommand);
			mockExecutorWrapper.Setup(executor => executor.Execute(expectedProcessInfo)).Returns(new ProcessResult(null, null, 0, false)).Verifiable();
			
			string expectedDisconnectCommand = string.Format(@"disconnect --user=CCNetUser --password=CCNetPassword --quiet --forceConfirm=yes");
			ProcessInfo expectedDisconnectProcessInfo = ExpectedProcessInfo(expectedDisconnectCommand);
			mockExecutorWrapper.Setup(executor => executor.Execute(expectedDisconnectProcessInfo)).Returns(new ProcessResult(null, null, 0, false)).Verifiable();
			
			mockIntegrationResult.SetupGet(result => result.Succeeded).Returns(true).Verifiable();
			mockIntegrationResult.SetupGet(result => result.Label).Returns("20").Verifiable();
			
			mks = CreateMks(CreateSourceControlXml(), mockHistoryParser, mockProcessExecutor);
			mks.LabelSourceControl(integrationResult);
		}

		[Test]
		public void CheckpointSourceOnUnSuccessfulBuild()
		{
			string expectedDisconnectCommand = string.Format(@"disconnect --user=CCNetUser --password=CCNetPassword --quiet --forceConfirm=yes");
			ProcessInfo expectedDisconnectProcessInfo = ExpectedProcessInfo(expectedDisconnectCommand);
			mockExecutorWrapper.Setup(executor => executor.Execute(expectedDisconnectProcessInfo)).Returns(new ProcessResult(null, null, 0, false)).Verifiable();
			
			mockIntegrationResult.SetupGet(result => result.Succeeded).Returns(false).Verifiable();
			
			mks = CreateMks(CreateSourceControlXml(), mockHistoryParser, mockProcessExecutor);
			mks.LabelSourceControl(integrationResult);

			mockIntegrationResult.Verify();
			mockIntegrationResult.VerifyNoOtherCalls();
		}

		[Test]
		public void GetModificationsCallsParseOnHistoryParser()
		{
			mksHistoryParserWrapper.Setup(parser => parser.Parse(It.IsAny<TextReader>(), FROM, TO)).Returns(new Modification[0]).Verifiable();
			
            ProcessInfo expectedProcessInfo = ExpectedProcessInfo(string.Format(@"viewsandbox --nopersist --filter=changed:all --xmlapi -R -S {0} --user=CCNetUser --password=CCNetPassword --quiet", 
                GeneratePath(@"{0}\myproject.pj".Replace('\\', System.IO.Path.DirectorySeparatorChar), sandboxRoot)));
			mockExecutorWrapper.Setup(executor => executor.Execute(expectedProcessInfo)).Returns(new ProcessResult(null, null, 0, false)).Verifiable();
			
			string expectedDisconnectCommand = string.Format(@"disconnect --user=CCNetUser --password=CCNetPassword --quiet --forceConfirm=yes");
			ProcessInfo expectedDisconnectProcessInfo = ExpectedProcessInfo(expectedDisconnectCommand);
			mockExecutorWrapper.Setup(executor => executor.Execute(expectedDisconnectProcessInfo)).Returns(new ProcessResult(null, null, 0, false)).Verifiable();
			
			mks = CreateMks(CreateSourceControlXml(), mksHistoryParser, mockProcessExecutor);
			Modification[] modifications = mks.GetModifications(IntegrationResultMother.CreateSuccessful(FROM), IntegrationResultMother.CreateSuccessful(TO));
			Assert.AreEqual(0, modifications.Length);

			mksHistoryParserWrapper.Verify();
			mksHistoryParserWrapper.VerifyNoOtherCalls();
		}

		[Test]
		public void GetModificationsCallsParseMemberInfo()
		{
			Modification addedModification = ModificationMother.CreateModification("myFile.file", "MyFolder");
			addedModification.Type = "Added";

			mksHistoryParserWrapper.Setup(parser => parser.Parse(It.IsAny<TextReader>(), FROM, TO)).Returns(new Modification[] { addedModification }).Verifiable();
			mksHistoryParserWrapper.Setup(parser => parser.ParseMemberInfoAndAddToModification(It.IsAny<Modification>(), It.IsAny<StringReader>())).Verifiable();
			mockExecutorWrapper.Setup(executor => executor.Execute(It.IsAny<ProcessInfo>())).Returns(new ProcessResult("", null, 0, false)).Verifiable();

            string expectedCommand = string.Format(@"memberinfo --xmlapi --user=CCNetUser --password=CCNetPassword --quiet {0}", 
                GeneratePath(@"{0}\MyFolder\myFile.file".Replace('\\', System.IO.Path.DirectorySeparatorChar), sandboxRoot));
			ProcessInfo expectedProcessInfo = ExpectedProcessInfo(expectedCommand);
			mockExecutorWrapper.Setup(executor => executor.Execute(expectedProcessInfo)).Returns(new ProcessResult(null, null, 0, false)).Verifiable();
			
			string expectedDisconnectCommand = string.Format(@"disconnect --user=CCNetUser --password=CCNetPassword --quiet --forceConfirm=yes");
			ProcessInfo expectedDisconnectProcessInfo = ExpectedProcessInfo(expectedDisconnectCommand);
			mockExecutorWrapper.Setup(executor => executor.Execute(expectedDisconnectProcessInfo)).Returns(new ProcessResult(null, null, 0, false)).Verifiable();
			
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

			mksHistoryParserWrapper.Setup(parser => parser.Parse(It.IsAny<TextReader>(), FROM, TO)).Returns(new Modification[] { addedModification }).Verifiable();
			mksHistoryParserWrapper.Setup(parser => parser.ParseMemberInfoAndAddToModification(It.IsAny<Modification>(), It.IsAny<StringReader>())).Verifiable();
			mockExecutorWrapper.Setup(executor => executor.Execute(It.IsAny<ProcessInfo>())).Returns(new ProcessResult("", null, 0, false)).Verifiable();

			string expectedCommand = string.Format(@"memberinfo --xmlapi --user=CCNetUser --password=CCNetPassword --quiet {0}", 
                GeneratePath(@"{0}\myFile.file".Replace('\\', System.IO.Path.DirectorySeparatorChar), sandboxRoot));
			ProcessInfo expectedProcessInfo = ExpectedProcessInfo(expectedCommand);
			mockExecutorWrapper.Setup(executor => executor.Execute(expectedProcessInfo)).Returns(new ProcessResult(null, null, 0, false)).Verifiable();

			string expectedDisconnectCommand = string.Format(@"disconnect --user=CCNetUser --password=CCNetPassword --quiet --forceConfirm=yes");
			ProcessInfo expectedDisconnectProcessInfo = ExpectedProcessInfo(expectedDisconnectCommand);
			mockExecutorWrapper.Setup(executor => executor.Execute(expectedDisconnectProcessInfo)).Returns(new ProcessResult(null, null, 0, false)).Verifiable();
			
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

			mksHistoryParserWrapper.Setup(parser => parser.Parse(It.IsAny<TextReader>(), FROM, TO)).Returns(new Modification[] { addedModification, modifiedModification, deletedModification }).Verifiable();
			mksHistoryParserWrapper.Setup(parser => parser.ParseMemberInfoAndAddToModification(addedModification, It.IsAny<StringReader>())).Verifiable();
			mksHistoryParserWrapper.Setup(parser => parser.ParseMemberInfoAndAddToModification(modifiedModification, It.IsAny<StringReader>())).Verifiable();
            mksHistoryParserWrapper.Setup(parser => parser.ParseMemberInfoAndAddToModification(deletedModification, It.IsAny<StringReader>())).Verifiable();
			mockExecutorWrapper.Setup(executor => executor.Execute(It.IsAny<ProcessInfo>())).Returns(new ProcessResult("", null, 0, false)).Verifiable();

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
			mksHistoryParserWrapper.Setup(parser => parser.Parse(It.IsAny<TextReader>(), FROM, TO)).Returns(integrationModifications).Verifiable();
			mksHistoryParserWrapper.Setup(parser => parser.ParseMemberInfoAndAddToModification(modificationBeforePreviousIntegration, It.IsAny<StringReader>())).Verifiable();
			mksHistoryParserWrapper.Setup(parser => parser.ParseMemberInfoAndAddToModification(modificationInThisIntegration, It.IsAny<StringReader>())).Verifiable();
			mksHistoryParserWrapper.Setup(parser => parser.ParseMemberInfoAndAddToModification(modificationAfterIntegrationStartTime, It.IsAny<StringReader>())).Verifiable();
			mockExecutorWrapper.Setup(executor => executor.Execute(It.IsAny<ProcessInfo>())).Returns(new ProcessResult("", null, 0, false)).Verifiable();

			mks = CreateMks(CreateSourceControlXml(), mksHistoryParser, mockProcessExecutor);
			mks.CheckpointOnSuccess = false;
			Modification[] modifications = mks.GetModifications(IntegrationResultMother.CreateSuccessful(FROM), IntegrationResultMother.CreateSuccessful(TO));
			Assert.AreEqual(1, modifications.Length);
		}

		private static Mks CreateMks(string xml, IHistoryParser historyParser, ProcessExecutor executor)
		{
			Mks newMks = new Mks(historyParser, executor);
			NetReflector.Read(xml, newMks);
			return newMks;
		}

		private static ProcessInfo ExpectedProcessInfo(string arguments)
		{
			return ExpectedProcessInfo(System.IO.Path.Combine("..", "bin", "si.exe"), arguments);
		}

		private static ProcessInfo ExpectedProcessInfo(string executable, string arguments)
		{
			ProcessInfo expectedProcessInfo = new ProcessInfo(executable, arguments);
			expectedProcessInfo.TimeOut = Timeout.DefaultTimeout.Millis;
			return expectedProcessInfo;
		}

        /// <summary>
        /// Path generation hack to text whether the desired path contains spaces.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks>
        /// This is required because some environments contain spaces for their temp paths (e.g. WinXP), 
        /// other don't (e.g. WinVista). Previously the unit tests would fail between the different
        /// environments just because of this.
        /// </remarks>
        private string GeneratePath(string path, params string[] args)
        {
            string basePath = string.Format(path, args);
            if (basePath.Contains(" ")) basePath = "\"" + basePath + "\"";
            return basePath;                                                                      
        }
	}
}