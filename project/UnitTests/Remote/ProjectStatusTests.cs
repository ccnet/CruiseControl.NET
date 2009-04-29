using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    [TestFixture]
    public class ProjectStatusTests
    {
        #region Test methods
        #region Properties
        [Test]
        public void BuildStageGetSetTest()
        {
            ProjectStatus activity = new ProjectStatus();
            activity.BuildStage = "testing";
            Assert.AreEqual("testing", activity.BuildStage);
        }

        [Test]
        public void StatusGetSetTest()
        {
            ProjectStatus activity = new ProjectStatus();
            activity.Status = ProjectIntegratorState.Running;
            Assert.AreEqual(ProjectIntegratorState.Running, activity.Status);
        }

        [Test]
        public void BuildStatusGetSetTest()
        {
            ProjectStatus activity = new ProjectStatus();
            activity.BuildStatus = IntegrationStatus.Cancelled;
            Assert.AreEqual(IntegrationStatus.Cancelled, activity.BuildStatus);
        }

        [Test]
        public void ActivityGetSetTest()
        {
            ProjectStatus activity = new ProjectStatus();
            activity.Activity = ProjectActivity.Building;
            Assert.AreEqual(ProjectActivity.Building, activity.Activity);
        }

        [Test]
        public void NameGetSetTest()
        {
            ProjectStatus activity = new ProjectStatus();
            activity.Name = "testing";
            Assert.AreEqual("testing", activity.Name);
        }

        [Test]
        public void CategoryGetSetTest()
        {
            ProjectStatus activity = new ProjectStatus();
            activity.Category = "testing";
            Assert.AreEqual("testing", activity.Category);
        }

        [Test]
        public void WebURLGetSetTest()
        {
            ProjectStatus activity = new ProjectStatus();
            activity.WebURL = "testing";
            Assert.AreEqual("testing", activity.WebURL);
        }

        [Test]
        public void LastBuildLabelGetSetTest()
        {
            ProjectStatus activity = new ProjectStatus();
            activity.LastBuildLabel = "testing";
            Assert.AreEqual("testing", activity.LastBuildLabel);
        }

        [Test]
        public void LastSuccessfulBuildLabelGetSetTest()
        {
            ProjectStatus activity = new ProjectStatus();
            activity.LastSuccessfulBuildLabel = "testing";
            Assert.AreEqual("testing", activity.LastSuccessfulBuildLabel);
        }

        [Test]
        public void LastBuildDateGetSetTest()
        {
            DateTime timeNow = DateTime.Now;
            ProjectStatus activity = new ProjectStatus();
            activity.LastBuildDate = timeNow;
            Assert.AreEqual(timeNow, activity.LastBuildDate);
        }

        [Test]
        public void NextBuildTimeGetSetTest()
        {
            DateTime timeNow = DateTime.Now;
            ProjectStatus activity = new ProjectStatus();
            activity.NextBuildTime = timeNow;
            Assert.AreEqual(timeNow, activity.NextBuildTime);
        }
        #endregion
        #endregion
    }
}
