using System.Web.UI;
using System.Web.UI.HtmlControls;
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
		private IView view;
		IRequest request;

		[SetUp]
		public void Setup()
		{
			mockActionFactory = new DynamicMock(typeof(IActionFactory));
			mockRequest = new DynamicMock(typeof(IRequest));
			mockAction = new DynamicMock(typeof(IAction));
			mockView = new DynamicMock(typeof(IView));

			action = (IAction) mockAction.MockInstance;
			request = (IRequest) mockRequest.MockInstance;
			view = (IView) mockView.MockInstance;

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
		public void ShouldExecuteActionFromFactoryPuttingActionArgsOnRequestAndReturnHtml()
		{
			/// Setup
			mockActionFactory.ExpectAndReturn("Create", action, request);
			string [] actionArgs = new string[] {"foo"};
			mockActionFactory.ExpectAndReturn("ActionArguments", actionArgs, request);
			mockRequest.Expect("ActionArguments", new object[] {actionArgs});
			mockAction.ExpectAndReturn("Execute", view, request);
			mockView.ExpectAndReturn("ResponseFragment", "my html");

			/// Execute & Verify
			Assert.AreEqual("my html", controller.Do());
			VerifyAll();
		}
	}
}
