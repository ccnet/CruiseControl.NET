using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Core.Security;
using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Security
{
    [TestFixture]
    public class UserPermissionTests
    {
        [Test]
        public void MatchingUserNameReturnsTrue()
        {
            UserPermission assertion = new UserPermission("johndoe", SecurityRight.Inherit, SecurityRight.Inherit, SecurityRight.Allow, SecurityRight.Inherit);
            bool result = assertion.CheckUser(null, "johndoe");
            Assert.IsTrue(result);
        }

        [Test]
        public void DifferentUserNameReturnsFalse()
        {
            UserPermission assertion = new UserPermission("johndoe", SecurityRight.Inherit, SecurityRight.Inherit, SecurityRight.Allow, SecurityRight.Inherit);
            bool result = assertion.CheckUser(null, "janedoe");
            Assert.IsFalse(result);
        }

        [Test]
        public void MatchingPermissionReturnsRight()
        {
            UserPermission assertion = new UserPermission("johndoe", SecurityRight.Inherit, SecurityRight.Inherit, SecurityRight.Allow, SecurityRight.Inherit);
            SecurityRight result = assertion.CheckPermission(null, SecurityPermission.ForceAbortBuild);
            Assert.AreEqual(SecurityRight.Allow, result);
        }

        [Test]
        public void DifferentPermissionReturnsInherited()
        {
            UserPermission assertion = new UserPermission("johndoe", SecurityRight.Inherit, SecurityRight.Inherit, SecurityRight.Allow, SecurityRight.Inherit);
            SecurityRight result = assertion.CheckPermission(null, SecurityPermission.SendMessage);
            Assert.AreEqual(SecurityRight.Inherit, result);
        }

        [Test]
        public void GetSetAllProperties()
        {
            string userName = "johndoe";
            UserPermission assertion = new UserPermission();
            assertion.UserName = userName;
            Assert.AreEqual(userName, assertion.UserName, "UserName not correctly set");
            Assert.AreEqual(userName, assertion.Identifier, "Identifier not correctly set");

            assertion.DefaultRight = SecurityRight.Deny;
            Assert.AreEqual(SecurityRight.Deny, assertion.DefaultRight, "DefaultRight not correctly set");
            assertion.ForceBuildRight = SecurityRight.Deny;
            Assert.AreEqual(SecurityRight.Deny, assertion.ForceBuildRight, "ForceBuildRight not correctly set");
            assertion.SendMessageRight = SecurityRight.Deny;
            Assert.AreEqual(SecurityRight.Deny, assertion.SendMessageRight, "SendMessageRight not correctly set");
            assertion.StartProjectRight = SecurityRight.Deny;
            Assert.AreEqual(SecurityRight.Deny, assertion.StartProjectRight, "StartProjectRight not correctly set");

            assertion.RefId = "A reference";
            Assert.AreEqual("A reference", assertion.RefId, "RefId not correctly set");
        }
    }
}
