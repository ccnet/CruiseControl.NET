using System.Diagnostics;

namespace ThoughtWorks.CruiseControl.CCTrayLib.X10
{
	public class LampController : ILampController
	{
		public const int GREEN_LAMP_DEVICE_CODE = 2;
		public const int RED_LAMP_DEVICE_CODE = 1;

		private readonly Lamp red;
		private readonly Lamp green;

		private enum LampState
		{
			Unknown,
			On,
			Off
		}

		private class Lamp
		{
			private readonly string name;
			private readonly int deviceCode;
			private readonly IX10LowLevelDriver lowLevelDriver;
			private LampState state = LampState.Unknown;

			public Lamp(string name, int deviceCode, IX10LowLevelDriver lowLevelDriver)
			{
				this.name = name;
				this.deviceCode = deviceCode;
				this.lowLevelDriver = lowLevelDriver;
			}

			public void SetState(LampState lampState)
			{
				if (lampState == state)
					return;

				Trace.WriteLine("Turning " + name + " light " + lampState);
				lowLevelDriver.ControlDevice(deviceCode, lampState == LampState.On ? Function.On : Function.Off, 0);

				state = lampState;
			}
		}


		public LampController(IX10LowLevelDriver lowLevelDriver)
		{
			red = new Lamp("red", RED_LAMP_DEVICE_CODE, lowLevelDriver);
			green = new Lamp("green", GREEN_LAMP_DEVICE_CODE, lowLevelDriver);
		}

		public bool RedLightOn
		{
			set { red.SetState(value ? LampState.On : LampState.Off); }
		}

		public bool GreenLightOn
		{
			set { green.SetState(value ? LampState.On : LampState.Off); }
		}
	}
}