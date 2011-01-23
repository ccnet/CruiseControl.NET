using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.IO
{
    [TestFixture]
    public class ConditionalGetFingerprintTest
    {
        private DateTime testDate;
        private string testETag;

        [SetUp]
        public void SetUp()
        {
            testDate = new DateTime(2007, 4, 22, 22, 50, 29, DateTimeKind.Utc);
            testETag = "test e tag";
        }

        [Test]
        public void ShouldBeEqualIfDateAndETagAreEqual()
        {
            ConditionalGetFingerprint firstFingerprint = new ConditionalGetFingerprint(testDate, testETag);
            ConditionalGetFingerprint secondFingerprint = new ConditionalGetFingerprint(testDate, testETag);

            Assert.AreEqual(firstFingerprint, secondFingerprint);
            Assert.AreNotSame(firstFingerprint, secondFingerprint);
        }

		[Test]
        public void ShouldNotBeEqualIfDatesDiffer()
        {
            ConditionalGetFingerprint firstFingerprint = new ConditionalGetFingerprint(testDate, testETag);

			DateTime differentDate = new DateTime(2007, 3, 22, 22, 50, 29, DateTimeKind.Utc);
			ConditionalGetFingerprint secondFingerprint = new ConditionalGetFingerprint(differentDate, testETag);

            Assert.AreNotEqual(firstFingerprint, secondFingerprint);
        }

        [Test]
        public void ShouldNeverEqualNotAvailable()
        {
            ConditionalGetFingerprint testFingerprint = new ConditionalGetFingerprint(testDate, testETag);

            Assert.AreNotEqual(testFingerprint, ConditionalGetFingerprint.NOT_AVAILABLE);
        }

        [Test]
        public void NotAvailableNotEvenEqualToItself()
        {
            Assert.AreNotEqual(ConditionalGetFingerprint.NOT_AVAILABLE, ConditionalGetFingerprint.NOT_AVAILABLE);
            Assert.AreSame(ConditionalGetFingerprint.NOT_AVAILABLE, ConditionalGetFingerprint.NOT_AVAILABLE);
        }

        [Test]
        public void ShouldThrowExceptionIfFingerprintsAreCombinedWhichHaveDifferentETags()
        {
            ConditionalGetFingerprint testFingerprint = new ConditionalGetFingerprint(testDate, testETag);
            ConditionalGetFingerprint fingerprintWithDifferentETag= new ConditionalGetFingerprint(testDate, testETag + "different");

            Assert.That(delegate { testFingerprint.Combine(fingerprintWithDifferentETag); },
                        Throws.TypeOf<UncombinableFingerprintException>());
        }

        [Test]
        public void ShouldUseMostRecentDateWhenCombined()
        {
            DateTime olderDate = new DateTime(2006,12,1);
            DateTime recentDate = new DateTime(2007,2,1);

            ConditionalGetFingerprint olderFingerprint = new ConditionalGetFingerprint(olderDate, testETag);
            ConditionalGetFingerprint newerFingerprint = new ConditionalGetFingerprint(recentDate, testETag);

            ConditionalGetFingerprint expectedFingerprint = newerFingerprint;
            Assert.AreEqual(expectedFingerprint, olderFingerprint.Combine(newerFingerprint));
        }

        [Test]
        public void NotAvailableShouldAlwaysProduceNotAvailableWhenCombined()
        {
            ConditionalGetFingerprint testFingerprint = new ConditionalGetFingerprint(testDate, testETag);

            Assert.AreSame(ConditionalGetFingerprint.NOT_AVAILABLE, ConditionalGetFingerprint.NOT_AVAILABLE.Combine(testFingerprint));
            Assert.AreSame(ConditionalGetFingerprint.NOT_AVAILABLE, testFingerprint.Combine(ConditionalGetFingerprint.NOT_AVAILABLE));
        }
    }
}
