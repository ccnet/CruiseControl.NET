using Exortech.NetReflector;
using NUnit.Framework;
using System;
using ThoughtWorks.CruiseControl.Core.Security;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Security
{
    [TestFixture]
    public class InMemorySessionCacheTest
    {
        [Test]
        public void InitialiseDoesNothing()
        {
            InMemorySessionCache cache = new InMemorySessionCache();
            cache.Initialise();
        }

        [Test]
        public void AddToCacheReturnsGuid()
        {
            InMemorySessionCache cache = new InMemorySessionCache();
            string sessionToken = cache.AddToCache("johndoe");
            string userName = cache.RetrieveFromCache(sessionToken);
            Guid sessionGuid = new Guid(sessionToken);
            Assert.AreEqual("johndoe", userName);
        }

        [Test]
        public void RemoveFromCacheRemovesSession()
        {
            InMemorySessionCache cache = new InMemorySessionCache();
            string sessionToken = cache.AddToCache("johndoe");
            cache.RemoveFromCache(sessionToken);
            string userName = cache.RetrieveFromCache(sessionToken);
            Assert.IsNull(userName);
        }

        [Test]
        public void FixedExpiryTimeExpires()
        {
            TestClock clock = new TestClock {Now = DateTime.Now};
            InMemorySessionCache cache = new InMemorySessionCache(clock);
            cache.Duration = 1;
            cache.ExpiryMode = SessionExpiryMode.Fixed;
            string sessionToken = cache.AddToCache("johndoe");
            clock.TimePasses(TimeSpan.FromSeconds(61));
            string userName = cache.RetrieveFromCache(sessionToken);
            Assert.IsNull(userName);
        }

        [Test]
        public void SlidingExpiryTimeDoesntExpire()
        {
            TestClock clock = new TestClock {Now = DateTime.Now};
            InMemorySessionCache cache = new InMemorySessionCache(clock);
            cache.Duration = 1;
            cache.ExpiryMode = SessionExpiryMode.Sliding;
            string sessionToken = cache.AddToCache("johndoe");
            clock.TimePasses(TimeSpan.FromSeconds(31));
            string userName = cache.RetrieveFromCache(sessionToken);
            Assert.AreEqual("johndoe", userName);
            clock.TimePasses(TimeSpan.FromSeconds(31));
            userName = cache.RetrieveFromCache(sessionToken);
            Assert.AreEqual("johndoe", userName);
        }

        [Test]
        public void LoadsFromXml()
        {
		    NetReflectorTypeTable typeTable = new NetReflectorTypeTable();
            typeTable.Add(AppDomain.CurrentDomain);
            NetReflectorReader reader = new NetReflectorReader(typeTable);

            object result = reader.Read("<inMemoryCache duration=\"5\" mode=\"Fixed\"/>");
            Assert.IsInstanceOfType(typeof(InMemorySessionCache), result);
            InMemorySessionCache cache = result as InMemorySessionCache;
            Assert.AreEqual(5, cache.Duration);
            Assert.AreEqual(SessionExpiryMode.Fixed, cache.ExpiryMode);
        }

        [Test]
        public void StoreSessionValueIsStored()
        {
            InMemorySessionCache cache = new InMemorySessionCache();
            string sessionToken = cache.AddToCache("johndoe");
            string key = "An item";
            object value = Guid.NewGuid();

            cache.StoreSessionValue(sessionToken, key, value);
            object result = cache.RetrieveSessionValue(sessionToken, key);
            Assert.AreEqual(value, result);
        }

        [Test]
        public void NonStoredValueReturnsNull()
        {
            InMemorySessionCache cache = new InMemorySessionCache();
            string sessionToken = cache.AddToCache("johndoe");
            string key = "An item";

            object result = cache.RetrieveSessionValue(sessionToken, key);
            Assert.IsNull(result);
        }

        [Test]
        public void InvalidSessionValueReturnsNull()
        {
            InMemorySessionCache cache = new InMemorySessionCache();
            string sessionToken = "Non-existant";
            string key = "An item";

            object result = cache.RetrieveSessionValue(sessionToken, key);
            Assert.IsNull(result);
        }
    }
}
