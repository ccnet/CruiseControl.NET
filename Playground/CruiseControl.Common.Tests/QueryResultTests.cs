namespace CruiseControl.Common.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class QueryResultTests
    {
        #region Tests
        [Test]
        public void ConstructorSetsTheDefaults()
        {
            var result = new QueryResult();
            Assert.AreEqual(RemoteResultCode.Success, result.ResultCode);
        }
        #endregion
    }
}
