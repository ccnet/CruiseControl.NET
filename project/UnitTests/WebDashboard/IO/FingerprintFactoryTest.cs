using System;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.IO
{
    [TestFixture]
    public class FingerprintFactoryTest
    {
        private DynamicMock mockRequest;
        private IRequest request;

        [SetUp]
        public void SetUp()
        {
            mockRequest = new DynamicMock(typeof(IRequest));
            request = (IRequest)mockRequest.MockInstance;
        }

        [Test]
        public void ShouldReturnNotAvailableIfEitherOrBothHeadersAreMissing()
        {
            mockRequest.ExpectAndReturn("IfModifiedSince", null);
            mockRequest.ExpectAndReturn("IfNoneMatch", null);
            ConditionalGetFingerprint fingerprint = new FingerprintFactory(null, null).BuildFromRequest(request);
            Assert.AreSame(ConditionalGetFingerprint.NOT_AVAILABLE, fingerprint);


            mockRequest.ExpectAndReturn("IfModifiedSince", DateTime.Now.ToString("r"));
            mockRequest.ExpectAndReturn("IfNoneMatch", null);
            fingerprint = new FingerprintFactory(null, null).BuildFromRequest(request);
            Assert.AreSame(ConditionalGetFingerprint.NOT_AVAILABLE, fingerprint);


            mockRequest.ExpectAndReturn("IfModifiedSince", null);
            mockRequest.ExpectAndReturn("IfNoneMatch", "\"opaque value in etag\"");
            fingerprint = new FingerprintFactory(null, null).BuildFromRequest(request);
            Assert.AreSame(ConditionalGetFingerprint.NOT_AVAILABLE, fingerprint);
        }

        [Test]
        public void ShouldBuildAFingerprintWithValuesFromRequestIfBothHeadersAreAvailable()
        {
            DateTime lastModifiedDate = new DateTime(2007, 4, 20);
            string etagValue = "\"some opaque value\"";

            mockRequest.SetupResult("IfModifiedSince", lastModifiedDate.ToString("r"));
            mockRequest.SetupResult("IfNoneMatch", etagValue);
            ConditionalGetFingerprint fingerprint = new FingerprintFactory(null, null).BuildFromRequest(request);
            Assert.AreEqual(new ConditionalGetFingerprint(lastModifiedDate, etagValue), fingerprint);
        }

        [Test]
        public void ShouldFailGracefullyWithDatesFromBrowserWhichAreNotInRfc1123FormatByReturningValidButIncorrectFingerprint()
        {
            DateTime lastModifiedDate = new DateTime(2007, 4, 20);
            string etagValue = "\"some opaque value\"";

            mockRequest.SetupResult("IfModifiedSince", lastModifiedDate.ToString());
            mockRequest.SetupResult("IfNoneMatch", etagValue);
            ConditionalGetFingerprint fingerprint = new FingerprintFactory(null, null).BuildFromRequest(request);
            Assert.AreNotEqual(new ConditionalGetFingerprint(lastModifiedDate, etagValue), fingerprint);
        }

        [Test]
        public void ShouldAddQuotesToStringFromVersionAssemblyProviderForFingerprintFromDate()
        {
            string testETag = "test e tag value";
            DateTime testDate = new DateTime(2007, 4, 20);
            DynamicMock mockVersionProvider = new DynamicMock(typeof(IVersionProvider));
            mockVersionProvider.SetupResult("GetVersion", testETag);

            ConditionalGetFingerprint testConditionalGetFingerprint =
                new FingerprintFactory((IVersionProvider) mockVersionProvider.MockInstance, null).BuildFromDate(testDate);
                

            string expectedETag = "\"" + testETag + "\"";
            Assert.AreEqual(expectedETag, testConditionalGetFingerprint.ETag);
        }
    }
}