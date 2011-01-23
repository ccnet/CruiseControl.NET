using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Core.Security;
using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Security
{
    [TestFixture]
    public class DefaultProjectAuthorisationTest
    {
        [Test]
        public void MatchingUserNameAndPermissionReturnsRight()
        {
            DefaultProjectAuthorisation authorisation = new DefaultProjectAuthorisation(SecurityRight.Deny,
                new UserPermission("johndoe", SecurityRight.Inherit, SecurityRight.Inherit, SecurityRight.Allow, SecurityRight.Inherit));
            bool result = authorisation.CheckPermission(null, "johndoe", SecurityPermission.ForceAbortBuild, SecurityRight.Inherit);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void MatchingUserNameDifferentPermissionReturnsDefault()
        {
            DefaultProjectAuthorisation authorisation = new DefaultProjectAuthorisation(SecurityRight.Deny,
                new UserPermission("johndoe", SecurityRight.Inherit, SecurityRight.Inherit, SecurityRight.Allow, SecurityRight.Inherit));
            bool result = authorisation.CheckPermission(null, "johndoe", SecurityPermission.SendMessage, SecurityRight.Inherit);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void DifferentUserNameMatchingPermissionReturnsDefault()
        {
            DefaultProjectAuthorisation authorisation = new DefaultProjectAuthorisation(SecurityRight.Deny,
                new UserPermission("johndoe", SecurityRight.Inherit, SecurityRight.Inherit, SecurityRight.Allow, SecurityRight.Inherit));
            bool result = authorisation.CheckPermission(null, "janedoe", SecurityPermission.ForceAbortBuild, SecurityRight.Inherit);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void DifferentUserNameAndPermissionReturnsDefault()
        {
            DefaultProjectAuthorisation authorisation = new DefaultProjectAuthorisation(SecurityRight.Deny,
                new UserPermission("johndoe", SecurityRight.Inherit, SecurityRight.Inherit, SecurityRight.Allow, SecurityRight.Inherit));
            bool result = authorisation.CheckPermission(null, "janedoe", SecurityPermission.SendMessage, SecurityRight.Inherit);
            Assert.AreEqual(false, result);
        }
    }
}
