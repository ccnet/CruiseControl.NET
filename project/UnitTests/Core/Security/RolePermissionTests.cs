using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Core.Security;
using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Security
{
    [TestFixture]
    public class RolePermissionTests
    {
        [Test]
        public void UserNameInRole()
        {
            RolePermission assertion = new RolePermission("testrole", SecurityRight.Inherit, SecurityRight.Inherit, SecurityRight.Allow, SecurityRight.Inherit, new UserName("johndoe"));
            bool result = assertion.CheckUser(null, "johndoe");
            Assert.IsTrue(result);
        }

        [Test]
        public void UserNameNotInRole()
        {
            RolePermission assertion = new RolePermission("testrole", SecurityRight.Inherit, SecurityRight.Inherit, SecurityRight.Allow, SecurityRight.Inherit, new UserName("johndoe"));
            bool result = assertion.CheckUser(null, "janedoe");
            Assert.IsFalse(result);
        }

        [Test]
        public void MatchingPermissionReturnsRight()
        {
            RolePermission assertion = new RolePermission("testrole", SecurityRight.Inherit, SecurityRight.Inherit, SecurityRight.Allow, SecurityRight.Inherit, new UserName("johndoe"));
            SecurityRight result = assertion.CheckPermission(null, SecurityPermission.ForceAbortBuild);
            Assert.AreEqual(SecurityRight.Allow, result);
        }

        [Test]
        public void DifferentPermissionReturnsInherited()
        {
            RolePermission assertion = new RolePermission("testrole", SecurityRight.Inherit, SecurityRight.Inherit, SecurityRight.Allow, SecurityRight.Inherit, new UserName("johndoe"));
            SecurityRight result = assertion.CheckPermission(null, SecurityPermission.SendMessage);
            Assert.AreEqual(SecurityRight.Inherit, result);
        }

        [Test]
        public void GetSetAllProperties()
        {
            string userName = "testrole";
            RolePermission assertion = new RolePermission();
            assertion.RoleName = userName;
            Assert.AreEqual(userName, assertion.RoleName, "RoleName not correctly set");
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
            assertion.Users = new UserName[0];
            Assert.AreEqual(0, assertion.Users.Length, "Users not correctly set - empty array");
            assertion.Users = new UserName[] { new UserName("JohnDoe") };
            Assert.AreEqual(1, assertion.Users.Length, "Users not correctly set - array with data");
        }
    }
}
