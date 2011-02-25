namespace CruiseControl.Core.Tests.Utilities
{
    using CruiseControl.Core.Utilities;
    using NUnit.Framework;

    public class ProcessOutputEventArgsTests
    {
        #region Tests
        [Test]
        public void ConstructorSetsProperties()
        {
            var data = "Some Data";
            var outputType = ProcessOutputType.ErrorOutput;
            var args = new ProcessOutputEventArgs(outputType, data);
            Assert.AreEqual(outputType, args.OutputType);
            Assert.AreEqual(data, args.Data);
        }
        #endregion
    }
}
