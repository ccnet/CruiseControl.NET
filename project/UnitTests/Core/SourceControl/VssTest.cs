using System;
using System.Globalization;
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
	public class VssTest : ProcessExecutorTestFixtureBase
	{
		public string DEFAULT_SS_EXE_PATH = Platform.IsWindows ? @"C:\Program Files\Microsoft Visual Studio\VSS\win32\ss.exe" : @"/Program Files/Microsoft Visual Studio/VSS/win32/ss.exe";

		private Mock<IRegistry> mockRegistry;
		private VssHistoryParser historyParser;
		private Vss vss;
		private DateTime today;
		private DateTime yesterday;

		[SetUp]
		public void SetUp()
		{
			CreateProcessExecutorMock(DEFAULT_SS_EXE_PATH);
			mockRegistry = new Mock<IRegistry>(MockBehavior.Strict);
			mockRegistry.Setup(registry => registry.GetExpectedLocalMachineSubKeyValue(It.IsAny<string>(), It.IsAny<string>())).Returns(DEFAULT_SS_EXE_PATH);
			VssLocale locale = new VssLocale(CultureInfo.InvariantCulture);
			historyParser = new VssHistoryParser(locale);

			vss = new Vss(locale, historyParser, (ProcessExecutor) mockProcessExecutor.Object, (IRegistry) mockRegistry.Object);
			vss.Project = "$/fooProject";
			vss.Culture = string.Empty; // invariant culture
			vss.Username = "Joe Admin";
			vss.Password = "admin";
			vss.WorkingDirectory = DefaultWorkingDirectory;

			today = DateTime.Now;
			yesterday = today.AddDays(-1);
		}
		
		[TearDown]
		public void TearDown()
		{
			Verify();
			mockRegistry.Verify();
		}

		// Configuration tests

		[Test]
		public void ShouldDeserialiseFromXml()
		{
			string xml =
			@"<sourceControl type=""vss"" autoGetSource=""true"">
    <executable>..\tools\vss\ss.exe</executable>
    <ssdir>..\tools\vss</ssdir>
    <project>$/root</project>
    <username>Joe Admin</username>
    <password>admin</password>
	<applyLabel>true</applyLabel>
	<timeout>5</timeout>
	<workingDirectory>C:\temp</workingDirectory>
	<culture>fr-FR</culture>
	<cleanCopy>false</cleanCopy>
	<alwaysGetLatest>true</alwaysGetLatest>
</sourceControl>";	

			NetReflector.Read(xml, vss);
			Assert.AreEqual(@"..\tools\vss\ss.exe", vss.Executable);
			Assert.AreEqual(@"admin", vss.Password.PrivateValue);
			Assert.AreEqual(@"$/root", vss.Project);
			Assert.AreEqual(@"..\tools\vss", vss.SsDir);
			Assert.AreEqual(@"Joe Admin", vss.Username);
			Assert.AreEqual(true, vss.ApplyLabel);
			Assert.AreEqual(new Timeout(5), vss.Timeout);
			Assert.AreEqual(true, vss.AutoGetSource);
			Assert.AreEqual(@"C:\temp", vss.WorkingDirectory);
			Assert.AreEqual("fr-FR", vss.Culture);
			Assert.AreEqual(false, vss.CleanCopy);
			Assert.AreEqual(true, vss.AlwaysGetLatest);
		}

		[Test]
		public void ShouldPopulateWithMinimalConfiguration()
		{
			vss = (Vss) NetReflector.Read("<vss />");
			Assert.AreEqual(Vss.DefaultProject, vss.Project);
		}
		
		[Test]
		public void StripQuotesFromSSDir()
		{
			vss.SsDir = @"""C:\Program Files\Microsoft Visual Studio\VSS""";
			Assert.AreEqual(@"C:\Program Files\Microsoft Visual Studio\VSS", vss.SsDir);
		}

		[Test]
		public void ReadDefaultExecutableFromRegistry()
		{
			mockRegistry.Setup(registry => registry.GetExpectedLocalMachineSubKeyValue(Vss.SS_REGISTRY_PATH, Vss.SS_REGISTRY_KEY)).
                Returns(Platform.IsWindows ? @"C:\Program Files\Microsoft Visual Studio\VSS\win32\SSSCC.DLL" : "/Program Files/Microsoft Visual Studio/VSS/win32/SSSCC.DLL").
                Verifiable();
			Assert.AreEqual(DEFAULT_SS_EXE_PATH, vss.Executable);
		}

		[Test]
		public void ShouldSetLocaleOnVssHistoryParserIfCultureChanges()
		{
			vss.Culture = "en-GB";
			Assert.AreEqual(new VssLocale(new CultureInfo("en-GB")), historyParser.Locale);
		}

		// GetModifications tests

        [Test]
		public void VerifyHistoryProcessInfoArguments()
		{
            string tempOutputFileName = Path.GetTempFileName();
            ExpectToExecuteArguments(string.Format(System.Globalization.CultureInfo.CurrentCulture,"history $/fooProject -R -Vd{0}~{1} \"-YJoe Admin,admin\" -I-Y {2}", 
                CommandDate(today), 
                CommandDate(yesterday), 
                GeneratePath("-O@{0}", tempOutputFileName)));
            vss.GetModifications(IntegrationResultMother.CreateSuccessful(yesterday), IntegrationResultMother.CreateSuccessful(today), tempOutputFileName);
		}

        [Test]
        public void VerifyHistoryProcessInfoArgumentsWithSpaceInProjectName()
		{
            string tempOutputFileName = Path.GetTempFileName();
            ExpectToExecuteArguments(string.Format(System.Globalization.CultureInfo.CurrentCulture,"history \"$/My Project\" -R -Vd{0}~{1} \"-YJoe Admin,admin\" -I-Y {2}", 
                CommandDate(today), 
                CommandDate(yesterday), 
                GeneratePath("-O@{0}", tempOutputFileName)));
			vss.Project = "$/My Project";
            vss.GetModifications(IntegrationResultMother.CreateSuccessful(yesterday), IntegrationResultMother.CreateSuccessful(today), tempOutputFileName);
		}

        [Test]
        public void VerifyHistoryProcessInfoArgumentsWhenUsernameIsNotSpecified()
		{
            string tempOutputFileName = Path.GetTempFileName();
            ExpectToExecuteArguments(string.Format(System.Globalization.CultureInfo.CurrentCulture,"history $/fooProject -R -Vd{0}~{1} -I-Y {2}", 
                CommandDate(today), 
                CommandDate(yesterday), 
                GeneratePath("-O@{0}", tempOutputFileName)));
			vss.Username = null;
            vss.GetModifications(IntegrationResultMother.CreateSuccessful(yesterday), IntegrationResultMother.CreateSuccessful(today), tempOutputFileName);
		}

		[Test]
		public void ShouldWorkWhenStandardErrorIsNull()
		{
			ExpectToExecuteAndReturn(new ProcessResult("foo", null, ProcessResult.SUCCESSFUL_EXIT_CODE, false));
			vss.GetModifications(IntegrationResult(yesterday), IntegrationResult(today));
		}

		[Test]
		public void ShouldWorkWhenStandardErrorIsNotNullButExitCodeIsZero()
		{
			ExpectToExecuteAndReturn(new ProcessResult("foo", "bar", ProcessResult.SUCCESSFUL_EXIT_CODE, false));
			vss.GetModifications(IntegrationResult(yesterday), IntegrationResult(today));
		}

		[Test]
		public void ShouldFailIfProcessTimesOut()
		{
			ExpectToExecuteAndReturn(new ProcessResult("x", null, ProcessResult.TIMED_OUT_EXIT_CODE, true));
            Assert.That(delegate { vss.GetModifications(IntegrationResult(yesterday), IntegrationResult(today)); },
                        Throws.TypeOf<CruiseControlException>());
		}

		// GetSource tests

		[Test]
		public void VerifyGetSourceProcessInfo()
		{
			ExpectToExecuteArguments(ForGetCommand());

			vss.AutoGetSource = true;
			vss.GetSource(IntegrationResultMother.CreateSuccessful(today));
		}

		[Test]
		public void VerifyGetSourceProcessInfoIfUsernameIsNotSpecified()
		{
			ExpectToExecuteArguments(RemoveUsername(ForGetCommand()));

			vss.AutoGetSource = true;
			vss.Username = "";
			vss.GetSource(IntegrationResultMother.CreateSuccessful(today));
		}

		private static string RemoveUsername(string cmd)
		{
			return cmd.Replace(" \"-YJoe Admin,admin\"", "");
		}

		[Test]
		public void GetSourceShouldNotGetCleanCopy()
		{
            ExpectToExecuteArguments(string.Format(System.Globalization.CultureInfo.CurrentCulture,"get $/fooProject/*?* -R -Vd{0} \"-YJoe Admin,admin\" -I-Y -W -GF- -GTM", CommandDate(today)));

			vss.AutoGetSource = true;
			vss.CleanCopy = false;
			vss.GetSource(IntegrationResultMother.CreateSuccessful(today));
		}

		[Test]
		public void DoNotGetFromDateIfAlwaysGetLatestIsTrue()
		{
			ExpectToExecuteArguments("get $/fooProject/*?* -R \"-YJoe Admin,admin\" -I-Y -W -GF- -GTM");

			vss.AutoGetSource = true;
			vss.CleanCopy = false;
			vss.AlwaysGetLatest = true;
			vss.GetSource(IntegrationResultMother.CreateSuccessful(today));
		}

		[Test]
		public void OnlyGetSourceIfAutoGetSourceIsSpecified()
		{
			vss.AutoGetSource = false;
			vss.GetSource(IntegrationResultMother.CreateSuccessful(today));
			mockProcessExecutor.VerifyNoOtherCalls();
		}

		[Test]
		public void CreateWorkingDirectoryIfItDoesNotExist()
		{
			string workingDirectory = TempFileUtil.GetTempPath("VSS");
			TempFileUtil.DeleteTempDir(workingDirectory);
			Assert.IsFalse(Directory.Exists(workingDirectory));

			ExpectToExecuteAndReturn(SuccessfulProcessResult());
			vss.AutoGetSource = true;
			vss.WorkingDirectory = workingDirectory;
			vss.GetSource(IntegrationResultMother.CreateFailed());

			Assert.IsTrue(Directory.Exists(workingDirectory));
			TempFileUtil.DeleteTempDir(workingDirectory);
		}

		[Test]
		public void RebaseRelativeWorkingDirectoryPathFromProjectWorkingDirectory()
		{
			string expectedWorkingDirectory = TempFileUtil.GetTempPath("VSS");
			ProcessInfo info = new ProcessInfo(DEFAULT_SS_EXE_PATH, ForGetCommand(), expectedWorkingDirectory);
			info.TimeOut = DefaultTimeout;
			ExpectToExecute(info);
			IntegrationResult result = IntegrationResultMother.CreateSuccessful(today);
			result.WorkingDirectory = Path.GetTempPath();

			vss.AutoGetSource = true;
			vss.WorkingDirectory = "VSS";
			vss.GetSource(result);
			Directory.Delete(expectedWorkingDirectory);
		}

		// ApplyLabel tests

		[Test]
		public void VerifyLabelProcessInfoArguments()
		{
			ExpectToExecuteArguments("label $/fooProject -LnewLabel \"-YJoe Admin,admin\" -I-Y");

			vss.ApplyLabel = true;
			vss.LabelSourceControl(IntegrationResultMother.CreateSuccessful("newLabel"));
		}

		[Test]
		public void VerifyLabelProcessInfoArgumentsWhenCreatingAndOverwritingTemporaryLabel()
		{
			ExpectToExecuteArguments("label $/fooProject -LCCNETUNVERIFIED06102005182431 \"-YJoe Admin,admin\" -I-Y");
			ExpectToExecuteArguments("label $/fooProject -LnewLabel -VLCCNETUNVERIFIED06102005182431 \"-YJoe Admin,admin\" -I-Y");

			IntegrationResult result = IntegrationResultMother.CreateSuccessful("newLabel");
			result.StartTime = new DateTime(2005, 6, 10, 18, 24, 31);

			vss.ApplyLabel = true;
			vss.AutoGetSource = false;
			vss.GetSource(result);
			vss.LabelSourceControl(result);
		}

		[Test]
		public void VerifyLabelProcessInfoArgumentsWhenCreatingAndOverwritingTemporaryLabelAndUsernameIsNotSpecified()
		{
			ExpectToExecuteArguments("label $/fooProject -LCCNETUNVERIFIED06102005182431 -I-Y");
			ExpectToExecuteArguments("label $/fooProject -LnewLabel -VLCCNETUNVERIFIED06102005182431 -I-Y");

			IntegrationResult result = IntegrationResultMother.CreateSuccessful("newLabel");
			result.StartTime = new DateTime(2005, 6, 10, 18, 24, 31);

			vss.Username = null;
			vss.ApplyLabel = true;
			vss.AutoGetSource = false;
			vss.GetSource(result);
			vss.LabelSourceControl(result);
		}

		[Test]
		public void TemporaryLabelNotAppliedByDefault()
		{
			vss.ApplyLabel = false;
			vss.AutoGetSource = false;
			vss.GetSource(IntegrationResultMother.CreateSuccessful());
			mockProcessExecutor.VerifyNoOtherCalls();
		}

		[Test]
		public void ShouldApplyTemporaryLabelBeforeGettingSource()
		{
			ExpectToExecuteArguments("label $/fooProject -LCCNETUNVERIFIED06102005182431 \"-YJoe Admin,admin\" -I-Y");

			IntegrationResult result = IntegrationResultMother.CreateSuccessful("newLabel");
			result.StartTime = new DateTime(2005, 6, 10, 18, 24, 31);

			vss.ApplyLabel = true;
			vss.AutoGetSource = false;
			vss.WorkingDirectory = DefaultWorkingDirectory;
			vss.GetSource(result);
		}

		[Test]
		public void ShouldDeleteTemporaryLabelIfIntegrationFailed()
		{
			mockProcessExecutor.Setup(executor => executor.Execute(It.IsAny<ProcessInfo>())).Returns(SuccessfulProcessResult()).Verifiable();
			ExpectToExecuteArguments("label $/fooProject -L -VLCCNETUNVERIFIED06102005182431 \"-YJoe Admin,admin\" -I-Y");

			IntegrationResult result = IntegrationResultMother.CreateFailed("foo");
			result.StartTime = new DateTime(2005, 6, 10, 18, 24, 31);
				
			vss.ApplyLabel = true;
			vss.AutoGetSource = false;
			vss.GetSource(result);
			vss.LabelSourceControl(result);
		}

		private static string CommandDate(DateTime date)
		{
			return new VssLocale(CultureInfo.InvariantCulture).FormatCommandDate(date);
		}

		private string ForGetCommand()
		{
            return string.Format(System.Globalization.CultureInfo.CurrentCulture,"get $/fooProject/*?* -R -Vd{0} \"-YJoe Admin,admin\" -I-Y -W -GF- -GTM -GWR", CommandDate(today));
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