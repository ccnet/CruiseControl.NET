namespace CruiseControl.Core.Tests.Tasks
{
    using System.Linq;
    using CruiseControl.Core.Tasks;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class MergeFilesTests
    {
        #region Tests
        [Test]
        public void RunImportsFiles()
        {
            var file = "C:\\somefile.txt";
            var task = new MergeFiles(new MergeFile(file));
            var contextMock = new Mock<TaskExecutionContext>(
                MockBehavior.Strict,
                new TaskExecutionParameters());
            contextMock.Setup(ec => ec.ImportFile(file, false));
            task.Run(contextMock.Object).Count();
            contextMock.Verify();
        }
        #endregion
    }
}
