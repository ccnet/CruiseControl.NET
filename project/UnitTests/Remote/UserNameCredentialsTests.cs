using NUnit.Framework;
using System;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    /// <summary>
    /// Unit tests for UserNameCredentials. 
    /// </summary>
    [TestFixture]
    public class UserNameCredentialsTests
    {
        [Test]
        public void CreateDefault()
        {
            TestHelpers.EnsureLanguageIsValid();
            UserNameCredentials credentials = new UserNameCredentials();
            Assert.IsNull(credentials.UserName);
            Assert.IsNull(credentials.Identifier);
        }

        [Test]
        public void CreateWithUserName()
        {
            TestHelpers.EnsureLanguageIsValid();
            string name = "JohnDoe";
            UserNameCredentials credentials = new UserNameCredentials(name);
            Assert.AreEqual(name, credentials.UserName);
            Assert.AreEqual(name, credentials.Identifier);
        }

        [Test]
        public void SetAndRetrieveUserName()
        {
            TestHelpers.EnsureLanguageIsValid();
            string name = "JohnDoe";
            UserNameCredentials credentials = new UserNameCredentials();
            Assert.IsNull(credentials.UserName);
            credentials.UserName = name;
            Assert.AreEqual(name, credentials.UserName);
        }

        [Test]
        public void SetAndRetrieveValue()
        {
            TestHelpers.EnsureLanguageIsValid();
            string value1 = "Something";
            string value2 = "Something Else";
            UserNameCredentials credentials = new UserNameCredentials();
            Assert.IsNull(credentials["Value"]);
            credentials["Value"] = value1;
            Assert.AreEqual(value1, credentials["Value"]);
            credentials["Value"] = value2;
            Assert.AreEqual(value2, credentials["Value"]);
            credentials["Value"] = null;
            Assert.IsNull(credentials["Value"]);
        }

        [Test]
        public void PassThroughSerialisation()
        {
            TestHelpers.EnsureLanguageIsValid();
            string name = "JohnDoe";
            string value1 = "Something";
            string value2 = null;
            UserNameCredentials credentials = new UserNameCredentials(name);
            credentials["Value1"] = value1;
            credentials["Value2"] = value2;
            object result = TestHelpers.RunSerialisationTest(credentials);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(typeof(UserNameCredentials), result);
            Assert.AreEqual(name, (result as UserNameCredentials).UserName);
            Assert.AreEqual(value1, (result as UserNameCredentials)["Value1"]);
            Assert.AreEqual(value2, (result as UserNameCredentials)["Value2"]);
        }

        [Test]
        public void ListAllCredentials()
        {
            string[] values = { "Value1", "Value2" };
            UserNameCredentials credentials = new UserNameCredentials();
            foreach (string value in values)
            {
                credentials[value] = value;
            }
            string[] allValues = credentials.Credentials;

            string[] expected = { "username", "value1", "value2" };
            Assert.AreEqual(expected.Length, allValues.Length);
            for (int loop = 0; loop < expected.Length; loop++)
            {
                Assert.AreEqual(expected[loop], allValues[loop], string.Format("Value at index {0} does not match", loop));
            }
        }
    }
}
