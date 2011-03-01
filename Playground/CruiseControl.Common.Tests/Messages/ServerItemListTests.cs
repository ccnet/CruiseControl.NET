namespace CruiseControl.Common.Tests.Messages
{
    using CruiseControl.Common.Messages;
    using NUnit.Framework;

    [TestFixture]
    public class ServerItemListTests
    {
        #region Constructor
        [Test]
        public void DefaultConstructorInitialisesEmptyList()
        {
            var list = new ServerItemList();
            CollectionAssert.IsEmpty(list.Children);
        }

        [Test]
        public void ConstructorInitialisesListWithChildren()
        {
            var children = new[]
                               {
                                   new ServerItem(),
                                   new ServerItem()
                               };
            var list = new ServerItemList(children);
            CollectionAssert.AreEqual(children, list.Children);
        }
        #endregion
    }
}
