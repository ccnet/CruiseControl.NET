using System;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.MVC
{
	[TestFixture]
	public class RequestControllerTest
	{
		private Mock<IActionFactory> mockActionFactory;
		private Mock<IRequest> mockRequest;
		private Mock<IAction> mockAction;
        private Mock<IFingerprintFactory> mockFingerprintFactory;
        private Mock<IConditionalGetFingerprintProvider> mockFingerprintableContentProvider;

		private RequestController controller;
		private IAction action;
		private IResponse response;
		private IRequest request;
	    private IFingerprintFactory fingerprintFactory;
	    private IConditionalGetFingerprintProvider fingerprintableContentProvider;

	    [SetUp]
		public void Setup()
		{
			mockActionFactory = new Mock<IActionFactory>();
			mockRequest = new Mock<IRequest>();
			mockAction = new Mock<IAction>();
            mockFingerprintFactory = new Mock<IFingerprintFactory>();
	        mockFingerprintableContentProvider = new Mock<IConditionalGetFingerprintProvider>();

			action = (IAction) mockAction.Object;
			request = (IRequest) mockRequest.Object;
            response = new HtmlFragmentResponse("<b>test</b>");
            fingerprintFactory = (IFingerprintFactory)mockFingerprintFactory.Object;
	        fingerprintableContentProvider =
	            (IConditionalGetFingerprintProvider) mockFingerprintableContentProvider.Object;

			controller = new RequestController((IActionFactory) mockActionFactory.Object, request, fingerprintFactory);
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
			mockActionFactory.Setup(factory => factory.Create(request)).Returns(action).Verifiable();
			mockAction.Setup(action => action.Execute(request)).Returns(response).Verifiable();
            mockFingerprintFactory.Setup(factory => factory.BuildFromRequest(It.IsAny<IRequest>())).Returns(ConditionalGetFingerprint.NOT_AVAILABLE).Verifiable();

			/// Execute & Verify
			Assert.AreEqual(response, controller.Do());
			VerifyAll();
		}

        [Test]
        public void ShouldReturnNormalResponseWithNotAvailableFingerprintIfActionDoesNotHaveFingerPrintProvider()
        {
            mockActionFactory.Setup(factory => factory.CreateFingerprintProvider(request)).Returns(() => null).Verifiable();
            mockActionFactory.Setup(factory => factory.Create(request)).Returns(action).Verifiable();

            mockAction.Setup(action => action.Execute(request)).Returns(response).Verifiable();

            IResponse actualResponse = controller.Do();

            Assert.IsAssignableFrom(typeof(HtmlFragmentResponse), actualResponse);
            Assert.AreSame(ConditionalGetFingerprint.NOT_AVAILABLE, actualResponse.ServerFingerprint);
        }

        [Test]
        public void ShouldReturnNormalResponseWithServerProvidedFingerprintIfClientDidNotProvideFingerprint()
        {
            mockActionFactory.Setup(factory => factory.CreateFingerprintProvider(request)).Returns(fingerprintableContentProvider).Verifiable();
            mockFingerprintFactory.Setup(factory => factory.BuildFromRequest(request)).Returns(ConditionalGetFingerprint.NOT_AVAILABLE).Verifiable();
            mockActionFactory.Setup(factory => factory.Create(request)).Returns(action).Verifiable();

            mockAction.Setup(action => action.Execute(request)).Returns(response).Verifiable();
            ConditionalGetFingerprint fingerprint = new ConditionalGetFingerprint(DateTime.Now, "test token");
            mockFingerprintableContentProvider.Setup(provider => provider.GetFingerprint(request)).Returns(fingerprint).Verifiable();

            IResponse actualResponse = controller.Do();

            Assert.IsAssignableFrom(typeof(HtmlFragmentResponse), actualResponse);
            Assert.AreEqual(fingerprint, actualResponse.ServerFingerprint);
        }

	    [Test]
	    public void ShouldReturnNotModifiedResponseIfClientFingerprintMatchesServerFingerprint()
	    {
            ConditionalGetFingerprint sharedFingerprint = new ConditionalGetFingerprint(new DateTime(1980, 10, 30), "None-Match-String");

            mockActionFactory.Setup(factory => factory.CreateFingerprintProvider(request)).Returns(fingerprintableContentProvider).Verifiable();
            mockFingerprintFactory.Setup(factory => factory.BuildFromRequest(request)).Returns(sharedFingerprint).Verifiable();
            mockActionFactory.Setup(factory => factory.Create(request)).Returns(action).Verifiable();

            mockAction.Setup(action => action.Execute(request)).Returns(response).Verifiable();
            mockFingerprintableContentProvider.Setup(provider => provider.GetFingerprint(request)).Returns(sharedFingerprint).Verifiable();

            IResponse actualResponse = controller.Do();

            Assert.IsAssignableFrom(typeof(NotModifiedResponse), actualResponse);
            Assert.AreEqual(sharedFingerprint, actualResponse.ServerFingerprint);
	    }
	}
}
