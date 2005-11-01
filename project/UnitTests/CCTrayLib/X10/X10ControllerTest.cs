using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.CCTrayLib.X10;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Presentation;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.X10
{
	[TestFixture]
	public class X10ControllerTest
	{
		private StubProjectMonitor stubProjectMonitor;
		private DynamicMock mockX10Driver;

		[SetUp]
		public void SetUp()
		{
			stubProjectMonitor = new StubProjectMonitor("project");

			mockX10Driver = new DynamicMock(typeof(ILampController));
			mockX10Driver.Strict = true;
			
		}

		[TearDown]
		public void TearDown()
		{
			mockX10Driver.Verify();
		}
		
		[Test]
		public void SetsTheLightStatusCorrectlyBasedOnTheIntegrationStatus()
		{
			new X10Controller(stubProjectMonitor, (ILampController) mockX10Driver.MockInstance);
			
			AssertIntegrationStatusEventGeneratesAppropriateLightSwitching(IntegrationStatus.Success, false, true);
			AssertIntegrationStatusEventGeneratesAppropriateLightSwitching(IntegrationStatus.Failure, true, false);
			AssertIntegrationStatusEventGeneratesAppropriateLightSwitching(IntegrationStatus.Exception, true, false);
			AssertIntegrationStatusEventGeneratesAppropriateLightSwitching(IntegrationStatus.Unknown, true, true);
		}
		
		private void AssertIntegrationStatusEventGeneratesAppropriateLightSwitching(IntegrationStatus status, bool redLightOn, bool greenLightOn)
		{
			stubProjectMonitor.IntegrationStatus = status;
			
			mockX10Driver.Expect("RedLightOn", redLightOn);
			mockX10Driver.Expect("GreenLightOn", greenLightOn);
			
			stubProjectMonitor.OnPolled(new MonitorPolledEventArgs(stubProjectMonitor));
			
			mockX10Driver.Verify();
		}
	}
}
