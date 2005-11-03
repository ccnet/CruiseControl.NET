using System;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.CCTrayLib.X10;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Presentation;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.X10
{
	[TestFixture]
	public class X10ControllerTest
	{
		private StubProjectMonitor stubProjectMonitor;
		private StubCurrentTimeProvider stubCurrentTimeProvider;
		private X10Configuration configuration;
		private DynamicMock mockX10Driver;

		[SetUp]
		public void SetUp()
		{
			stubProjectMonitor = new StubProjectMonitor("project");

			mockX10Driver = new DynamicMock(typeof(ILampController));
			mockX10Driver.Strict = true;
			
			configuration = new X10Configuration();
			configuration.Enabled = true;
			configuration.StartTime = DateTime.Parse("08:00");
			configuration.EndTime = DateTime.Parse("18:00");
			configuration.StartDay = DayOfWeek.Monday;
			configuration.EndDay = DayOfWeek.Friday;
			
			stubCurrentTimeProvider = new StubCurrentTimeProvider();
			stubCurrentTimeProvider.SetNow(new DateTime(2005, 11, 03, 12, 00, 00));
			Assert.AreEqual(DayOfWeek.Thursday, stubCurrentTimeProvider.Now.DayOfWeek);

			new X10Controller(
				stubProjectMonitor, 
				(ILampController) mockX10Driver.MockInstance,
				stubCurrentTimeProvider, 
				configuration);
		}

		[TearDown]
		public void TearDown()
		{
			mockX10Driver.Verify();
		}
		
		[Test]
		public void SetsTheLightStatusCorrectlyBasedOnTheIntegrationStatus()
		{
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
		
		[Test]
		public void WhenTheCurrentTimeIsOutsideTheAvailableHoursBothLightsAreSwitchedOff()
		{
			stubCurrentTimeProvider.SetNow(new DateTime(2005, 11, 05, 12, 00, 00));
			Assert.AreEqual(DayOfWeek.Saturday, stubCurrentTimeProvider.Now.DayOfWeek);
			AssertLightsAreSwitchedOffRegardlessOfIntegrationStateOutsideOfConfiguredHours();

			stubCurrentTimeProvider.SetNow(new DateTime(2005, 11, 05, 05, 00, 00));
			AssertLightsAreSwitchedOffRegardlessOfIntegrationStateOutsideOfConfiguredHours();

			stubCurrentTimeProvider.SetNow(new DateTime(2005, 11, 03, 05, 00, 00));
			Assert.AreEqual(DayOfWeek.Thursday, stubCurrentTimeProvider.Now.DayOfWeek);
			AssertLightsAreSwitchedOffRegardlessOfIntegrationStateOutsideOfConfiguredHours();

			stubCurrentTimeProvider.SetNow(new DateTime(2005, 11, 03, 20, 00, 00));
			AssertLightsAreSwitchedOffRegardlessOfIntegrationStateOutsideOfConfiguredHours();

		}

		private void AssertLightsAreSwitchedOffRegardlessOfIntegrationStateOutsideOfConfiguredHours()
		{
			AssertIntegrationStatusEventGeneratesAppropriateLightSwitching(IntegrationStatus.Success, false, false);
			AssertIntegrationStatusEventGeneratesAppropriateLightSwitching(IntegrationStatus.Failure, false, false);
			AssertIntegrationStatusEventGeneratesAppropriateLightSwitching(IntegrationStatus.Exception, false, false);
			AssertIntegrationStatusEventGeneratesAppropriateLightSwitching(IntegrationStatus.Unknown, false, false);
		}
	}
}
