using NUnit.Framework;
using System;
using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote.Security
{
    [TestFixture]
    public class AuditRecordTests
    {
        [Test]
        public void SetGetAllProperties()
        {
            AuditRecord record = new AuditRecord();
            record.EventType = SecurityEvent.ViewAuditLog;
            Assert.AreEqual(SecurityEvent.ViewAuditLog, record.EventType, "EventType get/set mismatch");
            record.Message = "Test Message";
            Assert.AreEqual("Test Message", record.Message, "Message get/set mismatch");
            record.ProjectName = "Test Project";
            Assert.AreEqual("Test Project", record.ProjectName, "ProjectName get/set mismatch");
            record.SecurityRight = SecurityRight.Allow;
            Assert.AreEqual(SecurityRight.Allow, record.SecurityRight, "SecurityRight get/set mismatch");
            record.TimeOfEvent = DateTime.Today;
            Assert.AreEqual(DateTime.Today, record.TimeOfEvent, "TimeOfEvent get/set mismatch");
            record.UserName = "Test User";
            Assert.AreEqual("Test User", record.UserName, "UserName get/set mismatch");
        }
    }
}
