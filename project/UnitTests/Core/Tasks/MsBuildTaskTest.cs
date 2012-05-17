using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Exortech.NetReflector;
using NUnit.Framework;
using Rhino.Mocks;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
    [TestFixture]
    public class MsBuildTaskTest : ProcessExecutorTestFixtureBase
    {
        private const string defaultLogger = "ThoughtWorks.CruiseControl.MsBuild.dll";
        private string logfile;
        private IIntegrationResult result;
        private MsBuildTask task;
        private MockRepository mocks = new MockRepository();

        [SetUp]
        protected void SetUp()
        {
            var shadowCopier = mocks.DynamicMock<IShadowCopier>();
            SetupResult.For(shadowCopier.RetrieveFilePath(defaultLogger)).Return(defaultLogger);

            var executionEnvironment = mocks.DynamicMock<IExecutionEnvironment>();
            SetupResult.For(executionEnvironment.IsRunningOnWindows).Return(true);
            SetupResult.For(executionEnvironment.RuntimeDirectory).Return(RuntimeEnvironment.GetRuntimeDirectory());

            mocks.ReplayAll();


            CreateProcessExecutorMock(Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "MSBuild.exe"));
            task = new MsBuildTask((ProcessExecutor) mockProcessExecutor.MockInstance, executionEnvironment, shadowCopier);

            result = IntegrationResult();
            result.Label = "1.0";
            result.ArtifactDirectory = DefaultWorkingDirectory;
            result.Guid = new Guid("0123456789abcdef0123456789abcdef");
            logfile = Path.Combine(result.ArtifactDirectory, MsBuildTask.LogFilename);
            TempFileUtil.DeleteTempFile(logfile);
        }

        [TearDown]
        protected void TearDown()
        {
            Verify();
        }

        [Test]
        public void ExecuteSpecifiedProject()
        {
            string args = "/nologo /t:target1;target2 " + IntegrationProperties() + " /p:Configuration=Release myproject.sln" + GetLoggerArgument();
            ExpectToExecuteArguments(args, DefaultWorkingDirectory);

            task.ProjectFile = "myproject.sln";
            task.Targets = "target1;target2";
            task.BuildArgs = "/p:Configuration=Release";
            task.Timeout = 600;

            mocks.ReplayAll();
            task.Run(result);

            Assert.AreEqual(1, result.TaskResults.Count);
            Assert.AreEqual(IntegrationStatus.Success, result.Status);
            Assert.That(result.TaskOutput, Is.Empty);
        }

        [Test]
        public void AddQuotesAroundProjectsWithSpacesAndHandleNoSpecifiedTargets()
        {
            ExpectToExecuteArguments(@"/nologo " + IntegrationProperties() + @" ""my project.proj""" + GetLoggerArgument());
            task.ProjectFile = "my project.proj";

            mocks.ReplayAll();
            task.Run(result);
        }

        [Test]
        public void AddQuotesAroundTargetsWithSpaces()
        {
            ExpectToExecuteArguments(@"/nologo /t:first;""next task"" " + IntegrationProperties() + GetLoggerArgument());
            task.Targets = "first;next task";

            mocks.ReplayAll();
            task.Run(result);
        }

        [Test]
        public void AddQuotesAroundPropertiesWithSpaces()
        {
            // NOTE: Property names are sorted alphabetically when passed as process arguments
            // Tests that look for the correct arguments will fail if the following properties
            // are not sorted alphabetically.
            string expectedProperties = string.Join(";", new string[]
            {
                //in alphabetical order
                string.Format("/p:CCNetArtifactDirectory={0}", StringUtil.AutoDoubleQuoteString(result.ArtifactDirectory)), 
                "CCNetBuildCondition=IfModificationExists",
                string.Format("CCNetBuildDate={0}", testDateString),
                string.Format("CCNetBuildGuid={0}", testGuid),
                string.Format("CCNetBuildTime={0}", testTimeString), 
                "CCNetFailureTasks=", 
                "CCNetFailureUsers=", 
                "CCNetIntegrationStatus=Success",
                "CCNetLabel=\"My Label\"",
                "CCNetLastIntegrationStatus=Success",
                string.Format("CCNetListenerFile={0}", StringUtil.AutoDoubleQuoteString(Path.GetTempPath() + "test_ListenFile.xml")),
                "CCNetModifyingUsers=", 
                "CCNetNumericLabel=0",
                "CCNetProject=test",
                "CCNetRequestSource=foo",
                "CCNetUser=",
                string.Format("CCNetWorkingDirectory={0}", StringUtil.AutoDoubleQuoteString(DefaultWorkingDirectoryWithSpaces))
            });
            ExpectToExecuteArguments(@"/nologo " + expectedProperties + GetLoggerArgument(), DefaultWorkingDirectoryWithSpaces);
            result.Label = @"My Label";
            result.WorkingDirectory = DefaultWorkingDirectoryWithSpaces;

            mocks.ReplayAll();
            task.Run(result);
        }

        [Test]
        public void DoNotAddQuotesAroundBuildArgs()
        {
            ExpectToExecuteArguments(@"/nologo " + IntegrationProperties() + @" /noconsolelogger /p:Configuration=Debug" + GetLoggerArgument());
            task.BuildArgs = "/noconsolelogger /p:Configuration=Debug";
            mocks.ReplayAll();
            task.Run(result);			
        }

        [Test]
        public void RebaseFromWorkingDirectory()
        {
            ProcessInfo info = NewProcessInfo("/nologo " + IntegrationProperties() + GetLoggerArgument(), Path.Combine(DefaultWorkingDirectory, "src"));
            info.WorkingDirectory = Path.Combine(DefaultWorkingDirectory, "src");
            ExpectToExecute(info);
            task.WorkingDirectory = "src";

            mocks.ReplayAll();
            task.Run(result);
        }

        [Test]
        public void TimedOutExecutionShouldFailBuild()
        {
            ExpectToExecuteAndReturn(TimedOutProcessResult());
            task.Run(result);

            Assert.That(result.Status, Is.EqualTo(IntegrationStatus.Failure));
            Assert.That(result.TaskOutput, Is.StringMatching("Command line '.*' timed out after \\d+ seconds"));
        }

        [Test]
        public void ShouldAutomaticallyMergeTheBuildOutputFile()
        {
            TempFileUtil.CreateTempXmlFile(string.Format(logfile, task.LogFileId), "<output/>");
            ExpectToExecuteAndReturn(SuccessfulProcessResult());
            task.Run(result);
            Assert.AreEqual(2, result.TaskResults.Count);
            Assert.That(result.TaskOutput, Is.EqualTo("<output/>"));
            Assert.IsTrue(result.Succeeded);
        }

        [Test]
        public void ShouldFailOnFailedProcessResult()
        {
            TempFileUtil.CreateTempXmlFile(string.Format(logfile, task.LogFileId), "<output/>");
            ExpectToExecuteAndReturn(FailedProcessResult());
            task.Run(result);
            Assert.AreEqual(2, result.TaskResults.Count);
            Assert.AreEqual("<output/>" + ProcessResultOutput, result.TaskOutput);
            Assert.IsTrue(result.Failed);			
        }

        [Test]
        public void PopulateFromConfiguration()
        {
            const string xml = @"<msbuild>
	<executable>C:\WINDOWS\Microsoft.NET\Framework\v2.0.50215\MSBuild.exe</executable>
	<workingDirectory>C:\dev\ccnet</workingDirectory>
	<projectFile>CCNet.sln</projectFile>
	<buildArgs>/p:Configuration=Debug /v:diag</buildArgs>
	<targets>Build;Test</targets>
	<timeout>15</timeout>
	<logger>Kobush.Build.Logging.XmlLogger,Kobush.MSBuild.dll</logger>
    <loggerParameters>
        <loggerParameter>buildresult.xml</loggerParameter>
        <loggerParameter>someField=true</loggerParameter>
    </loggerParameters>
    <priority>BelowNormal</priority>
</msbuild>";
            task = (MsBuildTask) NetReflector.Read(xml);
            Assert.AreEqual(@"C:\WINDOWS\Microsoft.NET\Framework\v2.0.50215\MSBuild.exe", task.Executable);
            Assert.AreEqual(@"C:\dev\ccnet", task.WorkingDirectory);
            Assert.AreEqual("CCNet.sln", task.ProjectFile);
            Assert.AreEqual("Build;Test", task.Targets);
            Assert.AreEqual("/p:Configuration=Debug /v:diag", task.BuildArgs);
            Assert.AreEqual(15, task.Timeout);
            Assert.AreEqual("Kobush.Build.Logging.XmlLogger,Kobush.MSBuild.dll", task.Logger);
            Assert.AreEqual(2, task.LoggerParameters.Length);
            Assert.AreEqual("buildresult.xml", task.LoggerParameters[0]);
            Assert.AreEqual("someField=true", task.LoggerParameters[1]);
            Assert.AreEqual(ProcessPriorityClass.BelowNormal, task.Priority);
        }

        [Test]
        public void PopulateFromMinimalConfiguration()
        {
            task = (MsBuildTask) NetReflector.Read("<msbuild />");
            Assert.AreEqual(defaultExecutable, task.Executable);
            Assert.AreEqual(MsBuildTask.DefaultTimeout, task.Timeout);
            Assert.AreEqual(null, task.Logger);
        }

        [Test]
        public void PopulateFromConfigurationBogusLoggerParameters()
        {
            const string xml = @"<msbuild>
	<executable>C:\WINDOWS\Microsoft.NET\Framework\v2.0.50215\MSBuild.exe</executable>
	<workingDirectory>C:\dev\ccnet</workingDirectory>
	<projectFile>CCNet.sln</projectFile>
	<buildArgs>/p:Configuration=Debug /v:diag</buildArgs>
	<targets>Build;Test</targets>
	<timeout>15</timeout>
	<logger>Kobush.Build.Logging.XmlLogger,Kobush.MSBuild.dll;buildresult.xml</logger>
    <priority>BelowNormal</priority>
</msbuild>";
            Assert.That(delegate { task = (MsBuildTask)NetReflector.Read(xml); },
                        Throws.TypeOf<NetReflectorException>());
        }

        private string GetLoggerArgument()
        {
            var logger = string.IsNullOrEmpty(task.Logger) ? defaultLogger : task.Logger;
            return string.Format(@" /l:{0};{1}", StringUtil.AutoDoubleQuoteString(logger), StringUtil.AutoDoubleQuoteString(string.Format(logfile, task.LogFileId)));
        }

        private string IntegrationProperties()
        {
            // NOTE: Property names are sorted alphabetically when passed as process arguments
            // Tests that look for the correct arguments will fail if the following properties
            // are not sorted alphabetically.
            return string.Join(";", new string[]
            {
                //in alphabetical order
                string.Format("/p:CCNetArtifactDirectory={0}", StringUtil.AutoDoubleQuoteString(result.ArtifactDirectory)), 
                "CCNetBuildCondition=IfModificationExists",
                string.Format("CCNetBuildDate={0}", testDateString),
                string.Format("CCNetBuildGuid={0}", testGuid),
                string.Format("CCNetBuildTime={0}", testTimeString), 
                "CCNetFailureTasks=", 
                "CCNetFailureUsers=", 
                "CCNetIntegrationStatus=Success",
                "CCNetLabel=1.0",
                "CCNetLastIntegrationStatus=Success",
                string.Format("CCNetListenerFile={0}", StringUtil.AutoDoubleQuoteString(Path.GetTempPath() + "test_ListenFile.xml")),
                "CCNetModifyingUsers=", 
                "CCNetNumericLabel=0",
                "CCNetProject=test",
                "CCNetRequestSource=foo",
                "CCNetUser=",
                string.Format("CCNetWorkingDirectory={0}", StringUtil.AutoDoubleQuoteString(DefaultWorkingDirectory))
            });
        }
    }
}
