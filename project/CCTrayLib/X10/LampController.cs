using System;
using System.Diagnostics;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;


namespace ThoughtWorks.CruiseControl.CCTrayLib.X10
{
	public class LampController : ILampController
	{
		private readonly Lamp red;
		private readonly Lamp green;

		public LampController(X10Configuration configuration, IX10LowLevelDriver lowLevelDriver)
		{
			if (configuration != null){
	            int successUnitCode = configuration.SuccessUnitCode;
	           	int failureUnitCode = configuration.FailureUnitCode;
	            LowLevelDriverFactory factory = new LowLevelDriverFactory(configuration);
	            if (lowLevelDriver == null){
	            	lowLevelDriver = factory.getDriver();
				}
				red = new Lamp("red", failureUnitCode, lowLevelDriver);
				green = new Lamp("green", successUnitCode, lowLevelDriver);
			}
		}

		public bool RedLightOn
		{
			set { red.SetState(value ? LampState.On : LampState.Off); }
		}

		public bool GreenLightOn
		{
			set { green.SetState(value ? LampState.On : LampState.Off); }
		}

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
				Trace.WriteLine("new lamp '" + name + "' created with device code " + deviceCode);
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


	}
}
