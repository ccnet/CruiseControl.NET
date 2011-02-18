namespace CruiseControl.Common.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class QueryArgumentsTests
    {
        #region Tests
        [Test]
        public void ConstructorSetsTheDefaultValues()
        {
            var args = new QueryArguments();
            Assert.AreEqual(DataDefinitions.Both, args.DataToInclude);
        }
        #endregion
    }
}
