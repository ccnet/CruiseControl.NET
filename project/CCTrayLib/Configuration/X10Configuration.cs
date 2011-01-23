using System;
using System.Xml.Serialization;
using ThoughtWorks.CruiseControl.CCTrayLib.X10;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{
	public class X10Configuration
	{
		public bool Enabled = false;
		public String ComPort = "COM1";
        public String DeviceType = ControllerType.CM17A.ToString();
        public String HouseCode = ThoughtWorks.CruiseControl.CCTrayLib.X10.HouseCode.A.ToString();
        public int SuccessUnitCode = 1;
        public int FailureUnitCode = 2;
        public int BuildingUnitCode = 3;
		[XmlElement(DataType="time")]
		public DateTime StartTime = new DateTime(2001, 1, 1, 9, 0, 0);
		[XmlElement(DataType="time")]
		public DateTime EndTime = new DateTime(2001, 1, 1, 17, 0, 0);
        public Boolean[] ActiveDays	= new Boolean[] {false,true,true,true,true,true,false};
	}
}
