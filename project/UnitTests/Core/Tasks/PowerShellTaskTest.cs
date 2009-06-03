using System.IO;
using Exortech.NetReflector;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;


namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
    [TestFixture]
    public class PowerShellTaskTest : ProcessExecutorTestFixtureBase
    {
        private const string POWERSHELL_PATH = @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe";
        private const string POWERSHELL1_PATH = @"C:\Windows\System32\WindowsPowerShell\v1.0";
        private const string POWERSHELL2_PATH = @"C:\Windows\System32\WindowsPowerShell\v1.0";
        private string SCRIPTS_PATH = System.Environment.GetEnvironmentVariable("USERPROFILE") + @"\Documents\WindowsPowerShell\";
       
        private PowerShellTask mytask;
        private IMock mockRegistry;

        [SetUp]
        public void Setup()
        {
            mockRegistry = new DynamicMock(typeof(IRegistry));
            CreateProcessExecutorMock(POWERSHELL1_PATH);
            mytask = new PowerShellTask((IRegistry)mockRegistry.MockInstance, (ProcessExecutor)mockProcessExecutor.MockInstance);
        }

        [TearDown]
        public void VerifyMocks()
        {
            mockRegistry.Verify();
            mockProcessExecutor.Verify();
        }

        [Test]
        public void ShouldLoadAllValuesFromConfiguration()
        {
            string xml = @"
<powershell>
    <script>testscript.ps1</script>    
	<executable>c:\powershell\powershell.exe</executable>
	<scriptsDirectory>D:\CruiseControl</scriptsDirectory>
	<buildArgs>-noprofile</buildArgs>
	<buildTimeoutSeconds>4</buildTimeoutSeconds>
	<successExitCodes>1</successExitCodes>
	<environment>
        <variable name=""Env1"" value=""value1""/>
        <variable name=""Env2"" value=""value2""/>
   </environment>
</powershell>";

            PowerShellTask task = (PowerShellTask)NetReflector.Read(xml);
            Assert.AreEqual(@"c:\powershell\powershell.exe", task.Executable);
            Assert.AreEqual(@"testscript.ps1", task.Script);
            Assert.AreEqual(@"D:\CruiseControl", task.ConfiguredScriptsDirectory);
            Assert.AreEqual(@"-noprofile", task.BuildArgs);
            Assert.AreEqual(4, task.BuildTimeoutSeconds);
            Assert.AreEqual("1", task.SuccessExitCodes);
            Assert.AreEqual("value1", task.EnvironmentVariables[0].value);
            Assert.AreEqual("Env1", task.EnvironmentVariables[0].name);
            Assert.AreEqual("value2", task.EnvironmentVariables[1].value);
            Assert.AreEqual("Env2", task.EnvironmentVariables[1].name);
         }

        [Test]
        public void ShouldLoadMinimalValuesFromConfiguration()
        {
            string xml = @"<powershell script=""myScript.ps1"" />";
            PowerShellTask task = (PowerShellTask)NetReflector.Read(xml);

            // Need to override the default registry search, otherwise this fails on a machine that does not have PowerShell installed
            var registryMock2 = new DynamicMock(typeof(IRegistry));
            task.Registry = (IRegistry)registryMock2.MockInstance;
            registryMock2.ExpectAndReturn("GetLocalMachineSubKeyValue",
                @"C:\Windows\System32\WindowsPowerShell\v1.0",
                @"SOFTWARE\Microsoft\PowerShell\2\PowerShellEngine",
                @"ApplicationBase");

            Assert.AreEqual(@"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe", task.Executable);
            Assert.AreEqual(@"myScript.ps1", task.Script);
            Assert.AreEqual(PowerShellTask.DefaultScriptsDirectory, task.ConfiguredScriptsDirectory);
            Assert.AreEqual(PowerShellTask.DefaultBuildTimeOut, task.BuildTimeoutSeconds);
            Assert.AreEqual(string.Empty, task.SuccessExitCodes);
            Assert.AreEqual(0, task.EnvironmentVariables.Length, "Checking environment variable array size.");
        }

        [Test]
        public void DefaultPowerShellShouldBe1IfNothingNewerInstalled()
        {
            IMock mockRegistry2 = new DynamicMock(typeof(IRegistry));

            PowerShellTask task = new PowerShellTask((IRegistry)mockRegistry2.MockInstance, (ProcessExecutor)mockProcessExecutor.MockInstance);
            mockRegistry2.ExpectAndReturn("GetLocalMachineSubKeyValue", null, PowerShellTask.regkeypowershell2, PowerShellTask.regkeyholder);
            mockRegistry2.ExpectAndReturn("GetLocalMachineSubKeyValue", POWERSHELL1_PATH,
                                         PowerShellTask.regkeypowershell1, PowerShellTask.regkeyholder);
            Assert.AreEqual(POWERSHELL1_PATH + "\\powershell.exe", task.Executable);
            mockRegistry2.Verify();
            mockProcessExecutor.Verify();
        }  

        [Test]
        public void DefaultPowerShellShouldBe2IfInstalled()
        {
            IMock mockRegistry2 = new DynamicMock(typeof(IRegistry));

            PowerShellTask task = new PowerShellTask((IRegistry)mockRegistry2.MockInstance, (ProcessExecutor)mockProcessExecutor.MockInstance);
            mockRegistry2.ExpectAndReturn("GetLocalMachineSubKeyValue", POWERSHELL2_PATH, 
                                        PowerShellTask.regkeypowershell2,PowerShellTask.regkeyholder);
            Assert.AreEqual(POWERSHELL2_PATH + "\\powershell.exe", task.Executable);
            mockRegistry2.Verify();
            mockProcessExecutor.Verify();
        }           

        [Test]
        public void VerifyPowerShellProcessInfoBasic()
        {
            CollectingConstraint constraint = new CollectingConstraint();
            ProcessResult processResult = new ProcessResult("output", "error", 0, false);
            mockProcessExecutor.ExpectAndReturn("Execute", processResult, new object[] { constraint });
            mytask.Executable = POWERSHELL_PATH;
            mytask.Script = "MyScipt.ps1";
           
            mytask.Run(IntegrationResult());

            ProcessInfo info = (ProcessInfo)constraint.Parameter;
            Assert.AreEqual(POWERSHELL_PATH, info.FileName);
            Assert.AreEqual(PowerShellTask.DefaultBuildTimeOut * 1000, info.TimeOut);
            CustomAssertion.AssertContains(mytask.Script, info.Arguments);
        }

        [Test]
        public void VerifyPowerShellProcessInfoWithScriptsDirectoryConfigured()
        {
            CollectingConstraint constraint = new CollectingConstraint();
            ProcessResult processResult = new ProcessResult("output", "error", 0, false);
            mockProcessExecutor.ExpectAndReturn("Execute", processResult, new object[] { constraint });
            mytask.Executable = POWERSHELL_PATH;
            mytask.Script = "MyScript.ps1";
            mytask.ConfiguredScriptsDirectory = @"D:\CruiseControl";

            mytask.Run(IntegrationResult());

            ProcessInfo info = (ProcessInfo)constraint.Parameter;
            Assert.AreEqual(POWERSHELL_PATH, info.FileName);
            Assert.AreEqual(PowerShellTask.DefaultBuildTimeOut * 1000, info.TimeOut);
            CustomAssertion.AssertStartsWith(@"D:\CruiseControl\MyScript.ps1", info.Arguments);
        }

        [Test]
        public void ShouldSetOutputAndIntegrationStatusToSuccessOnSuccessfulBuild()
        {
            ProcessResult processResult = new ProcessResult(" ", string.Empty, ProcessResult.SUCCESSFUL_EXIT_CODE, false);
            mockProcessExecutor.ExpectAndReturn("Execute", processResult, new object[] { new IsAnything() });
            mytask.Executable = POWERSHELL_PATH;
            mytask.Script = "MyScript.ps1";

            IntegrationResult result = (IntegrationResult)IntegrationResult();
            mytask.Run(result);

            Assert.AreEqual(IntegrationStatus.Success, result.Status);
            CustomAssertion.AssertMatches(" ", result.TaskOutput);
        }

        [Test]
        public void ShouldSetOutputAndIntegrationStatusToFailedOnFailedBuild()
        {
            ProcessResult processResult = new ProcessResult(@"Documents\WindowsPowerShell\MyScript.ps1' is not recognized as a cmdlet", string.Empty, 1, false);
            CollectingConstraint constraint = new CollectingConstraint();
            mockProcessExecutor.ExpectAndReturn("Execute", processResult, new object[] { constraint });

            mytask.Executable = POWERSHELL_PATH;
            mytask.Script = "MyScript.ps1";
            mytask.ConfiguredScriptsDirectory = @"D:\CruiseControl";

            IIntegrationResult result = Integration("myProject", @"D:\CruiseControl", "myArtifactDirectory");
            mytask.Run(result);

            ProcessInfo info = (ProcessInfo)constraint.Parameter;
            Assert.AreEqual(@"D:\CruiseControl", info.WorkingDirectory);

            Assert.AreEqual(IntegrationStatus.Failure, result.Status);
            CustomAssertion.AssertMatches(@"(\.|\n)*is not recognized as a cmdlet", result.TaskOutput);
        }

        [Test, ExpectedException(typeof(BuilderException))]
        public void ShouldThrowBuilderExceptionIfProcessExecutorThrowsAnException()
        {
            mockProcessExecutor.ExpectAndThrow("Execute", new IOException(), new object[] { new IsAnything() });
            mytask.Executable = POWERSHELL_PATH;
            mytask.Script = "MyScript.ps1";

            mytask.Run(IntegrationResult());
        }

        [Test, ExpectedException(typeof(BuilderException))]
        public void ShouldThrowBuilderExceptionIfProcessExecutorThrowsAnExceptionUsingUnkownProject()
        {
            mockProcessExecutor.ExpectAndThrow("Execute", new IOException(), new object[] { new IsAnything() });
            mytask.Executable = POWERSHELL_PATH;
            mytask.Script = "MyScript.ps1";

            mytask.Run(IntegrationResult());
        }

        [Test, ExpectedException(typeof(BuilderException))]
        public void ShouldThrowBuilderExceptionIfProcessTimesOut()
        {
            ProcessResult processResult = new ProcessResult(string.Empty, string.Empty, ProcessResult.TIMED_OUT_EXIT_CODE, true);
            mockProcessExecutor.ExpectAndReturn("Execute", processResult, new object[] { new IsAnything() });
            mytask.BuildTimeoutSeconds = 2;
            mytask.Executable = POWERSHELL_PATH;
            mytask.Script = "MyScript.ps1";

            mytask.Run(IntegrationResult());
        }

    }
}
