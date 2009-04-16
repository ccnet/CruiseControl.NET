using NUnit.Framework;
using System;
using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote.Security
{
    [TestFixture]
    public class AuditFiltersTests
    {
        private AuditRecord record1;
        private AuditRecord record2;
        private AuditRecord record3;

        [SetUp]
        public void SetUp()
        {
            record1 = new AuditRecord();
            record1.EventType = SecurityEvent.ViewAuditLog;
            record1.Message = "Message #1";
            record1.ProjectName = "Project #1";
            record1.SecurityRight = SecurityRight.Allow;
            record1.TimeOfEvent = DateTime.Today.AddDays(-1);
            record1.UserName = "User #1";

            record2 = new AuditRecord();
            record2.EventType = SecurityEvent.StopProject;
            record2.Message = "Message #2";
            record2.ProjectName = "Project #2";
            record2.SecurityRight = SecurityRight.Deny;
            record2.TimeOfEvent = DateTime.Today.AddDays(1);
            record2.UserName = "User #2";

            record3 = new AuditRecord();
            record3.EventType = SecurityEvent.StopProject;
            record3.Message = "Message #3";
            record3.ProjectName = "Project #2";
            record3.SecurityRight = SecurityRight.Deny;
            record3.TimeOfEvent = DateTime.Today.AddDays(1);
            record3.UserName = "User #1";
        }

        [Test]
        public void ByProject()
        {
            IAuditFilter filter = AuditFilters.ByProject("Project #1");
            Assert.IsTrue(filter.CheckFilter(record1), "Project not included");
            Assert.IsFalse(filter.CheckFilter(record2), "Project not excluded");
        }

        [Test]
        public void ByDateRange()
        {
            IAuditFilter filter = AuditFilters.ByDateRange(DateTime.Today.AddDays(-2), DateTime.Today);
            Assert.IsTrue(filter.CheckFilter(record1), "Date/Time not included");
            Assert.IsFalse(filter.CheckFilter(record2), "Date/Time not excluded");
        }

        [Test]
        public void ByEventType()
        {
            IAuditFilter filter = AuditFilters.ByEventType(SecurityEvent.ViewAuditLog);
            Assert.IsTrue(filter.CheckFilter(record1), "SecurityEvent not included");
            Assert.IsFalse(filter.CheckFilter(record2), "SecurityEvent not excluded");
        }

        [Test]
        public void ByRight()
        {
            IAuditFilter filter = AuditFilters.ByRight(SecurityRight.Allow);
            Assert.IsTrue(filter.CheckFilter(record1), "SecurityRight not included");
            Assert.IsFalse(filter.CheckFilter(record2), "SecurityRight not excluded");
        }

        [Test]
        public void ByUser()
        {
            IAuditFilter filter = AuditFilters.ByUser("User #1");
            Assert.IsTrue(filter.CheckFilter(record1), "User not included");
            Assert.IsFalse(filter.CheckFilter(record2), "User not excluded");
        }

        [Test]
        public void Combined()
        {
            IAuditFilter filter = AuditFilters.Combine(
                AuditFilters.ByProject("Project #1"),
                AuditFilters.ByUser("User #2"));
            Assert.IsTrue(filter.CheckFilter(record1), "Project not included");
            Assert.IsTrue(filter.CheckFilter(record2), "Project not included");
            Assert.IsFalse(filter.CheckFilter(record3), "Project not excluded");
        }

        [Test]
        public void ByProjectAndUser()
        {
            IAuditFilter filter = AuditFilters.ByProject("Project #1")
                .ByUser("User #1");
            Assert.IsTrue(filter.CheckFilter(record1), "Project not included");
            Assert.IsFalse(filter.CheckFilter(record3), "Project not excluded");
        }

        [Test]
        public void ByProjectAndDateRange()
        {
            IAuditFilter filter = AuditFilters.ByProject("Project #1")
                .ByDateRange(DateTime.Today.AddDays(-2), DateTime.Today);
            Assert.IsTrue(filter.CheckFilter(record1), "Date/Time not included");
            Assert.IsFalse(filter.CheckFilter(record3), "Date/Time not excluded");
        }

        [Test]
        public void ByProjectAndEventType()
        {
            IAuditFilter filter = AuditFilters.ByProject("Project #1")
                .ByEventType(SecurityEvent.ViewAuditLog);
            Assert.IsTrue(filter.CheckFilter(record1), "SecurityEvent not included");
            Assert.IsFalse(filter.CheckFilter(record3), "SecurityEvent not excluded");
        }

        [Test]
        public void ByProjectAndRight()
        {
            IAuditFilter filter = AuditFilters.ByProject("Project #1")
                .ByRight(SecurityRight.Allow);
            Assert.IsTrue(filter.CheckFilter(record1), "SecurityRight not included");
            Assert.IsFalse(filter.CheckFilter(record3), "SecurityRight not excluded");
        }

        [Test]
        public void ByUserAndProject()
        {
            IAuditFilter filter = AuditFilters.ByUser("User #1")
                .ByProject("Project #1");
            Assert.IsTrue(filter.CheckFilter(record1), "Project not included");
            Assert.IsFalse(filter.CheckFilter(record3), "Project not excluded");
        }
    }
}
