using System;
using System.Web.UI;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Config;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.MVC.Cruise
{
	[TestFixture]
	public class SecurityCheckingProxyActionTest : Assertion
	{
		private DynamicMock proxiedActionMock;
		private DynamicMock configurationGetterMock;
		private SecurityCheckingProxyAction checkingAction;

		IRequest request = new NameValueCollectionRequest(null);

		[SetUp]
		public void Setup()
		{
			proxiedActionMock = new DynamicMock(typeof(IAction));
			configurationGetterMock = new DynamicMock(typeof(IConfigurationGetter));
			checkingAction = new SecurityCheckingProxyAction((IAction) proxiedActionMock.MockInstance, (IConfigurationGetter) configurationGetterMock.MockInstance);
		}

		private void VerifyAll()
		{
			proxiedActionMock.Verify();
			configurationGetterMock.Verify();
		}

		[Test]
		public void ShouldProxyIfSecureActionsAllowed()
		{
			// Setup
			Control view = new Control();
			configurationGetterMock.ExpectAndReturn("GetSimpleConfigSetting", "true", "AllowSecureActions");
			proxiedActionMock.ExpectAndReturn("Execute", view, request);
	
			// Execute
			Control returnedView = checkingAction.Execute(request);

			// Verify
			AssertEquals(view, returnedView);
			VerifyAll();
		}

		[Test]
		public void ShouldNotProxyIfSecureActionsNotAllowed()
		{
			// Setup
			Control view = new Control();
			configurationGetterMock.ExpectAndReturn("GetSimpleConfigSetting", "false", "AllowSecureActions");
			proxiedActionMock.ExpectNoCall("Execute", typeof(IRequest));
	
			// Execute
			Control returnedView = checkingAction.Execute(request);

			// Verify
			Assert(view != returnedView);
			AssertNotNull(returnedView);
			VerifyAll();
		}

		[Test]
		public void ShouldNotProxyIfSecureActionsNotConfigured()
		{
			// Setup
			Control view = new Control();
			configurationGetterMock.ExpectAndReturn("GetSimpleConfigSetting", null, "AllowSecureActions");
			proxiedActionMock.ExpectNoCall("Execute", typeof(IRequest));
	
			// Execute
			Control returnedView = checkingAction.Execute(request);

			// Verify
			Assert(view != returnedView);
			AssertNotNull(returnedView);
			VerifyAll();
		}
	}
}
