namespace CruiseControl.Core.Tests
{
    using System.Collections.Generic;
    using System.Xaml;
    using NUnit.Framework;

    [TestFixture]
    public class AttachablePropertyStoreTests
    {
        #region Tests
        [Test]
        public void SetPropertyAddsAProperty()
        {
            var store = new AttachablePropertyStoreTest();
            var identifier = new AttachableMemberIdentifier(
                typeof(AttachablePropertyStoreTest),
                "Test");
            store.SetProperty(identifier, 123);
            Assert.AreEqual(1, store.PropertyCount);
            object value;
            Assert.IsTrue(store.TryGetProperty(identifier, out value));
            Assert.AreEqual(123, value);
        }

        [Test]
        public void RemovePropertyAddsAProperty()
        {
            var store = new AttachablePropertyStoreTest();
            var identifier = new AttachableMemberIdentifier(
                typeof(AttachablePropertyStoreTest),
                "Test");
            store.SetProperty(identifier, 123);
            Assert.IsTrue(store.RemoveProperty(identifier));
            Assert.AreEqual(0, store.PropertyCount);
            object value;
            Assert.IsFalse(store.TryGetProperty(identifier, out value));
        }

        [Test]
        public void CopyPropertiesToCopiesValues()
        {
            var store = new AttachablePropertyStoreTest();
            var identifier = new AttachableMemberIdentifier(
                typeof(AttachablePropertyStoreTest),
                "Test");
            store.SetProperty(identifier, 123);
            var array = new KeyValuePair<AttachableMemberIdentifier, object>[1];
            store.CopyPropertiesTo(array, 0);
            Assert.AreSame(identifier, array[0].Key);
        }
        #endregion

        #region Private classes
        private class AttachablePropertyStoreTest
            : AttachablePropertyStore
        { }
        #endregion
    }
}
