using Exortech.NetReflector;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Core.Security;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Security
{
    [TestFixture]
    public class InMemorySessionCacheTest
    {
        [Test]
        public void InitialiseDoesNothing()
        {
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
            InMemorySessionCache cache = new InMemorySessionCache();
            cache.Duration = 1;
            cache.ExpiryMode = SessionExpiryMode.Fixed;
            string sessionToken = cache.AddToCache("johndoe");
            System.Threading.Thread.Sleep(61000);
            string userName = cache.RetrieveFromCache(sessionToken);
            Assert.IsNull(userName);
        }

        [Test]
        public void SlidingExpiryTimeDoesntExpire()
        {
            InMemorySessionCache cache = new InMemorySessionCache();
            cache.Duration = 1;
            cache.ExpiryMode = SessionExpiryMode.Sliding;
            string sessionToken = cache.AddToCache("johndoe");
            System.Threading.Thread.Sleep(31000);
            string userName = cache.RetrieveFromCache(sessionToken);
            Assert.AreEqual("johndoe", userName);
            System.Threading.Thread.Sleep(31000);
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
    }
}
