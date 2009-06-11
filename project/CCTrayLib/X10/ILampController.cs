namespace ThoughtWorks.CruiseControl.CCTrayLib.X10
{
	public interface ILampController
	{
		bool RedLightOn { set; }
        bool YellowLightOn { set;}
		bool GreenLightOn { set; }
	}
}