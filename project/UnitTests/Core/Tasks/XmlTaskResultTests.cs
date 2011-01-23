namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
    using CruiseControl.Core.Tasks;
    using NUnit.Framework;
    using System;

    public class XmlTaskResultTests
    {
        #region Tests
        [Test]
        public void CheckIfSuccessReturnsSuccessProperty()
        {
            var result = new XmlTaskResult
                             {
                                 Success = true
                             };
            Assert.IsTrue(result.CheckIfSuccess());
        }

        [Test]
        public void DataCanBeWrittenViaTheWriter()
        {
            var result = new XmlTaskResult();
            var writer = result.GetWriter();
            writer.WriteElementString("key", "value");
            var actual = result.Data;
            var expected = "<key>value</key>";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CachedDataWillBeReturnedOnSubsequentReads()
        {
            var result = new XmlTaskResult();
            var writer = result.GetWriter();
            writer.WriteElementString("key", "value");
            var first = result.Data;
            var second = result.Data;
            Assert.AreEqual(first, second);
        }

        [Test]
        public void DataReturnsErrorIfWriterNotInitialised()
        {
            var result = new XmlTaskResult();
            string data = null;
            Assert.Throws<InvalidOperationException>(() => data = result.Data);
            Assert.IsNull(data);
        }

        [Test]
        public void WriterCannotBeStartedAfterDataHasBeenAccessed()
        {
            var result = new XmlTaskResult();
            result.GetWriter();
            var data = result.Data;
            Assert.AreEqual(string.Empty, data);
            Assert.Throws<InvalidOperationException>(() => result.GetWriter());
        }
        #endregion
    }
}
