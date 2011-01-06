namespace CruiseControl.Core.Tests.Tasks
{
    using System.Linq;
    using CruiseControl.Core.Tasks;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class CommentTests
    {
        #region Tests
        [Test]
        public void DefaultConstructorLeavesNameBlank()
        {
            var task = new Comment();
            Assert.IsNull(task.Name);
            Assert.IsNull(task.Text);
        }

        [Test]
        public void SingleArgConstructorSetsName()
        {
            var name = "commentName";
            var task = new Comment(name);
            Assert.AreEqual(name, task.Name);
            Assert.IsNull(task.Text);
        }

        [Test]
        public void DoubleArgConstructorSetsNameAndText()
        {
            var name = "commentName";
            var text = "commentText";
            var task = new Comment(name, text);
            Assert.AreEqual(name, task.Name);
            Assert.AreEqual(text, task.Text);
        }

        [Test]
        public void RunAddsComment()
        {
            using (var interceptor = LogHelper.InterceptLogging(typeof(Comment)))
            {
                var text = "commentText";
                var contextMock = new Mock<TaskExecutionContext>();
                contextMock.Setup(c => c.AddEntryToBuildLog(text)).Verifiable();
                var name = "commentName";
                var task = new Comment(name, text);
                var result = task.Run(contextMock.Object);
                result.Count(); // This is needed to actually run the task
                contextMock.Verify();
                var expected = new[]
                                   {
                                       "Info|CruiseControl.Core.Tasks.Comment|Adding comment to the build log"
                                   };
                CollectionAssert.AreEqual(expected, interceptor.Messages);
            }
        }
        #endregion
    }
}
