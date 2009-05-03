using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Security;
using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Security
{
    [TestFixture]
    public class PermissionBaseTests
    {
        private MockRepository mocks = new MockRepository();

        [Test]
        [ExpectedException(typeof(BadReferenceException), ExpectedMessage="Reference 'doesNotExist' is either incorrect or missing.")]
        public void CheckUserWithInvalidReference()
        {
            string userName = "johndoe";
            string badReference = "doesNotExist";
            ISecurityManager manager = mocks.StrictMock<ISecurityManager>();
            Expect.Call(manager.RetrievePermission(badReference)).Return(null);

            mocks.ReplayAll();
            UserPermission assertion = new UserPermission();
            assertion.RefId = badReference;
            assertion.CheckUser(manager, userName);
        }

        [Test]
        public void CheckUserWithValidReference()
        {
            string userName = "johndoe";
            string goodReference = "doesExist";
            IPermission goodAssertion = mocks.StrictMock<IPermission>();
            ISecurityManager manager = mocks.StrictMock<ISecurityManager>();
            Expect.Call(manager.RetrievePermission(goodReference)).Return(goodAssertion);
            Expect.Call(goodAssertion.CheckUser(manager, userName)).Return(true);

            mocks.ReplayAll();
            UserPermission assertion = new UserPermission();
            assertion.RefId = goodReference;
            bool result = assertion.CheckUser(manager, userName);
            Assert.IsTrue(result);
            mocks.VerifyAll();
        }

        [Test]
        [ExpectedException(typeof(BadReferenceException), ExpectedMessage = "Reference 'doesNotExist' is either incorrect or missing.")]
        public void CheckPermissionWithInvalidReference()
        {
            SecurityPermission permission = SecurityPermission.ForceBuild;
            string badReference = "doesNotExist";
            ISecurityManager manager = mocks.StrictMock<ISecurityManager>();
            Expect.Call(manager.RetrievePermission(badReference)).Return(null);

            mocks.ReplayAll();
            UserPermission assertion = new UserPermission();
            assertion.RefId = badReference;
            assertion.CheckPermission(manager, permission);
        }

        [Test]
        public void CheckPermissionWithValidReference()
        {
            SecurityPermission permission = SecurityPermission.ForceBuild;
            string goodReference = "doesExist";
            IPermission goodAssertion = mocks.StrictMock<IPermission>();
            ISecurityManager manager = mocks.StrictMock<ISecurityManager>();
            Expect.Call(manager.RetrievePermission(goodReference)).Return(goodAssertion);
            Expect.Call(goodAssertion.CheckPermission(manager, permission)).Return(SecurityRight.Allow);

            mocks.ReplayAll();
            UserPermission assertion = new UserPermission();
            assertion.RefId = goodReference;
            SecurityRight result = assertion.CheckPermission(manager, permission);
            Assert.AreEqual(SecurityRight.Allow, result);
            mocks.VerifyAll();
        }

        [Test]
        public void CorrectPermissionsReturnedForceBuild()
        {
            UserPermission assertion = new UserPermission("johnDoe",
                SecurityRight.Deny,
                SecurityRight.Deny,
                SecurityRight.Allow,
                SecurityRight.Deny);
            SecurityRight right = assertion.CheckPermission(null, SecurityPermission.ForceBuild);
            Assert.AreEqual(SecurityRight.Allow, right);
        }

        [Test]
        public void CorrectPermissionsReturnedSendMessage()
        {
            UserPermission assertion = new UserPermission("johnDoe",
                SecurityRight.Deny,
                SecurityRight.Allow,
                SecurityRight.Deny,
                SecurityRight.Deny);
            SecurityRight right = assertion.CheckPermission(null, SecurityPermission.SendMessage);
            Assert.AreEqual(SecurityRight.Allow, right);
        }

        [Test]
        public void CorrectPermissionsReturnedStartProject()
        {
            UserPermission assertion = new UserPermission("johnDoe",
                SecurityRight.Deny,
                SecurityRight.Deny,
                SecurityRight.Deny,
                SecurityRight.Allow);
            SecurityRight right = assertion.CheckPermission(null, SecurityPermission.StartProject);
            Assert.AreEqual(SecurityRight.Allow, right);
        }
    }
}
