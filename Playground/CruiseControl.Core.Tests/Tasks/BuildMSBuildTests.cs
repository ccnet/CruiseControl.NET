namespace CruiseControl.Core.Tests.Tasks
{
    using System;
    using System.Linq;
    using CruiseControl.Core.Tasks;
    using CruiseControl.Core.Tests.Stubs;
    using NUnit.Framework;

    [TestFixture]
    public class BuildMSBuildTests
    {
        #region Tests
        [Test]
        public void RunWithDefaultParametersCallExecutor()
        {
            var projectFile = "project";
            var task = new BuildMSBuild
                           {
                               Project = new Project("Test"),
                               ProjectFile = projectFile
                           };
            var context = TaskExecutionContextHelpers.Initialise(task.Project);
            task.ProcessExecutor = new ProcessExecutorStub(
                "msbuild",
                projectFile,
                Environment.CurrentDirectory,
                task,
                context);

            var result = task.Run(context);
            result.Count(); // This is needed to actually run the task
            Assert.AreEqual(IntegrationStatus.Success, context.CurrentStatus);
        }

        [Test]
        public void RunHandlesExecutorFailing()
        {
            var projectFile = "project";
            var task = new BuildMSBuild
                           {
                               Project = new Project("Test"),
                               ProjectFile = projectFile
                           };
            var context = TaskExecutionContextHelpers.Initialise(task.Project);
            var executor = new ProcessExecutorStub(
                "msbuild",
                projectFile,
                Environment.CurrentDirectory,
                task,
                context) { Failed = true };
            task.ProcessExecutor = executor;

            var result = task.Run(context);
            result.Count(); // This is needed to actually run the task
            Assert.AreEqual(IntegrationStatus.Failure, context.CurrentStatus);
        }
        #endregion
    }
}
