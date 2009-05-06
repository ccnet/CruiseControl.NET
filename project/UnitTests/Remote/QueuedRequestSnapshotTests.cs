using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    [TestFixture]
    public class QueuedRequestSnapshotTests
    {
        #region Test methods
        #region Properties
        [Test]
        public void ProjectNameGetSetTest()
        {
            QueuedRequestSnapshot activity = new QueuedRequestSnapshot();
            activity.ProjectName = "testing";
            Assert.AreEqual("testing", activity.ProjectName);
        }

        [Test]
        public void ActivityGetSetTest()
        {
            QueuedRequestSnapshot activity = new QueuedRequestSnapshot();
            activity.Activity = ProjectActivity.Building;
            Assert.AreEqual(ProjectActivity.Building, activity.Activity);
        }

        [Test]
        public void LastBuildDateGetSetTest()
        {
            DateTime timeNow = DateTime.Now;
            QueuedRequestSnapshot activity = new QueuedRequestSnapshot();
            activity.RequestTime = timeNow;
            Assert.AreEqual(timeNow, activity.RequestTime);
        }
        #endregion
        #endregion
    }
}
