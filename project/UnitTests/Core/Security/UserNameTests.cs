using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Core.Security;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Security
{
    [TestFixture]
    public class UserNameTests
    {
        [Test]
        public void CreateDefault()
        {
            UserName userName = new UserName();
            Assert.IsNull(userName.Name);
        }

        [Test]
        public void CreateWithName()
        {
            UserName userName = new UserName("johnDoe");
            Assert.AreEqual("johnDoe", userName.Name);
        }

        [Test]
        public void GetSetAllProperties()
        {
            string name = "johnDoe";
            UserName userName = new UserName();
            userName.Name = name;
            Assert.AreEqual(name, userName.Name, "Name does not match");
        }
    }
}
