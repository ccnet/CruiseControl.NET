using System;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{
	public class X10Configuration
	{
		public bool Enabled = false;
		public string ComPort = "COM1";
		
		[XmlElement(DataType="time")]
		public DateTime StartTime = new DateTime(1, 1, 1, 9, 0, 0);
		[XmlElement(DataType="time")]
		public DateTime EndTime = new DateTime(1, 1, 1, 17, 0, 0);
		public DayOfWeek StartDay = DayOfWeek.Monday;
		public DayOfWeek EndDay = DayOfWeek.Friday;
		
	}
}