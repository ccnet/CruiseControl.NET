using System;
using Exortech.NetReflector;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Triggers;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Triggers
{
	[TestFixture]
	public class UrlTriggerTest : IntegrationFixture
	{
		private Mock<DateTimeProvider> mockDateTime;
		private Mock<HttpWrapper> mockHttpWrapper;
		private UrlTrigger trigger;
		private DateTime initialDateTimeNow;
		private const string DefaultUrl = @"http://confluence.public.thoughtworks.org/";

		[SetUp]
		public void Setup()
		{
			Source = "UrlTrigger";
			initialDateTimeNow = new DateTime(2002, 1, 2, 3, 0, 0, 0);
			mockDateTime = new Mock<DateTimeProvider>();
			mockDateTime.SetupGet(provider => provider.Now).Returns(this.initialDateTimeNow);
			mockHttpWrapper = new Mock<HttpWrapper>();

			trigger = new UrlTrigger((DateTimeProvider) mockDateTime.Object, (HttpWrapper) mockHttpWrapper.Object);
			trigger.Url = DefaultUrl;
		}

		public void VerifyAll()
		{
			mockDateTime.Verify();
			mockHttpWrapper.Verify();
		}

		[Test]
		public void ShouldFullyPopulateFromReflector()
		{
			string xml = string.Format(@"<urlTrigger name=""url"" seconds=""1"" buildCondition=""ForceBuild"" url=""{0}"" />", DefaultUrl);
			trigger = (UrlTrigger) NetReflector.Read(xml);
            Assert.AreEqual(1, trigger.IntervalSeconds, "trigger.IntervalSeconds");
            Assert.AreEqual(BuildCondition.ForceBuild, trigger.BuildCondition, "trigger.BuildCondition");
            Assert.AreEqual(DefaultUrl, trigger.Url, "trigger.Url");
            Assert.AreEqual("url", trigger.Name, "trigger.Name");
		}

		[Test]
		public void ShouldDefaultPopulateFromReflector()
		{
			string xml = string.Format(@"<urlTrigger url=""{0}"" />", DefaultUrl);
			trigger = (UrlTrigger) NetReflector.Read(xml);
            Assert.AreEqual(UrlTrigger.DefaultIntervalSeconds, trigger.IntervalSeconds, "trigger.IntervalSeconds");
            Assert.AreEqual(BuildCondition.IfModificationExists, trigger.BuildCondition, "trigger.BuildCondition");
            Assert.AreEqual(DefaultUrl, trigger.Url, "trigger.Url");
			Assert.AreEqual("UrlTrigger", trigger.Name, "trigger.Name");
		}

		[Test]
		public void ShouldNotBuildFirstTime()
		{
            Assert.IsNull(trigger.Fire(), "trigger.Fire()");
			VerifyAll();
		}


        [Test]
        public void ShouldBuildAfterFirstInterval()
        {
            mockDateTime.SetupGet(provider => provider.Now).Returns(initialDateTimeNow.AddSeconds(trigger.IntervalSeconds));
            mockHttpWrapper.Setup(http => http.GetLastModifiedTimeFor(new Uri(DefaultUrl), DateTime.MinValue)).Returns(initialDateTimeNow).Verifiable();
            Assert.AreEqual(ModificationExistRequest(), trigger.Fire(), "trigger.Fire()");
            VerifyAll();
        }

        [Test]
		public void ShouldNotBuildIfUrlHasNotChanged()
		{
            // Initial check, no build
			mockHttpWrapper.Setup(http => http.GetLastModifiedTimeFor(new Uri(DefaultUrl), DateTime.MinValue)).Returns(initialDateTimeNow).Verifiable();
            Assert.IsNull(trigger.Fire(), "trigger.Fire()");
			trigger.IntegrationCompleted();

            // First interval passes, initial build because url date/time is unknown
            mockDateTime.SetupGet(provider => provider.Now).Returns(initialDateTimeNow.AddSeconds(trigger.IntervalSeconds));
            Assert.AreEqual(ModificationExistRequest(), trigger.Fire(), "trigger.Fire()");

            // Second interval passes, no build because url has not changed
			mockDateTime.SetupGet(provider => provider.Now).Returns(initialDateTimeNow.AddSeconds(trigger.IntervalSeconds * 2));
			mockHttpWrapper.Setup(http => http.GetLastModifiedTimeFor(new Uri(DefaultUrl), initialDateTimeNow)).Returns(initialDateTimeNow).Verifiable();
            Assert.IsNull(trigger.Fire(), "trigger.Fire()");
            Assert.AreEqual(initialDateTimeNow.AddSeconds(trigger.IntervalSeconds * 3), trigger.NextBuild, "trigger.NextBuild");		// Next build should be at third interval
			VerifyAll();
		}

		[Test]
		public void ShouldHandleExceptionAccessingUrl()
		{
			mockHttpWrapper.Setup(http => http.GetLastModifiedTimeFor(new Uri(DefaultUrl), DateTime.MinValue)).Throws(new Exception("Uh-oh")).Verifiable();
            Assert.IsNull(trigger.Fire(), "trigger.Fire()");
		}
	}
}