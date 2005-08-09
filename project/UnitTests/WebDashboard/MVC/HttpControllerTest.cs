using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.MVC
{
	[TestFixture]
	public class RequestControllerTest
	{
		private DynamicMock mockActionFactory;
		private DynamicMock mockRequest;
		private DynamicMock mockAction;
		private DynamicMock mockView;

		private RequestController controller;
		private IAction action;
		private IResponse response;
		IRequest request;

		[SetUp]
		public void Setup()
		{
			mockActionFactory = new DynamicMock(typeof(IActionFactory));
			mockRequest = new DynamicMock(typeof(IRequest));
			mockAction = new DynamicMock(typeof(IAction));
			mockView = new DynamicMock(typeof(IResponse));

			action = (IAction) mockAction.MockInstance;
			request = (IRequest) mockRequest.MockInstance;
			response = (IResponse) mockView.MockInstance;

			controller = new RequestController((IActionFactory) mockActionFactory.MockInstance, request);
		}

		private void VerifyAll()
		{
			mockActionFactory.Verify();
			mockAction.Verify();
			mockRequest.Verify();
			mockView.Verify();
		}

		[Test]
		public void ShouldExecuteActionFromFactoryAndReturnHtml()
		{
			/// Setup
			mockActionFactory.ExpectAndReturn("Create", action, request);
			mockAction.ExpectAndReturn("Execute", response, request);

			/// Execute & Verify
			Assert.AreEqual(response, controller.Do());
			VerifyAll();
		}
	}
}
