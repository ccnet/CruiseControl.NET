using System.IO;
using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
	// TODO: verbosity?, loggers, spaces in properties

	[TestFixture]
	public class MsBuildTaskTest : ProcessExecutorTestFixtureBase
	{
		private IIntegrationResult result;
		private MsBuildTask task;

		[SetUp]
		protected void SetUp()
		{
			CreateProcessExecutorMock(MsBuildTask.DefaultExecutable);
			result = IntegrationResult();
			result.Label = "1.0";
			task = new MsBuildTask((ProcessExecutor) mockProcessExecutor.MockInstance);
		}

		[TearDown]
		protected void TearDown()
		{
			Verify();
		}

		[Test]
		public void ExecuteSpecifiedProject()
		{
			ExpectToExecuteArguments("/nologo /t:target1;target2 " + IntegrationProperties() + " /p:Configuration=Release myproject.sln");

			task.ProjectFile = "myproject.sln";
			task.Targets = "target1;target2";
			task.BuildArgs = "/p:Configuration=Release";
			task.Timeout = 600;
			task.Run(result);

			Assert.AreEqual(1, result.TaskResults.Count);
			Assert.AreEqual(IntegrationStatus.Success, result.Status);
			Assert.AreEqual(ProcessResultOutput, result.TaskOutput);
		}

		private string IntegrationProperties()
		{
			return string.Format(@"/p:ccnet.buildcondition=NoBuild;ccnet.integration.status=Success;ccnet.label=1.0;ccnet.lastintegration.status=Unknown;ccnet.project=test;ccnet.working.directory=" + DefaultWorkingDirectory);
		}

		[Test]
		public void AddQuotesAroundProjectsWithSpacesAndHandleNoSpecifiedTargets()
		{
			ExpectToExecuteArguments(@"/nologo " + IntegrationProperties() + @" ""my project.proj""");
			task.ProjectFile = "my project.proj";
			task.Run(result);
		}

		[Test]
		public void RebaseFromWorkingDirectory()
		{
			ProcessInfo info = NewProcessInfo("/nologo " + IntegrationProperties());
			info.WorkingDirectory = Path.Combine(DefaultWorkingDirectory, "src");
			ExpectToExecute(info);
			task.WorkingDirectory = "src";
			task.Run(result);
		}

		[Test]
		public void TimedOutExecutionShouldFailBuild()
		{
			ExpectToExecuteAndReturn(TimedOutProcessResult());
			task.Run(result);
			Assert.AreEqual(IntegrationStatus.Failure, result.Status);
		}

		[Test]
		public void PopulateFromConfiguration()
		{
			string xml = @"<msbuild>
	<executable>C:\WINDOWS\Microsoft.NET\Framework\v2.0.50215\MSBuild.exe</executable>
	<workingDirectory>C:\dev\ccnet</workingDirectory>
	<projectFile>CCNet.sln</projectFile>
	<buildArgs>/p:Configuration=Debug /v:diag</buildArgs>
	<targets>Build;Test</targets>
	<timeout>15</timeout>
</msbuild>";
			task = (MsBuildTask) NetReflector.Read(xml);
			Assert.AreEqual(@"C:\WINDOWS\Microsoft.NET\Framework\v2.0.50215\MSBuild.exe", task.Executable);
			Assert.AreEqual(@"C:\dev\ccnet", task.WorkingDirectory);
			Assert.AreEqual("CCNet.sln", task.ProjectFile);
			Assert.AreEqual("Build;Test", task.Targets);
			Assert.AreEqual("/p:Configuration=Debug /v:diag", task.BuildArgs);
			Assert.AreEqual(15, task.Timeout);
		}

		[Test]
		public void PopulateFromMinimalConfiguration()
		{
			task = (MsBuildTask) NetReflector.Read("<msbuild />");
			Assert.AreEqual(defaultExecutable, task.Executable);
			Assert.AreEqual(MsBuildTask.DefaultTimeout, task.Timeout);
		}
	}
}