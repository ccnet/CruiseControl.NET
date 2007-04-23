using System;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.MVC
{
	[TestFixture]
	public class RequestControllerTest
	{
		private DynamicMock mockActionFactory;
		private DynamicMock mockRequest;
		private DynamicMock mockAction;
        private DynamicMock mockFingerprintFactory;
        private DynamicMock mockFingerprintableContentProvider;

		private RequestController controller;
		private IAction action;
		private IResponse response;
		private IRequest request;
	    private IFingerprintFactory fingerprintFactory;
	    private IConditionalGetFingerprintProvider fingerprintableContentProvider;

	    [SetUp]
		public void Setup()
		{
			mockActionFactory = new DynamicMock(typeof(IActionFactory));
			mockRequest = new DynamicMock(typeof(IRequest));
			mockAction = new DynamicMock(typeof(IAction));
            mockFingerprintFactory = new DynamicMock(typeof(IFingerprintFactory));
	        mockFingerprintableContentProvider = new DynamicMock(typeof (IConditionalGetFingerprintProvider));

			action = (IAction) mockAction.MockInstance;
			request = (IRequest) mockRequest.MockInstance;
            response = new HtmlFragmentResponse("<b>test</b>");
            fingerprintFactory = (IFingerprintFactory)mockFingerprintFactory.MockInstance;
	        fingerprintableContentProvider =
	            (IConditionalGetFingerprintProvider) mockFingerprintableContentProvider.MockInstance;

			controller = new RequestController((IActionFactory) mockActionFactory.MockInstance, request, fingerprintFactory);
		}

		private void VerifyAll()
		{
			mockActionFactory.Verify();
			mockAction.Verify();
			mockRequest.Verify();
            mockFingerprintFactory.Verify();
		    mockFingerprintableContentProvider.Verify();
		}

		[Test]
		public void ShouldExecuteActionFromFactoryAndReturnHtml()
		{
			/// Setup
			mockActionFactory.ExpectAndReturn("Create", action, request);
			mockAction.ExpectAndReturn("Execute", response, request);
            mockFingerprintFactory.SetupResult("BuildFromRequest", ConditionalGetFingerprint.NOT_AVAILABLE, typeof(IRequest));

			/// Execute & Verify
			Assert.AreEqual(response, controller.Do());
			VerifyAll();
		}

        [Test]
        public void ShouldReturnNormalResponseWithNotAvailableFingerprintIfActionDoesNotHaveFingerPrintProvider()
        {
            mockActionFactory.ExpectAndReturn("CreateFingerprintProvider", null, request);
            mockActionFactory.ExpectAndReturn("Create", action, request);

            mockAction.ExpectAndReturn("Execute", response, request);

            IResponse actualResponse = controller.Do();

            Assert.IsAssignableFrom(typeof(HtmlFragmentResponse), actualResponse);
            Assert.AreSame(ConditionalGetFingerprint.NOT_AVAILABLE, actualResponse.ServerFingerprint);
        }

        [Test]
        public void ShouldReturnNormalResponseWithServerProvidedFingerprintIfClientDidNotProvideFingerprint()
        {
            mockActionFactory.ExpectAndReturn("CreateFingerprintProvider", fingerprintableContentProvider, request);
            mockFingerprintFactory.ExpectAndReturn("BuildFromRequest", ConditionalGetFingerprint.NOT_AVAILABLE, request);
            mockActionFactory.ExpectAndReturn("Create", action, request);

            mockAction.ExpectAndReturn("Execute", response, request);
            ConditionalGetFingerprint fingerprint = new ConditionalGetFingerprint(DateTime.Now, "test token");
            mockFingerprintableContentProvider.ExpectAndReturn("GetFingerprint", fingerprint, request);

            IResponse actualResponse = controller.Do();

            Assert.IsAssignableFrom(typeof(HtmlFragmentResponse), actualResponse);
            Assert.AreEqual(fingerprint, actualResponse.ServerFingerprint);
        }

	    [Test]
	    public void ShouldReturnNotModifiedResponseIfClientFingerprintMatchesServerFingerprint()
	    {
            ConditionalGetFingerprint sharedFingerprint = new ConditionalGetFingerprint(new DateTime(1980, 10, 30), "None-Match-String");

            mockActionFactory.ExpectAndReturn("CreateFingerprintProvider", fingerprintableContentProvider, request);
            mockFingerprintFactory.ExpectAndReturn("BuildFromRequest", sharedFingerprint, request);
            mockActionFactory.ExpectAndReturn("Create", action, request);

            mockAction.ExpectAndReturn("Execute", response, request);
            mockFingerprintableContentProvider.ExpectAndReturn("GetFingerprint", sharedFingerprint, request);

            IResponse actualResponse = controller.Do();

            Assert.IsAssignableFrom(typeof(NotModifiedResponse), actualResponse);
            Assert.AreEqual(sharedFingerprint, actualResponse.ServerFingerprint);
	    }
	}
}
