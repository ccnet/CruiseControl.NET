using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.X10;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.X10
{
	[TestFixture]
	public class LampControllerTest
	{
		private LampController lampController;
		private Mock x10LowLevelDriverMock;

		[SetUp]
		public void SetUp()
		{
			x10LowLevelDriverMock = new DynamicMock(typeof (IX10LowLevelDriver));
			x10LowLevelDriverMock.Strict = true;
			
			IX10LowLevelDriver x10LowLevelDriver = x10LowLevelDriverMock.MockInstance as IX10LowLevelDriver;
			lampController = new LampController(x10LowLevelDriver);
		}
		
		[TearDown]
		public void TearDown()
		{
			x10LowLevelDriverMock.Verify();
		}
		
		[Test]
		public void ShouldSendGreenControlCodeWhenTurningGreenLampOn()
		{
			x10LowLevelDriverMock.Expect("ControlDevice",LampController.GREEN_LAMP_DEVICE_CODE, Function.On, 0 );
			lampController.GreenLightOn = true;
		}
		
		[Test]
		public void ShouldSendRedControlCodeWhenTurningRedLampOn()
		{
			x10LowLevelDriverMock.Expect("ControlDevice",LampController.RED_LAMP_DEVICE_CODE, Function.On, 0 );
			lampController.RedLightOn = true;
		}

		[Test]
		public void ShouldSendGreenControlCodeWhenTurningGreenLampOff()
		{
			x10LowLevelDriverMock.Expect("ControlDevice",LampController.GREEN_LAMP_DEVICE_CODE, Function.Off, 0 );
			lampController.GreenLightOn = false;
		}

		[Test]
		public void ShouldSendRedControlCodeWhenTurningRedLampOff()
		{
			x10LowLevelDriverMock.Expect("ControlDevice",LampController.RED_LAMP_DEVICE_CODE, Function.Off, 0 );
			lampController.RedLightOn = false;
		}
		
		[Test]
		public void OnceTheLampHasBeenTurnedOnTurningItOnAgainDoesNotSendTheCommandAgain()
		{
			x10LowLevelDriverMock.Expect("ControlDevice",LampController.RED_LAMP_DEVICE_CODE, Function.Off, 0 );
			lampController.RedLightOn = false;
			lampController.RedLightOn = false;
			lampController.RedLightOn = false;
			x10LowLevelDriverMock.Expect("ControlDevice",LampController.RED_LAMP_DEVICE_CODE, Function.On, 0 );
			lampController.RedLightOn = true;
			lampController.RedLightOn = true;
			lampController.RedLightOn = true;

			x10LowLevelDriverMock.Expect("ControlDevice",LampController.GREEN_LAMP_DEVICE_CODE, Function.On, 0 );
			lampController.GreenLightOn = true;
			lampController.GreenLightOn = true;
			lampController.GreenLightOn = true;
			x10LowLevelDriverMock.Expect("ControlDevice",LampController.GREEN_LAMP_DEVICE_CODE, Function.Off, 0 );
			lampController.GreenLightOn = false;
			lampController.GreenLightOn = false;
			lampController.GreenLightOn = false;

		
		}

	}
}
