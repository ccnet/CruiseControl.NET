using System;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Presentation;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.CCTrayLib.X10;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.X10
{
	[TestFixture]
	public class X10ControllerTest
	{
		private StubProjectMonitor stubProjectMonitor;
		private StubCurrentTimeProvider stubCurrentTimeProvider;
		private X10Configuration configuration;
		private Mock<ILampController> mockLampController;

		[SetUp]
		public void SetUp()
		{
			stubProjectMonitor = new StubProjectMonitor("project");

			mockLampController = new Mock<ILampController>(MockBehavior.Strict);
			ILampController lampController = mockLampController.Object as ILampController;
			
			configuration = new X10Configuration();
			configuration.Enabled = true;
			configuration.StartTime = DateTime.Parse("08:00");
			configuration.EndTime = DateTime.Parse("18:00");
            configuration.ActiveDays[(int)DayOfWeek.Sunday] = false;
            configuration.ActiveDays[(int)DayOfWeek.Monday] = true;
            configuration.ActiveDays[(int)DayOfWeek.Tuesday] = true;
            configuration.ActiveDays[(int)DayOfWeek.Wednesday] = true;
            configuration.ActiveDays[(int)DayOfWeek.Thursday] = true;
            configuration.ActiveDays[(int)DayOfWeek.Friday] = true;
            configuration.ActiveDays[(int)DayOfWeek.Saturday] = false;
			
			stubCurrentTimeProvider = new StubCurrentTimeProvider();
			stubCurrentTimeProvider.SetNow(new DateTime(2005, 11, 03, 12, 00, 00));
			Assert.AreEqual(DayOfWeek.Thursday, stubCurrentTimeProvider.Now.DayOfWeek);

			new X10Controller(
				stubProjectMonitor, 
				stubCurrentTimeProvider, 
				configuration,
				lampController);
		}

		[Test]
        [Platform(Exclude = "Mono", Reason = "No X display available")]
		public void SetsTheLightStatusCorrectlyBasedOnTheIntegrationStatus()
		{
			// for each set of conditions (Integration Status + Project State),
			// set the lights (Red, Yellow, Green)
			AssertPollingGeneratesAppropriateLights(IntegrationStatus.Success, ProjectState.Building, false, true, true);
			AssertPollingGeneratesAppropriateLights(IntegrationStatus.Failure, ProjectState.Building, true, true, false);
			AssertPollingGeneratesAppropriateLights(IntegrationStatus.Exception, ProjectState.Building, true, true, false);
			// If we can't get status, turn all lights off. 
			AssertPollingGeneratesAppropriateLights(IntegrationStatus.Unknown, ProjectState.Building, false, false, false);
		
			// this next case seems to be degenerate - can this ever happen?
			AssertPollingGeneratesAppropriateLights(IntegrationStatus.Success, ProjectState.Broken, true, false, false);

			AssertPollingGeneratesAppropriateLights(IntegrationStatus.Failure, ProjectState.Broken, true, false, false);
			AssertPollingGeneratesAppropriateLights(IntegrationStatus.Exception, ProjectState.Broken, true, false, false);
			AssertPollingGeneratesAppropriateLights(IntegrationStatus.Unknown, ProjectState.Broken, false, false, false);

			AssertPollingGeneratesAppropriateLights(IntegrationStatus.Success, ProjectState.BrokenAndBuilding, true, true, false);
			AssertPollingGeneratesAppropriateLights(IntegrationStatus.Failure, ProjectState.BrokenAndBuilding, true, true, false);
			AssertPollingGeneratesAppropriateLights(IntegrationStatus.Exception, ProjectState.BrokenAndBuilding, true, true, false);
			AssertPollingGeneratesAppropriateLights(IntegrationStatus.Unknown, ProjectState.BrokenAndBuilding, false, false, false);
		
			AssertPollingGeneratesAppropriateLights(IntegrationStatus.Success, ProjectState.NotConnected, false, false, false);
			AssertPollingGeneratesAppropriateLights(IntegrationStatus.Failure, ProjectState.NotConnected, false, false, false);
			AssertPollingGeneratesAppropriateLights(IntegrationStatus.Exception, ProjectState.NotConnected, false, false, false);
			AssertPollingGeneratesAppropriateLights(IntegrationStatus.Unknown, ProjectState.NotConnected, false, false, false);

			AssertPollingGeneratesAppropriateLights(IntegrationStatus.Success, ProjectState.Success, false, false, true);
			AssertPollingGeneratesAppropriateLights(IntegrationStatus.Failure, ProjectState.Success, true, false, false);
			AssertPollingGeneratesAppropriateLights(IntegrationStatus.Exception, ProjectState.Success, true, false, false);
			AssertPollingGeneratesAppropriateLights(IntegrationStatus.Unknown, ProjectState.Success, false, false, false);
		}

		private void AssertPollingGeneratesAppropriateLights(IntegrationStatus status, ProjectState state, bool redLightOn, bool yellowLightOn, bool greenLightOn)
		{
			stubProjectMonitor.IntegrationStatus = status;
			stubProjectMonitor.ProjectState = state;
			
			mockLampController.SetupSet(controller => controller.RedLightOn = redLightOn).Verifiable();
			mockLampController.SetupSet(controller => controller.YellowLightOn = yellowLightOn).Verifiable();
			mockLampController.SetupSet(controller => controller.GreenLightOn = greenLightOn).Verifiable();
			
			stubProjectMonitor.OnPolled(new MonitorPolledEventArgs(stubProjectMonitor));
			
			mockLampController.Verify();
		}
		
		[Test]
        [Platform(Exclude = "Mono", Reason = "No X display available")]
		public void WhenTheCurrentTimeIsOutsideTheAvailableHoursAllLightsAreSwitchedOff()
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
			AssertPollingGeneratesAppropriateLights(IntegrationStatus.Success, ProjectState.Success, false, false, false);
			AssertPollingGeneratesAppropriateLights(IntegrationStatus.Failure, ProjectState.Success, false, false, false);
			AssertPollingGeneratesAppropriateLights(IntegrationStatus.Exception, ProjectState.Success, false, false, false);
			AssertPollingGeneratesAppropriateLights(IntegrationStatus.Unknown, ProjectState.Success, false, false, false);
		}
	}
}
