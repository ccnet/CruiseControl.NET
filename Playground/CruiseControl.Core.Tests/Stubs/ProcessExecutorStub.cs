namespace CruiseControl.Core.Tests.Stubs
{
    using System;
    using System.IO;
    using CruiseControl.Core.Interfaces;
    using CruiseControl.Core.Utilities;
    using Moq;
    using NUnit.Framework;

    public class ProcessExecutorStub
        : IProcessExecutor
    {
        public ProcessExecutorStub(
            string fileName,
            string args,
            string workingDir,
            ProjectItem item,
            TaskExecutionContext context)
        {
            this.FileName = fileName;
            this.Arguments = args;
            this.WorkingDirectory = workingDir;
            this.Item = item;
            this.Context = context;
        }

        public string FileName { get; set; }
        public string Arguments { get; set; }
        public string WorkingDirectory { get; set; }
        public ProjectItem Item { get; set; }
        public TaskExecutionContext Context { get; set; }
        public bool Failed { get; set; }
        public bool TimedOut { get; set; }
        public int ExitCode { get; set; }

        public ProcessResult Execute(ProcessInfo processInfo, string projectName, string itemId, string outputFile)
        {
            throw new NotImplementedException();
        }

        public ProcessResult Execute(ProcessInfo processInfo, ProjectItem item, TaskExecutionContext context)
        {
            var outputFile = context.GeneratePathInWorkingDirectory(item.NameOrType + ".log");
            if ((this.Item == null) || (this.Context == null))
            {
                return this.Execute(processInfo,
                                    item.Project.Name,
                                    item.NameOrType,
                                    outputFile);
            }

            Assert.AreEqual(this.FileName, processInfo.FileName);
            var actual = processInfo.Arguments == null ? null : processInfo.Arguments.ToString();
            Assert.AreEqual(this.Arguments, actual);
            Assert.AreEqual(this.WorkingDirectory, processInfo.WorkingDirectory);
            Assert.AreSame(this.Item, item);
            Assert.AreEqual(this.Context, context);

            var fileSystemMock = new Mock<IFileSystem>();
            fileSystemMock.Setup(fs => fs.OpenFileForRead(outputFile)).Returns(new MemoryStream());
            var result = new ProcessResult(
                fileSystemMock.Object,
                outputFile,
                this.ExitCode,
                this.TimedOut,
                this.Failed);
            return result;
        }

        public event EventHandler<ProcessOutputEventArgs> ProcessOutput;
    }
}
