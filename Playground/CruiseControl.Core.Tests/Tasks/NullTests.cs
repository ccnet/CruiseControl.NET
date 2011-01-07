namespace CruiseControl.Core.Tests.Tasks
{
    using System.Linq;
    using CruiseControl.Core.Tasks;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class NullTests
    {
        #region Tests
        [Test]
        public void RunAddsToLog()
        {
            using (var intercept = LogHelper.InterceptLogging(typeof(Null)))
            {
                var task = new Null();
                var contextMock = new Mock<TaskExecutionContext>(null, null, null, null);
                var tasks = task.Run(contextMock.Object);
                tasks.Count();
                var expected = new[]
                                   {
                                       "Info|CruiseControl.Core.Tasks.Null|Doing nothing"
                                   };
                CollectionAssert.AreEqual(expected, intercept.Messages);
            }
        }
        #endregion
    }
}
