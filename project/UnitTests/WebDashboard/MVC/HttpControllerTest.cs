using System.Web.UI;
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
		private RequestController controller;
		private IAction action;
		IRequest request;

		[SetUp]
		public void Setup()
		{
			mockActionFactory = new DynamicMock(typeof(IActionFactory));
			mockRequest = new DynamicMock(typeof(IRequest));
			mockAction = new DynamicMock(typeof(IAction));

			action = (IAction) mockAction.MockInstance;

			request = (IRequest) mockRequest.MockInstance;
			controller = new RequestController((IActionFactory) mockActionFactory.MockInstance, request);
		}

		private void VerifyAll()
		{
			mockActionFactory.Verify();
			mockAction.Verify();
			mockRequest.Verify();
		}

		[Test]
		public void RunsActionFromFactoryAndPutsResultInParentControl()
		{
			/// Setup
			mockActionFactory.ExpectAndReturn("Create", action, request);
			Control actionResult = new Control();
			mockAction.ExpectAndReturn("Execute", actionResult, request);
			Control topLevelControl = new Control();

			/// Execute
			controller.Do(topLevelControl);

			/// Verify
			Assert.IsTrue(topLevelControl.Controls.Contains(actionResult));
			VerifyAll();
		}
	}
}
